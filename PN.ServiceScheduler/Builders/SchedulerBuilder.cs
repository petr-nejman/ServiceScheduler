using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PN.ServiceScheduler.Builders
{
    public class SchedulerBuilder
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly TimeProvider _timeProvider;
        private List<Registration> _registrations;
        private bool _autoRegisterJobs = true;

        public SchedulerBuilder(IServiceCollection serviceCollection, TimeProvider? timeProvider = null)
        {
            _serviceCollection = serviceCollection;
            _timeProvider = timeProvider ?? TimeProvider.System;
            _registrations = new List<Registration>();
        }

        /// <summary>
        /// Use when job instances will be provided by an external <see cref="IJobFactory"/>.
        /// When set, the builder will NOT register job implementation types into the provided
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        public SchedulerBuilder UseExternalJobFactory()
        {
            _autoRegisterJobs = false;
            return this;
        }

        /// <summary>
        /// Registers the job implementation <typeparamref name="TJob"/> as a singleton in the provided
        /// <see cref="IServiceCollection"/> and begins a registration flow for scheduling.
        /// If external job factory mode is enabled, the type will NOT be registered.
        /// </summary>
        public RegistrationBuilder AddSingletonJob<TJob>(string name) where TJob : class, Interfaces.IJob
        {
            if (_autoRegisterJobs)
            {
                _serviceCollection.TryAddSingleton<TJob>();
            }
            return new RegistrationBuilder(this, new JobDefinition(name, typeof(TJob), false), _timeProvider);
        }

        /// <summary>
        /// Registers the job implementation <typeparamref name="TJob"/> with a scoped lifetime in the provided
        /// <see cref="IServiceCollection"/> and begins a registration flow for scheduling.
        /// If external job factory mode is enabled, the type will NOT be registered.
        /// </summary>
        public RegistrationBuilder AddScopedJob<TJob>(string name) where TJob : class, Interfaces.IJob
        {
            if (_autoRegisterJobs)
            {
                _serviceCollection.TryAddScoped<TJob>();
            }
            return new RegistrationBuilder(this, new JobDefinition(name, typeof(TJob), true), _timeProvider);
        }

        /// <summary>
        /// Registers the job implementation <typeparamref name="TJob"/> with a transient lifetime in the provided
        /// <see cref="IServiceCollection"/> and begins a registration flow for scheduling.
        /// If external job factory mode is enabled, the type will NOT be registered.
        /// </summary>
        public RegistrationBuilder AddTransientJob<TJob>(string name) where TJob : class, Interfaces.IJob
        {
            if (_autoRegisterJobs)
            {
                _serviceCollection.TryAddTransient<TJob>();
            }
            return new RegistrationBuilder(this, new JobDefinition(name, typeof(TJob), false), _timeProvider);
        }

        internal void AddRegistration(Registration registration)
        {
            _registrations.Add(registration);
        }

        internal IReadOnlyList<Registration> Build() => _registrations;
    }
}
