using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Common.Helpers
{
    public class TimeMetrics : ITimeMetric, IDisposable
    {
        public const string DateTimeFormatLong = "yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff' 'K"; //:O has fffffffK
        public const string DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss"; //:s has 'T'
        public const string DateOnlyFormat = "yyyy'-'MM'-'dd";
        public const string TimeOnlyFormat = "HH':'mm':'ss";
        public const string TimeOnlyFormatLong = "HH':'mm':'ss'.'fff";

        public static IList<string> RunTimes { get; set; } =
            new List<string>();
        private static IDictionary<string, Stopwatch> _timers =
            new Dictionary<string, Stopwatch>();
        private readonly ILogger _logger;

        public TimeMetrics() : base()
        {
            _logger = LogProvider.GetLogger<TimeMetrics>();
        }

        public TimeMetrics(ILogger<TimeMetrics> logger) : base()
        {
            _logger = logger;
        }

        public void InitialiseTimer(string name)
        {
            AddTimer(name);
            StartTimer(name);
        }

        public void AddTimer(string name)
        {
            if (!_timers.ContainsKey(name))
                _timers.Add(name, new Stopwatch());
        }

        public void StartTimer(string name)
        {
            _timers[name].Start();
        }

        public void StopTimer(string name)
        {
            _timers[name].Stop();
        }

        public void ResetTimer(string name)
        {
            _timers[name].Reset();
        }

        public void RestartTimer(string name)
        {
            _timers[name].Reset();
            _timers[name].Start();
        }

        public TimeSpan GetTimeSpan(string name)
        {
            return _timers[name].Elapsed;
        }

        public string ToTimeString(string name)
        {
            var ts = _timers[name].Elapsed;
            return string.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds);
        }

        public static string ToLongTimeString(string name)
        {
            var ts = _timers[name].Elapsed;
            string elapsedTime = string.Empty;
            if (ts.Hours > 0) elapsedTime += $"{ts.Hours} hour{ts.Hours.S()}, ";
            if (ts.Minutes > 0) elapsedTime += $"{ts.Minutes} minute{ts.Minutes.S()} and ";
            if (ts.Seconds > 0) elapsedTime += $"{ts.Seconds}.{ts.Milliseconds:000} seconds";
            else if (ts.Milliseconds > 1) elapsedTime += $"{ts.Milliseconds}.{((ts.Ticks % 10000) / 10):000} milliseconds";
            else elapsedTime += $"{(ts.Ticks % 10000) / 10f} microseconds";
            return elapsedTime;
        }

        public void LogTime(string name,
            LogLevel level = LogLevel.Debug)
        {
            string time = ToLongTimeString(name);
            string message = $"{name} completed in {time}.";
            RunTimes.Add(message);
            if (_logger != null)
                _logger.Log(level, message);
        }

        public void LogStopTime(string name,
            LogLevel level = LogLevel.Debug)
        {
            try
            {
                _timers[name].Stop();
                LogTime(name, level);
            }
            catch (KeyNotFoundException ex)
            {
                if (_logger != null)
                    _logger.LogWarning(ex, "{TimerName} key not found.", name);
            }
        }

        public void LogStopResetTime(string name,
            LogLevel level = LogLevel.Debug)
        {
            LogStopTime(name, level);
            _timers[name].Reset();
        }

        public IEnumerable<string> LogAndReset()
        {
            var runTimes = new List<string>();

            foreach (var time in RunTimes)
                runTimes.Add(time);

            RunTimes.Clear();

            foreach (var name in _timers.Keys)
            {
                _timers[name].Reset();
                _timers.Remove(name);
            }

            return runTimes;
        }

        public void Dispose()
        {
            _timers?.Clear();
        }
    }

    public interface ITimeMetric
    {
        void InitialiseTimer(string name);
        void AddTimer(string name);
        void StartTimer(string name);
        void StopTimer(string name);
        TimeSpan GetTimeSpan(string name);
        void ResetTimer(string name);
        void RestartTimer(string name);
        string ToTimeString(string name);
        void LogTime(string name, LogLevel level);
        void LogStopTime(string name, LogLevel level);
    }
}
