namespace PN.ServiceScheduler.Builders
{
    public class RegistrationBuilder
    {
        private readonly SchedulerBuilder _schedulerBuilder;
        private readonly JobDefinition _jobDefinition;

        public RegistrationBuilder(SchedulerBuilder schedulerBuilder, JobDefinition jobDefinition)
        {
            _schedulerBuilder = schedulerBuilder;
            _jobDefinition = jobDefinition;
        }

        public void Every(TimeSpan interval)
        {
            _schedulerBuilder.AddRegistration(new Registration(
                _jobDefinition.Name,
                _jobDefinition.JobType,
                _jobDefinition.IsScoped,
                new Triggers.IntervalTrigger(interval)));
        }

        public void OnceAfter(TimeSpan after)
        {
            _schedulerBuilder.AddRegistration(new Registration(
                _jobDefinition.Name,
                _jobDefinition.JobType,
                _jobDefinition.IsScoped,
                new Triggers.OnceAfterTrigger(after)));
        }

        public void EveryDayAt(TimeOnly time)
        {
            _schedulerBuilder.AddRegistration(new Registration(
                _jobDefinition.Name,
                _jobDefinition.JobType,
                _jobDefinition.IsScoped,
                new Triggers.EveryDayAtTrigger(Enumerable.Repeat(time, 1))));
        }

        public void EveryDayAt(IEnumerable<TimeOnly> time)
        {
            _schedulerBuilder.AddRegistration(new Registration(
                _jobDefinition.Name,
                _jobDefinition.JobType,
                _jobDefinition.IsScoped,
                new Triggers.EveryDayAtTrigger(time)));
        }
    }
}
