using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PN.ServiceScheduler
{
    public class Scheduler : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IReadOnlyList<Registration> _registrations;
        private readonly ILogger<Scheduler> _logger;

        public Scheduler(IServiceProvider serviceProvider, IReadOnlyList<Registration> jobRegistrations, ILogger<Scheduler> logger)
        {
            _serviceProvider = serviceProvider;
            _registrations = jobRegistrations;
            _logger = logger;
        }

        #region BackgroundService

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Scheduler - Started");

            while (true)
            {
                try
                {
                    DateTime? nextRun = GetNextRunUtc();
                    if (nextRun.HasValue)
                    {
                        int delay = Math.Max(0, (int)(nextRun.Value - DateTime.UtcNow).TotalMilliseconds);
                        if (delay > 0)
                            await Task.Delay(delay, stoppingToken);
                    }
                    else
                    {
                        //Všechny triggery jsou už u konce
                        break;
                    }

                    await CheckJobs(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Scheduler - Failed");
                }
            }

            _logger.LogDebug("Scheduler - Finished");
        }

        #endregion

        private DateTime? GetNextRunUtc()
        {
            DateTime nextRun = DateTime.MaxValue;
            foreach (var registration in _registrations)
            {
                DateTime? next = registration.Trigger.GetNextRunUtc();
                if (next.HasValue && next.Value < nextRun)
                {
                    nextRun = next.Value;
                }
            }

            return nextRun < DateTime.MaxValue ? nextRun : null;
        }

        private async Task CheckJobs(CancellationToken stoppingToken)
        {
            DateTime now = DateTime.UtcNow;
            foreach (var registration in _registrations)
            {
                if (ShouldRun(registration, now))
                {
                    if (registration.RunningTask == null || registration.RunningTask.IsCompleted)
                    {
                        await ExecuteJob(registration, stoppingToken);
                    }
                    else
                    {
                        SkipJob(registration);
                    }
                }
            }
        }

        private bool ShouldRun(Registration registration, DateTime utcNow)
        {
            return registration.Trigger.GetNextRunUtc() <= utcNow;
        }

        private async Task ExecuteJob(Registration registration, CancellationToken stoppingToken)
        {
            try
            {
                var job = await registration.GetRegisteredService(_serviceProvider);

                registration.RunningTask = Task.Run(function: async () =>
                {
                    try
                    {
                        _logger.LogDebug("{Name} - Started", registration.Name);
                        await job.ExecuteAsync(stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // Ignore cancellation
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "{Name} - Failed", registration.Name);
                    }
                    finally
                    {
                        _logger.LogDebug("{Name} - Finished", registration.Name);
                    }
                }, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Name} - Failed", registration.Name);
            }
            finally
            {
                registration.Trigger.SetLastRunUtc(DateTime.UtcNow);
            }
        }

        private void SkipJob(Registration registration)
        {
            registration.Trigger.SetLastRunUtc(DateTime.UtcNow);
            _logger.LogDebug("{Name} - Skipped", registration.Name);
        }
    }
}
