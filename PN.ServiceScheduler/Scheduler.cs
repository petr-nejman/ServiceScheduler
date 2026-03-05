using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PN.ServiceScheduler.Factories;

namespace PN.ServiceScheduler
{
    public class Scheduler : BackgroundService
    {
        private readonly IJobFactory _jobFactory;
        private readonly IReadOnlyList<Registration> _registrations;
        private readonly ILogger<Scheduler> _logger;
        private readonly TimeProvider _timeProvider;

        public Scheduler(IJobFactory jobFactory, IReadOnlyList<Registration> jobRegistrations, ILogger<Scheduler> logger, TimeProvider? timeProvider = null)
        {
            _jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
            _registrations = jobRegistrations;
            _logger = logger;
            _timeProvider = timeProvider ?? TimeProvider.System;
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
                        int delay = Math.Max(0, (int)(nextRun.Value - _timeProvider.GetUtcNow().DateTime).TotalMilliseconds);
                        if (delay > 0)
                            await Task.Delay(delay, stoppingToken);
                    }
                    else
                    {
                        //All done
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
            DateTime now = _timeProvider.GetUtcNow().DateTime;
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
            var next = registration.Trigger.GetNextRunUtc();
            return next.HasValue && next.Value <= utcNow;
        }

        private async Task ExecuteJob(Registration registration, CancellationToken stoppingToken)
        {
            JobInstance? jobInstance = null;

            try
            {
                jobInstance = await _jobFactory.CreateAsync(registration, stoppingToken);

                var job = jobInstance.Job;

                registration.RunningTask = Task.Run(async () =>
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

                        if (jobInstance != null)
                        {
                            try
                            {
                                await jobInstance.DisposeAsync();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "{Name} - Failed to dispose job scope", registration.Name);
                            }
                        }
                    }
                }, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Name} - Failed", registration.Name);

                if (jobInstance != null)
                {
                    try { await jobInstance.DisposeAsync(); } catch { /* ignore */ }
                }
            }
            finally
            {
                registration.Trigger.SetLastRunUtc(_timeProvider.GetUtcNow().DateTime);
            }
        }

        private void SkipJob(Registration registration)
        {
            registration.Trigger.SetLastRunUtc(_timeProvider.GetUtcNow().DateTime);
            _logger.LogDebug("{Name} - Skipped", registration.Name);
        }
    }
}
