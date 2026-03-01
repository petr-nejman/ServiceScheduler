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

        public Type JobType => _jobType;

        public bool IsScoped => _isScoped;

        internal Task? RunningTask { get; set; }

        public Registration(string name, Type jobType, bool isScoped, ITrigger trigger)
        {
            _name = name;
            _jobType = jobType;
            _isScoped = isScoped;
            _trigger = trigger;
        }
    }
}
