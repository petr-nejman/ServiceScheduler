
namespace PN.ServiceScheduler.TestApp.Services
{
    public class ScopedService : Interfaces.IJob
    {
        private readonly ILogger<ScopedService> _logger;

        public ScopedService(ILogger<ScopedService> logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var start = DateTime.Now;
            await Task.Delay(100, stoppingToken);
            _logger.LogInformation("ScopedService stop after {Time}ms", (DateTime.Now - start).TotalMilliseconds);
        }
    }
}
