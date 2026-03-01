using PN.ServiceScheduler.Interfaces;
using System;

namespace PN.ServiceScheduler.Builders
{
    public class RegistrationBuilder
    {
        private readonly SchedulerBuilder _schedulerBuilder;
        private readonly JobDefinition _jobDefinition;
        private readonly TimeProvider _timeProvider;

        public RegistrationBuilder(SchedulerBuilder schedulerBuilder, JobDefinition jobDefinition, TimeProvider? timeProvider = null)
        {
            _schedulerBuilder = schedulerBuilder;
            _jobDefinition = jobDefinition;
            _timeProvider = timeProvider ?? TimeProvider.System;
        }

        /// <summary>
        /// Registers the job using a custom trigger.
        /// </summary>
        /// <param name="trigger">The <see cref="ITrigger"/> that controls when the job will be executed. This value must not be <c>null</c>.</param>
        public void UsingTrigger(ITrigger trigger)
        {
            _schedulerBuilder.AddRegistration(new Registration(
                _jobDefinition.Name,
                _jobDefinition.JobType,
                _jobDefinition.IsScoped,
                trigger));
        }

        /// <summary>
        /// Registers the job to run repeatedly at the specified <paramref name="interval"/>.
        /// </summary>
        /// <param name="interval">The time to wait between consecutive executions of the job.</param>
        public void Every(TimeSpan interval)
        {
            _schedulerBuilder.AddRegistration(new Registration(
                _jobDefinition.Name,
                _jobDefinition.JobType,
                _jobDefinition.IsScoped,
                new Triggers.IntervalTrigger(interval, _timeProvider)));
        }

        /// <summary>
        /// Registers the job to run once after the specified <paramref name="after"/> delay.
        /// </summary>
        /// <param name="after">The amount of time to wait before the job is executed.</param>
        public void OnceAfter(TimeSpan after)
        {
            _schedulerBuilder.AddRegistration(new Registration(
                _jobDefinition.Name,
                _jobDefinition.JobType,
                _jobDefinition.IsScoped,
                new Triggers.OnceAfterTrigger(after, _timeProvider)));
        }

        /// <summary>
        /// Registers the job to run every day at the specified <paramref name="time"/> in the given <paramref name="timeZone"/>.
        /// </summary>
        /// <param name="time">The time of day (a <see cref="TimeOnly"/>) at which the job should run each day.</param>
        /// <param name="timeZone">The <see cref="TimeZoneInfo"/> used to interpret <paramref name="time"/>.</param>
        public void EveryDayAt(TimeOnly time, TimeZoneInfo timeZone)
        {
            _schedulerBuilder.AddRegistration(new Registration(
                _jobDefinition.Name,
                _jobDefinition.JobType,
                _jobDefinition.IsScoped,
                new Triggers.EveryDayAtTrigger(Enumerable.Repeat(time, 1), timeZone, _timeProvider)));
        }

        /// <summary>
        /// Registers the job to run every day at the specified set of <paramref name="time"/> values in the given <paramref name="timeZone"/>.
        /// </summary>
        /// <param name="time">A sequence of <see cref="TimeOnly"/> values that specify the times of day when the job should run.
        /// Each value is interpreted in the context of the provided <paramref name="timeZone"/>.</param>
        /// <param name="timeZone">The <see cref="TimeZoneInfo"/> used to interpret the <paramref name="time"/> values.</param>
        public void EveryDayAt(IEnumerable<TimeOnly> time, TimeZoneInfo timeZone)
        {
            _schedulerBuilder.AddRegistration(new Registration(
                _jobDefinition.Name,
                _jobDefinition.JobType,
                _jobDefinition.IsScoped,
                new Triggers.EveryDayAtTrigger(time, timeZone, _timeProvider)));
        }
    }
}
