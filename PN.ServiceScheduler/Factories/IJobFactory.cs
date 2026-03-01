namespace PN.ServiceScheduler.Factories
{
    public interface IJobFactory
    {
        /// <summary>
        /// Creates an instance of the job for the provided registration. If the registration is scoped,
        /// the returned <see cref="JobInstance"/> holds a scope that will be disposed when the instance is disposed.
        /// </summary>
        Task<JobInstance> CreateAsync(Registration registration, CancellationToken cancellationToken = default);
    }
}
