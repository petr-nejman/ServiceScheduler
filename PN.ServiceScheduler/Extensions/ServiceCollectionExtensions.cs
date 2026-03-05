using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PN.ServiceScheduler.Builders;
using PN.ServiceScheduler.Factories;

namespace PN.ServiceScheduler.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the ServiceScheduler and its required services into the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to which scheduler services will be added.</param>
        /// <param name="configure">An action to configure a <see cref="SchedulerBuilder"/> (register jobs, triggers, etc.) before the scheduler is built and registered.</param>
        /// <param name="timeProvider">Optional <see cref="TimeProvider"/> to be used by the scheduler builder. If <c>null</c>, <see cref="TimeProvider.System"/> is used.</param>
        /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
        public static IServiceCollection AddServiceScheduler(this IServiceCollection services, Action<SchedulerBuilder> configure, TimeProvider? timeProvider = null)
        {
            var builder = new SchedulerBuilder(services, timeProvider);
            configure(builder);

            services.AddSingleton(builder.Build());
            
            services.TryAddSingleton<IJobFactory, DefaultJobFactory>();

            services.AddHostedService<Scheduler>();

            return services;
        }
    }
}
