using PN.ServiceScheduler.Interfaces;
using System.Diagnostics.Metrics;

namespace PN.ServiceScheduler.Triggers
{
    public class OnceAfterTrigger : ITrigger
    {
        private DateTime? _nextRun = null;

        public OnceAfterTrigger(TimeSpan after, TimeProvider? timeProvider = null)
        { 
            if(after <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(after), "Value must be a positive TimeSpan.");

            _nextRun = (timeProvider ?? TimeProvider.System).GetUtcNow().DateTime.Add(after);
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
