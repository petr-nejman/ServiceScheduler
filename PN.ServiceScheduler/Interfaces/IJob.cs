namespace PN.ServiceScheduler.Interfaces
{
    public interface IJob
    {
        /// <summary>
        /// Executes the job asynchronously.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        Task ExecuteAsync(CancellationToken stoppingToken);
    }
}
