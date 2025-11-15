namespace PN.ServiceScheduler.Interfaces
{
    public interface IJob
    {
        /// <summary>
        /// Executes the job asynchronously.
        /// </summary>
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
