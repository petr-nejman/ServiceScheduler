using Microsoft.Extensions.DependencyInjection;

namespace PN.ServiceScheduler.Factories
{
    public class DefaultJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Task<JobInstance> CreateAsync(Registration registration, CancellationToken cancellationToken = default)
        {
            if (registration.IsScoped)
            {
                var scope = _serviceProvider.CreateAsyncScope();
                var job = (Interfaces.IJob)scope.ServiceProvider.GetRequiredService(registration.JobType);
                var instance = new JobInstance(job, scope);
                return Task.FromResult(instance);
            }
            else
            {
                var job = (Interfaces.IJob)_serviceProvider.GetRequiredService(registration.JobType);
                var instance = new JobInstance(job, null);
                return Task.FromResult(instance);
            }
        }
    }
}
