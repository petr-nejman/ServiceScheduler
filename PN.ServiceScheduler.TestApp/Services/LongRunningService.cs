namespace PN.ServiceScheduler.TestApp.Services
{
    public class LongRunningService : Interfaces.IJob
    {
        private readonly ILogger<LongRunningService> _logger;

        public LongRunningService(ILogger<LongRunningService> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var start = DateTime.Now;
            await Task.Delay(4000, stoppingToken);
            _logger.LogInformation("LongRunningService stop after {Time}ms", (DateTime.Now - start).TotalMilliseconds);
        }
    }
}
