using System;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Cronos;

namespace Common.Helpers.BaseClass
{
    //https://docs.microsoft.com/en-us/dotnet/core/extensions/timer-service
    public abstract class TimerService<T> : TimerBase<T>, IAsyncDisposable
    {
        private System.Timers.Timer _timer;
        protected CronExpression _cronExpression;

        public TimerService(IServiceProvider serviceProvider) : base(serviceProvider) { }

        protected override void GetScheduler(string schedule)
        {
            try
            {
                if (string.IsNullOrEmpty(_schedule))
                    _logger.LogWarning("CronScheduling in AppSettings.json has not been specified.");
                else if (_schedule.Split(' ').Length > 5)
                    _cronExpression = CronExpression.Parse(_schedule, CronFormat.IncludeSeconds);
                else
                    _cronExpression = CronExpression.Parse(_schedule);
            }
            catch (CronFormatException ex)
            {
                _logger.LogError(ex, "Failed to parse Cron expression: {0}.", _schedule);
            }
        }

        protected override DateTimeOffset? GetScheduledStart()
        {
            return _cronExpression?.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local, true);
        }

        protected override async Task StartTimerAsync()
        {
            _timer = new System.Timers.Timer(_finishDelay.TotalMilliseconds);
            //_timer.Elapsed += async (sender, args) => await ScheduleNextJobAsync();
            _timer.Elapsed += TimerCallback;
            if (_startDelay > TimeSpan.Zero)
            {
                string startTime = _scheduledStart.ToString(DateTimeFormat); // fixes the culture format
                _logger.LogTrace("{0} is waiting {1:c} for the start time of {2}.", _name, _startDelay, startTime);
                await Task.Delay(_startDelay, _appToken);
            }
            _timer.Start();
            await ExecuteJobAsync();
        }

        private async void TimerCallback(object sender, ElapsedEventArgs e)
        {
            _timer?.Dispose();
            _timer = null;
            await base.ScheduleNextJobAsync();
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();
            return base.StopAsync(cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            //if (_timer != null) _timer.Elapsed -= async (sender, args) => await ScheduleNext();
            if (_timer is IAsyncDisposable timer)
                await timer.DisposeAsync();
            _timer = null;
        }
    }
}
