using PN.ServiceScheduler.Interfaces;

namespace PN.ServiceScheduler.Triggers
{
    internal class EveryDayAtTrigger : ITrigger
    {
        private DateTime? _nextRun = null;

        private readonly IReadOnlyList<TimeOnly> _timeUtc;

        public EveryDayAtTrigger(IEnumerable<TimeOnly> timeUtc)
        {
            _timeUtc = timeUtc.OrderBy(t => t).Distinct().ToList();
            _nextRun = GetNext(DateTime.UtcNow);
        }

        public DateTime? GetNextRunUtc()
        {
            return _nextRun;
        }

        public void SetLastRunUtc(DateTime lastRunUtc)
        {
            _nextRun = GetNext(lastRunUtc);
        }

        public bool ShouldRun(DateTime nowUtc)
        {
            return nowUtc >= _nextRun;
        }

        private DateTime? GetNext(DateTime utcNow)
        {
            if (_timeUtc.Count == 0) return null;

            TimeOnly now = TimeOnly.FromDateTime(utcNow);
            TimeOnly? nextTime = null;

            foreach (var time in _timeUtc)
            {
                if(now < time)
                {
                    nextTime = time;
                    break;
                }
            }

            if (nextTime.HasValue)
                return utcNow.Date.Add(nextTime.Value.ToTimeSpan());
            else
                return utcNow.Date.AddDays(1).Add(_timeUtc[0].ToTimeSpan());
        }
    }
}
