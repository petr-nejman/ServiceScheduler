using Microsoft.Extensions.DependencyInjection;
using PN.ServiceScheduler.Builders;

namespace PN.ServiceScheduler.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceScheduler(this IServiceCollection services, Action<SchedulerBuilder> configure)
        {
            var builder = new SchedulerBuilder(services);
            configure(builder);

            services.AddSingleton(builder.Build());
            services.AddHostedService<Scheduler>();

            return services;
        }
    }
}
