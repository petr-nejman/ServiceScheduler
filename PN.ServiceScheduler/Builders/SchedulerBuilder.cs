using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PN.ServiceScheduler.Builders
{
    public class SchedulerBuilder
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly TimeProvider _timeProvider;
        private List<Registration> _registrations;

        public SchedulerBuilder(IServiceCollection serviceCollection, TimeProvider? timeProvider = null)
        {
            _serviceCollection = serviceCollection;
            _timeProvider = timeProvider ?? TimeProvider.System;
            _registrations = new List<Registration>();
        }

        /// <summary>
        /// Registers the job implementation <typeparamref name="TJob"/> as a singleton in the provided
        /// <see cref="IServiceCollection"/> and begins a registration flow for scheduling.
        /// </summary>
        /// <typeparam name="TJob">The job implementation type. Must implement <see cref="Interfaces.IJob"/>.</typeparam>
        /// <param name="name">A unique logical name for the job used by the scheduler to identify the registration.</param>
        /// <returns>
        /// A <see cref="RegistrationBuilder"/> that can be used to configure triggers, scheduling details and
        /// finalize the job registration.
        /// </returns>
        public RegistrationBuilder AddSingletonJob<TJob>(string name) where TJob : class, Interfaces.IJob
        {
            _serviceCollection.TryAddSingleton<TJob>();
            return new RegistrationBuilder(this, new JobDefinition(name, typeof(TJob), false), _timeProvider);
        }

        /// <summary>
        /// Registers the job implementation <typeparamref name="TJob"/> with a scoped lifetime in the provided
        /// <see cref="IServiceCollection"/> and begins a registration flow for scheduling.
        /// </summary>
        /// <typeparam name="TJob">The job implementation type. Must implement <see cref="Interfaces.IJob"/>.</typeparam>
        /// <param name="name">A unique logical name for the job used by the scheduler to identify the registration.</param>
        /// <returns>
        /// A <see cref="RegistrationBuilder"/> that can be used to configure triggers, scheduling details and
        /// finalize the job registration.
        /// </returns>
        public RegistrationBuilder AddScopedJob<TJob>(string name) where TJob : class, Interfaces.IJob
        {
            _serviceCollection.TryAddScoped<TJob>();
            return new RegistrationBuilder(this, new JobDefinition(name, typeof(TJob), true), _timeProvider);
        }

        /// <summary>
        /// Registers the job implementation <typeparamref name="TJob"/> with a transient lifetime in the provided
        /// <see cref="IServiceCollection"/> and begins a registration flow for scheduling.
        /// </summary>
        /// <typeparam name="TJob">The job implementation type. Must implement <see cref="Interfaces.IJob"/>.</typeparam>
        /// <param name="name">A unique logical name for the job used by the scheduler to identify the registration.</param>
        /// <returns>
        /// A <see cref="RegistrationBuilder"/> that can be used to configure triggers, scheduling details and
        /// finalize the job registration.
        /// </returns>
        public RegistrationBuilder AddTransientJob<TJob>(string name) where TJob : class, Interfaces.IJob
        {
            _serviceCollection.TryAddTransient<TJob>();
            return new RegistrationBuilder(this, new JobDefinition(name, typeof(TJob), false), _timeProvider);
        }

        internal void AddRegistration(Registration registration)
        {
            _registrations.Add(registration);
        }

        internal IReadOnlyList<Registration> Build() => _registrations;
    }
}
