using Microsoft.Extensions.DependencyInjection;
using PN.ServiceScheduler.Interfaces;

namespace PN.ServiceScheduler
{
    public class Registration
    {
        private readonly string _name;
        private readonly Type _jobType;
        private readonly bool _isScoped;
        private readonly ITrigger _trigger;

        public string Name => _name;

        public ITrigger Trigger => _trigger;

        internal Task? RunningTask { get; set; }

        public Registration(string name, Type jobType, bool isScoped, ITrigger trigger)
        {
            _name = name;
            _jobType = jobType;
            _isScoped = isScoped;
            _trigger = trigger;
        }

        public async Task<IJob> GetRegisteredService(IServiceProvider serviceProvider)
        {
            if (_isScoped)
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                return (IJob)scope.ServiceProvider.GetRequiredService(_jobType);
            }
            else
            {
                return (IJob)serviceProvider.GetRequiredService(_jobType);
            }
        }
    }
}
