using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan GetDelayFromNow(this TimeSpan relativeTime)
            => relativeTime.GetAbsoluteTime().GetDelayFromNow();

        public static TimeSpan GetDelayFromNow(this DateTime scheduledTime, TimeSpan? timeToNext = null)
        {
            var dateTimeNow = DateTime.Now;
            var delay = (dateTimeNow < scheduledTime) ?
                scheduledTime - dateTimeNow : TimeSpan.Zero;
            if (timeToNext is TimeSpan timeToAdd)
                if (timeToAdd > TimeSpan.Zero)
                    delay += timeToAdd;
            return delay;
        }

        public static DateTime GetAbsoluteTime(this TimeSpan relativeTime, ushort? daysToAdd = null)
        {
            var dateTimeNow = DateTime.Now;

            var scheduledTime = new DateTime(
                dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day,
                relativeTime.Hours, relativeTime.Minutes, relativeTime.Seconds);

            if (relativeTime.Days != 0)
                scheduledTime = scheduledTime.AddDays(relativeTime.Days);

            if (daysToAdd > 0 && scheduledTime < dateTimeNow)
                scheduledTime = scheduledTime.AddDays(daysToAdd.Value);

            return scheduledTime;
        }

        public static TimeSpan GetRelativeTime(this DateTime absoluteTime, bool getDays = false)
        {
            int daysDiff = getDays ? (absoluteTime - DateTime.Now).Days : 0;
            return new TimeSpan(daysDiff, absoluteTime.Hour, absoluteTime.Minute, absoluteTime.Second);
        }

        //public static async Task HeartbeatAsync(this TimeSpan interval, Action action,
        //    CancellationToken reset, CancellationToken app)
        //{
        //    while (!app.IsCancellationRequested)
        //    {
        //        await Task.Delay(interval, reset);
        //        if (!reset.IsCancellationRequested)
        //            action();
        //    }
        //}

        public static string ToTimeString(this TimeSpan ts)
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds); //:c doesn't have ms
        }

        public static string ToShortTimeString(this TimeSpan ts)
        {
            var sb = new StringBuilder();
            if (ts.TotalDays >= 1)
            {
                sb.Append($"{ts.Days}.");
                sb.Append($"{ts.Hours:00}:");
                sb.Append($"{ts.Minutes:00}:");
                sb.Append($"{ts.Seconds:00}");
            }
            else if (ts.TotalMinutes >= 1)
            {
                if (ts.Hours > 0) sb.Append($"{ts.Hours:00}:");
                sb.Append($"{ts.Minutes:00}:");
                sb.Append($"{ts.Seconds:00}");
            }
            else if (ts.TotalSeconds >= 1)
            {
                sb.Append($"{ts.Seconds}.{ts.Milliseconds:000}s");
            }
            else if (ts.TotalMilliseconds >= 1)
            {
                sb.Append($"{ts.Milliseconds}.{((ts.Ticks % 10000) / 10):000}ms");
            }
            else
            {
                sb.Append($"{(ts.Ticks % 10000) / 10f}us");
            }
            return sb.ToString();
        }

        public static string ToLongTimeString(this TimeSpan ts)
        {
            var sb = new StringBuilder();
            if (ts.Days > 0) sb.Append($"{ts.Days} day{ts.Days.S()}, ");
            if (ts.Hours > 0) sb.Append($"{ts.Hours} hour{ts.Hours.S()}, ");
            if (ts.Minutes > 0) sb.Append($"{ts.Minutes} minute{ts.Minutes.S()} and ");
            if (ts.Seconds > 0) sb.Append($"{ts.Seconds}.{ts.Milliseconds:000} seconds");
            else if (ts.Milliseconds > 1) sb.Append($"{ts.Milliseconds}.{((ts.Ticks % 10000) / 10):000} milliseconds");
            else sb.Append($"{(ts.Ticks % 10000) / 10f} microseconds");
            return sb.ToString();
        }

        public static string ToDateName(this DateTime date)
            => string.Format("{0:yyyyMMdd}", date);

        public static string ToDateTimeName(this DateTime dateTime)
            => string.Format("{0:yyyyMMdd_HHmmss}", dateTime);

        public static string ToDateString(this DateTime date)
            => string.Format("{0:yyyy'-'MM'-'dd}", date);

        public static string ToTimeString(this DateTime time)
            => string.Format("{0:HH':'mm':'ss'.'fff}", time);

        public static string ToDateTimeString(this DateTime dateTime)
            => string.Format("{0:yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff}", dateTime); //:s has 'T'

        public static string ToDateOptionalTimeString(this DateTime dateTime)
            => dateTime.Hour == 0 && dateTime.Minute == 0 && dateTime.Second == 0 ?
                dateTime.ToDateString() : dateTime.ToDateTimeString();

        public static string ToDateString(this DateTimeOffset date)
            => string.Format("{0:yyyy'-'MM'-'dd' 'K}", date);

        public static string ToTimeString(this DateTimeOffset time)
            => string.Format("{0:HH':'mm':'ss'.'fff' 'K}", time);

        public static string ToDateTimeString(this DateTimeOffset dateTime)
            => string.Format("{0:yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff' 'K}", dateTime); //:O has fffffffK
    }
}
