using PN.ServiceScheduler.Interfaces;
using System;

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

        public void UsingTrigger(ITrigger trigger)
        {
            _schedulerBuilder.AddRegistration(new Registration(
                _jobDefinition.Name,
                _jobDefinition.JobType,
                _jobDefinition.IsScoped,
                trigger));
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

        public void EveryDayAt(TimeOnly time, TimeZoneInfo timeZone)
        {
            _schedulerBuilder.AddRegistration(new Registration(
                _jobDefinition.Name,
                _jobDefinition.JobType,
                _jobDefinition.IsScoped,
                new Triggers.EveryDayAtTrigger(Enumerable.Repeat(time, 1), timeZone)));
        }

        public void EveryDayAt(IEnumerable<TimeOnly> time, TimeZoneInfo timeZone)
        {
            _schedulerBuilder.AddRegistration(new Registration(
                _jobDefinition.Name,
                _jobDefinition.JobType,
                _jobDefinition.IsScoped,
                new Triggers.EveryDayAtTrigger(time, timeZone)));
        }
    }
}
