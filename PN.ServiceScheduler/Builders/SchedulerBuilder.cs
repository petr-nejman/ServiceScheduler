using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PN.ServiceScheduler.Builders
{
    public class SchedulerBuilder
    {
        private readonly IServiceCollection _serviceCollection;
        private List<Registration> _registrations;

        public SchedulerBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
            _registrations = new List<Registration>();
        }

        public RegistrationBuilder AddSingletonJob<TJob>(string name) where TJob : class, Interfaces.IJob
        {
            _serviceCollection.TryAddSingleton<TJob>();
            return new RegistrationBuilder(this, new JobDefinition(name, typeof(TJob), false));
        }

        public RegistrationBuilder AddScopedJob<TJob>(string name) where TJob : class, Interfaces.IJob
        {
            _serviceCollection.TryAddScoped<TJob>();
            return new RegistrationBuilder(this, new JobDefinition(name, typeof(TJob), true));
        }

        public RegistrationBuilder AddTransientJob<TJob>(string name) where TJob : class, Interfaces.IJob
        {
            _serviceCollection.TryAddTransient<TJob>();
            return new RegistrationBuilder(this, new JobDefinition(name, typeof(TJob), false));
        }

        internal void AddRegistration(Registration registration)
        {
            _registrations.Add(registration);
        }

        internal IReadOnlyList<Registration> Build() => _registrations;
    }
}
