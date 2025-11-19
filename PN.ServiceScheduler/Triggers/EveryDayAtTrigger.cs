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

        private DateTime? GetNext(DateTime utcNow)
        {
            if (_time.Count == 0) return null;

            DateTimeOffset now = TimeZoneInfo.ConvertTime(new DateTimeOffset(utcNow, TimeSpan.Zero), _timeZone);
            DateOnly day = DateOnly.FromDateTime(now.DateTime);
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

            if (!nextTime.HasValue)
            {
                day = day.AddDays(1);
                nextTime = _time[0];
            }

            return new DateTimeOffset(day, nextTime.Value, _timeZone.GetUtcOffset(new DateTime(day, nextTime.Value))).UtcDateTime;
        }
    }
}
