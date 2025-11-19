using PN.ServiceScheduler.Interfaces;

namespace PN.ServiceScheduler.Triggers
{
    public class OnceAfterTrigger : ITrigger
    {
        private DateTime? _nextRun = null;

        public OnceAfterTrigger(TimeSpan after)
        {
            _nextRun = DateTime.UtcNow.Add(after);
        }
        public DateTime? GetNextRunUtc()
        {
            return _nextRun;
        }

        public void SetLastRunUtc(DateTime lastRunUtc)
        {
            _nextRun = null;
        }
    }
}
