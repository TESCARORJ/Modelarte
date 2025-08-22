// Application/BackgroundServices/DailyReminderBackgroundService.cs
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ByTescaro.ConstrutorApp.Application.Interfaces;
using ByTescaro.ConstrutorApp.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ByTescaro.ConstrutorApp.Application.BackgroundServices
{
    public class DailyReminderBackgroundService : BackgroundService
    {
        private readonly ILogger<DailyReminderBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

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
                    
                    try
                    {
                        var activeReminders = await configuracaoService.GetActiveDailyRemindersAsync();
                        var now = DateTime.Now;

                        var nextExecutionTime = activeReminders
                            .Select(r => now.Date.Add(r.HoraDoDia))
                            .Where(t => t > now)
                            .OrderBy(t => t)
                            .FirstOrDefault();

                        if (nextExecutionTime == default)
                        {
                            var firstReminderTomorrow = activeReminders
                                .OrderBy(r => r.HoraDoDia)
                                .FirstOrDefault();

                            if (firstReminderTomorrow != null)
                            {
                                nextExecutionTime = now.Date.AddDays(1).Add(firstReminderTomorrow.HoraDoDia);
                                _logger.LogInformation("Nenhum lembrete futuro para hoje. Próximo agendado para amanhã às {NextTime:hh\\:mm\\:ss}.", nextExecutionTime.TimeOfDay);
                            }
                            else
                            {
                                _logger.LogWarning("Nenhuma configuração de lembrete diário ativa encontrada. Verificando novamente em 1 hora.");
                                delayTime = TimeSpan.FromMinutes(10);
                                await Task.Delay(delayTime, stoppingToken);
                                continue; // Pula para a próxima iteração
                            }
                        }
                        
                        delayTime = nextExecutionTime - now;

                        if (delayTime < TimeSpan.Zero)
                        {
                             delayTime = TimeSpan.FromSeconds(5); // Atraso de segurança
                            _logger.LogWarning("O tempo de atraso calculado foi negativo. Ajustando para {DelayTime} para reavaliação.", delayTime);
                        }

                        _logger.LogInformation("Aguardando por {DelayTime} para a próxima execução às {ExecutionTime}.", delayTime, nextExecutionTime);
                        await Task.Delay(delayTime, stoppingToken);

                        // Verifica se o cancelamento foi solicitado durante o delay
                        if (stoppingToken.IsCancellationRequested) break;

                        _logger.LogInformation("Iniciando o envio de lembretes diários.");
                        var dailyReminderService = scope.ServiceProvider.GetRequiredService<DailyReminderService>();
                        await dailyReminderService.SendDailyRemindersAsync();
                        _logger.LogInformation("Envio de lembretes diários concluído.");

                    }
                    catch (OperationCanceledException)
                    {
                        // Exceção esperada quando o serviço está parando.
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro inesperado no serviço de lembretes diários. Tentando novamente em 5 minutos.");
                        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    }
                }
            }

            _logger.LogInformation("Serviço de Lembretes Diários parado em {StopTime}.", DateTimeOffset.Now);
        }
    }
}