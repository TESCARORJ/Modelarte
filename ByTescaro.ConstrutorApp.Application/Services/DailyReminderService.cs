// Em ByTescaro.ConstrutorApp.Domain.Services/DailyReminderService.cs
using Azure.Core;
using ByTescaro.ConstrutorApp.Application.DTOs;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq; // Necessário para .Select e .ToList()
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Services
{
    public class DailyReminderService
    {
        private readonly IConfiguracaoLembreteDiarioService _configuracaoLembreteDiarioService;
        private readonly IHolidaysService _holidaysService;
        private readonly INotificationService _notificationService;
        private readonly IAgendaService _agendaService;
        private readonly ILogger<DailyReminderService> _logger;

        public DailyReminderService(
            IConfiguracaoLembreteDiarioService configuracaoLembreteDiarioService,
            IHolidaysService holidaysService,
            INotificationService notificationService,
            IAgendaService agendaService,
            ILogger<DailyReminderService> logger)
        {
            _configuracaoLembreteDiarioService = configuracaoLembreteDiarioService;
            _holidaysService = holidaysService;
            _notificationService = notificationService;
            _agendaService = agendaService;
            _logger = logger;
        }

        public async Task SendDailyRemindersAsync()
        {
            _logger.LogInformation("Iniciando verificação de lembretes diários para {TodayDate:dd/MM/yyyy}.", DateTime.Today);

            if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
            {
                _logger.LogInformation($"Hoje é fim de semana ({DateTime.Today.DayOfWeek}). Nenhum lembrete diário será enviado.");
                return;
            }
            _logger.LogInformation("Hoje é dia útil.");

            bool isHoliday = await _holidaysService.IsHolidayAsync(DateTime.Today);
            if (isHoliday)
            {
                _logger.LogInformation($"Hoje é feriado ({DateTime.Today:dd/MM/yyyy}). Nenhum lembrete diário será enviado.");
                return;
            }
            _logger.LogInformation("Hoje não é feriado.");

            var activeReminders = await _configuracaoLembreteDiarioService.GetActiveDailyRemindersAsync();
            _logger.LogInformation($"Encontradas {activeReminders.Count()} configurações de lembretes diários ativas.");

            if (!activeReminders.Any())
            {
                _logger.LogInformation("Nenhuma configuração de lembrete diário ativa encontrada. Encerrando.");
                return;
            }

            var today = DateTime.Today;
            // Garante que o GetEventosByDateRangeAsync na AgendaService inclua os participantes e seus usuários
            var eventsToday = await _agendaService.GetEventosByDateRangeAsync(today, today.AddDays(1).AddTicks(-1));
            _logger.LogInformation($"Encontrados {eventsToday.Count()} eventos agendados para hoje.");

            if (!eventsToday.Any())
            {
                _logger.LogInformation("Nenhum evento agendado para hoje. Nenhum lembrete de evento será enviado.");
                return;
            }

            // Agrupa os eventos pelos participantes para enviar uma única mensagem por participante
            var remindersByParticipant = new Dictionary<long, List<EventoDto>>();

            foreach (var evento in eventsToday)
            {
                foreach (var participante in evento.Participantes)
                {
                    // Adiciona o evento à lista do participante
                    if (!remindersByParticipant.ContainsKey(participante.UsuarioId))
                    {
                        remindersByParticipant[participante.UsuarioId] = new List<EventoDto>();
                    }
                    remindersByParticipant[participante.UsuarioId].Add(evento);
                }
            }
            _logger.LogInformation($"Eventos agrupados por {remindersByParticipant.Count} participantes únicos.");

            var nowTime = DateTime.Now.TimeOfDay;
            _logger.LogInformation($"Hora atual de processamento: {nowTime:hh\\:mm\\:ss}.");

            foreach (var reminderConfig in activeReminders)
            {
                _logger.LogInformation($"Verificando configuração de lembrete para a hora: {reminderConfig.HoraDoDia:hh\\:mm}. Descrição: {reminderConfig.Descricao}.");

                if (reminderConfig.HoraDoDia <= nowTime)
                {
                    _logger.LogInformation($"Configuração de lembrete para {reminderConfig.HoraDoDia:hh\\:mm} é <= hora atual. Processando.");

                    if (!remindersByParticipant.Any())
                    {
                        _logger.LogWarning("Não há participantes com eventos para enviar lembretes nesta configuração de hora.");
                        continue; // Próxima configuração de lembrete
                    }

                    foreach (var entry in remindersByParticipant)
                    {
                        var userId = entry.Key;
                        var usersEvents = entry.Value; // Lista de eventos para este usuário específico

                        var stringBuilder = new StringBuilder();
                        //stringBuilder.AppendLine($"LEMBRETE AUTOMÁTICO MODELARTE");
                        stringBuilder.AppendLine($"Olá! Aqui estão seus compormissos para hoje *{DateTime.Today:dd/MM/yyyy}*.");
                        stringBuilder.AppendLine();

                        foreach (var evento in usersEvents.OrderBy(e => e.DataHoraInicio))
                        {
                            // === CORREÇÃO AQUI: Construindo a string de participantes para CADA EVENTO ===
                            string participantesEventoString = evento.Participantes != null && evento.Participantes.Any()
                                ? string.Join(", ", evento.Participantes.Select(p => p.NomeUsuario).Where(n => !string.IsNullOrEmpty(n)))
                                : "Nenhum participante adicional"; // Ou "Você é o único participante"
                                                                   // Considerar se o criador do evento deve ser incluído na lista de participantes aqui,
                                                                   // se ele não estiver explicitamente na lista de Participantes.
                                                                   // string criadorNome = evento.NomeCriador;

                            stringBuilder.AppendLine(new string('*', 48)); 
                            stringBuilder.AppendLine($"- *{evento.Titulo}* às {evento.DataHoraInicio:HH:mm} (Início: {evento.DataHoraInicio:dd/MM})");
                            stringBuilder.AppendLine($"*Conteúdo*:"); // Removido a quebra de linha extra aqui
                            stringBuilder.AppendLine($"{evento.Descricao}"); // Removido a quebra de linha extra aqui
                            stringBuilder.AppendLine($"*Participantes*: {participantesEventoString}");
                            stringBuilder.AppendLine(); // Adiciona uma linha em branco entre os eventos
                        }

                        stringBuilder.AppendLine("Tenha um ótimo dia!");

                        var finalMessage = stringBuilder.ToString();
                        var subject = reminderConfig.Descricao ?? "Lembretes Diários de Eventos - MODELARTE";

                        _logger.LogInformation($"Tentando enviar lembrete de eventos para o usuário ID: {userId}. Assunto: '{subject}'");
                        await _notificationService.SendNotificationAsync(userId, subject, finalMessage);
                        _logger.LogInformation($"Lembrete de eventos enviado via WhatsApp para o usuário ID: {userId}.");
                    }
                }
                else
                {
                    _logger.LogInformation($"Configuração de lembrete para {reminderConfig.HoraDoDia:hh\\:mm} ainda não chegou. Pulando.");
                }
            }
            _logger.LogInformation($"Total de {remindersByParticipant.Count} usuários participantes com lembretes diários processados e potenciais envios de WhatsApp.");
            _logger.LogInformation("Verificação de lembretes diários concluída.");
        }
    }
}