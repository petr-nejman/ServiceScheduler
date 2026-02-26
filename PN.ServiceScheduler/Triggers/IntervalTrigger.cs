using PN.ServiceScheduler.Interfaces;

namespace PN.ServiceScheduler.Triggers
{
    public class IntervalTrigger : ITrigger
    {
        private DateTime _nextRun;

        private readonly TimeSpan _interval;

        public IntervalTrigger(TimeSpan interval, TimeProvider? timeProvider = null)
        {
            _interval = interval;
            _nextRun = (timeProvider ?? TimeProvider.System).GetUtcNow().DateTime;
        }

        public DateTime? GetNextRunUtc()
        {
            return _nextRun;
        }

        public void SetLastRunUtc(DateTime lastRunUtc)
        {
            _nextRun = lastRunUtc.Add(_interval);
        }
    }
}
