using PN.ServiceScheduler.Interfaces;

namespace PN.ServiceScheduler.Triggers
{
    internal class EveryDayAtTrigger : ITrigger
    {
        private DateTime? _nextRun = null;

        private readonly IReadOnlyList<TimeOnly> _time;
        private readonly TimeZoneInfo _timeZone;

        public EveryDayAtTrigger(IEnumerable<TimeOnly> time, TimeZoneInfo timeZone)
        {
            _time = time.OrderBy(t => t).Distinct().ToList();
            _timeZone = timeZone;
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
            if (_time.Count == 0) return null;

            DateTimeOffset now = TimeZoneInfo.ConvertTime(new DateTimeOffset(utcNow, TimeSpan.Zero), _timeZone);
            TimeOnly nowTime =  TimeOnly.FromDateTime(now.DateTime);
            TimeOnly? nextTime = null;

            foreach (var time in _time)
            {
                if(nowTime < time)
                {
                    nextTime = time;
                    break;
                }
            }

            var next = now.Add(-now.TimeOfDay);

            if (nextTime.HasValue)
                next = next.Add(nextTime.Value.ToTimeSpan());
            else
                next = next.AddDays(1).Add(_time[0].ToTimeSpan());
            
            return next.UtcDateTime;
        }
    }
}
