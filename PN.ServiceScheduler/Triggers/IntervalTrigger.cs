using PN.ServiceScheduler.Interfaces;

namespace PN.ServiceScheduler.Triggers
{
    public class IntervalTrigger : ITrigger
    {
        private DateTime _nextRun = DateTime.UtcNow;

        private readonly TimeSpan _interval;

        public IntervalTrigger(TimeSpan interval)
        {
            _interval = interval;
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
