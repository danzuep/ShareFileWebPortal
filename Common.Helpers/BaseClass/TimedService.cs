using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Common.Helpers.Models;
using Cronos;

namespace Common.Helpers.BaseClass
{
    public abstract class TimedService<T> : TimerBase<T>, IDisposable
    {
        public static string SchedulingConfig { get; set; } = $"Config:{SchedulingOptions.Name}";

        private Timer _timer;
        protected CronExpression _cronExpression;

        public TimedService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            if (_schedulingOptions == null)
                _scheduling = _configuration.GetSection(SchedulingConfig).Get<SchedulingOptions>();
        }

        protected override void GetScheduler(string schedule)
        {
            try
            {
                if (string.IsNullOrEmpty(schedule))
                    _logger.LogWarning("CronScheduling in AppSettings.json has not been specified.");
                else if (schedule.Split(' ').Length > 5)
                    _cronExpression = CronExpression.Parse(schedule, CronFormat.IncludeSeconds);
                else
                    _cronExpression = CronExpression.Parse(schedule);
            }
            catch (CronFormatException ex)
            {
                _logger.LogError(ex, "Failed to parse Cron expression: {0}.", schedule);
            }
        }

        protected override DateTimeOffset? GetScheduledStart()
        {
            return _cronExpression?.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local, true);
        }

        protected override Task StartTimerAsync()
        {
            _timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            if (_startDelay > TimeSpan.Zero)
            {
                string startTime = _scheduledStart.ToString(DateTimeFormat); // fixes the culture format
                _logger.LogTrace("{0} is waiting {1:c} for the start time of {2}.", _name, _startDelay, startTime);
            }
            _timer?.Change(_startDelay, _finishDelay);
            return _completedTask;
        }

        private async void TimerCallback(object stateInfo)
        {
            //var autoEvent = stateInfo as AutoResetEvent?;
            _timer?.Dispose();
            _timer = null;
            await ExecuteJobAsync();
            await ScheduleNextJobAsync();
        }

        public override Task StopAsync(CancellationToken cancellationToken = default)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return base.StopAsync(cancellationToken);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
