// Application/BackgroundServices/DailyReminderBackgroundService.cs
using ByTescaro.ConstrutorApp.Application.Interfaces; // Adicione este using
using ByTescaro.ConstrutorApp.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq; // Para usar LINQ

namespace ByTescaro.ConstrutorApp.Application.BackgroundServices
{
    public class DailyReminderBackgroundService : BackgroundService
    {
        private readonly ILogger<DailyReminderBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        // Não precisamos injetar IConfiguracaoLembreteDiarioService diretamente aqui,
        // pois já estamos usando um scope para obter o DailyReminderService,
        // que por sua vez obtém IConfiguracaoLembreteDiarioService.
        // A lógica de agendamento precisa apenas do tempo.

        public DailyReminderBackgroundService(ILogger<DailyReminderBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de Lembretes Diários iniciado em {StartTime}.", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                TimeSpan delayTime;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var configuracaoService = scope.ServiceProvider.GetRequiredService<IConfiguracaoLembreteDiarioService>();
                    var dailyReminderService = scope.ServiceProvider.GetRequiredService<DailyReminderService>();

                    try
                    {
                        // 1. Obter todas as configurações de lembretes diários ativas
                        var activeReminders = await configuracaoService.GetActiveDailyRemindersAsync();

                        // 2. Calcular o próximo horário de execução
                        var now = DateTime.Now;
                        var today = now.Date;

                        // Encontrar o próximo horário configurado para hoje que ainda não passou
                        var nextReminderTimeToday = activeReminders
                            .Where(r => r.HoraDoDia > now.TimeOfDay) // Horários no futuro
                            .OrderBy(r => r.HoraDoDia)
                            .FirstOrDefault();

                        if (nextReminderTimeToday != null)
                        {
                            // Há um lembrete para hoje no futuro
                            var nextExecutionDateTime = today.Add(nextReminderTimeToday.HoraDoDia);
                            delayTime = nextExecutionDateTime - now;
                            _logger.LogInformation("Próximo lembrete agendado para hoje às {NextTime:hh\\:mm\\:ss}.", nextExecutionDateTime.TimeOfDay);
                        }
                        else
                        {
                            // Todos os lembretes para hoje já passaram ou não há lembretes configurados para hoje.
                            // Agendar para o primeiro lembrete do dia seguinte.
                            var firstReminderTimeTomorrow = activeReminders
                                .OrderBy(r => r.HoraDoDia)
                                .FirstOrDefault();

                            if (firstReminderTimeTomorrow != null)
                            {
                                var nextDay = today.AddDays(1);
                                var nextExecutionDateTime = nextDay.Add(firstReminderTimeTomorrow.HoraDoDia);
                                delayTime = nextExecutionDateTime - now;
                                _logger.LogInformation("Todos os lembretes para hoje já passaram ou não há mais lembretes para hoje. Agendando para o primeiro lembrete de amanhã às {NextTime:hh\\:mm\\:ss}.", nextExecutionDateTime.TimeOfDay);
                            }
                            else
                            {
                                // Não há configurações de lembrete ativas. Espera um tempo razoável (ex: 1 hora) e verifica novamente.
                                _logger.LogWarning("Nenhuma configuração de lembrete diário ativa encontrada. Verificando novamente em 1 hora.");
                                delayTime = TimeSpan.FromHours(1);
                            }
                        }

                        // Garante que o atraso seja positivo para evitar exceções
                        if (delayTime < TimeSpan.Zero)
                        {
                            _logger.LogWarning("Tempo de atraso calculado foi negativo ({DelayTime}). Ajustando para 1 segundo para evitar erro e reavaliar.", delayTime);
                            delayTime = TimeSpan.FromSeconds(1); // Atraso mínimo para reavaliar
                        }

                        // Se o atraso for muito pequeno (ex: milissegundos), defina um mínimo para evitar loop intenso.
                        if (delayTime < TimeSpan.FromSeconds(1))
                        {
                            _logger.LogInformation("Tempo de atraso muito pequeno ({DelayTime}). Ajustando para 1 segundo.", delayTime);
                            delayTime = TimeSpan.FromSeconds(1);
                        }

                        // Executa o serviço DailyReminderService imediatamente se o tempo já passou ou é agora
                        // (o que significa que nextReminderTimeToday seria null ou muito próximo de now)
                        // ou se estamos apenas aguardando o próximo dia.
                        // A lógica de SendDailyRemindersAsync já filtra o que deve ser enviado "agora".
                        await dailyReminderService.SendDailyRemindersAsync();

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro inesperado ao calcular o próximo agendamento ou executar SendDailyRemindersAsync.");
                        delayTime = TimeSpan.FromMinutes(5); // Em caso de erro, tenta novamente após 5 minutos.
                    }
                }

                _logger.LogInformation("Aguardando por {DelayTime} antes da próxima verificação.", delayTime);
                await Task.Delay(delayTime, stoppingToken);
            }

            _logger.LogInformation("Serviço de Lembretes Diários parado em {StopTime}.", DateTimeOffset.Now);
        }
    }
}