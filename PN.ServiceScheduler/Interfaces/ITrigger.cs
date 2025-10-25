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

        /// <summary>
        /// Determines whether the job should run at the specified UTC time.
        /// </summary>
        /// <param name="nowUtc">The current time in UTC.</param>
        /// <returns>True if the job should run; otherwise, false.</returns>
        bool ShouldRun(DateTime nowUtc);
    }
}
