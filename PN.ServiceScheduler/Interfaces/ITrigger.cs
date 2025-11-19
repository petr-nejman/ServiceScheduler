namespace PN.ServiceScheduler.Interfaces
{
    /// <summary>
    /// Defines a trigger that determines when a job should run.
    /// </summary>
    public interface ITrigger
    {
        /// <summary>
        /// Gets the next scheduled run time in UTC.
        /// </summary>
        /// <returns>The next run time in UTC, or null if there are no more scheduled runs.</returns>
        DateTime? GetNextRunUtc();

        /// <summary>
        /// Sets the last run time in UTC.
        /// </summary>
        /// <param name="lastRunUtc">The last run time in UTC.</param>
        void SetLastRunUtc(DateTime lastRunUtc);
    }
}
