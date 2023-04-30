using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Cronos;
using Common.Helpers.Models;

namespace Common.Helpers.BaseClass
{
    public abstract class PeriodicWorker<T> : TimerBase<T>, IDisposable
    {
        private PeriodicTimer? _timer;
        protected CronExpression? _cronExpression;
        public virtual string SectionName { get; set; } = SchedulingOptions.Name;

        // To use IOptions, add either of the following to Program.cs:
        // services.Configure<SchedulingOptions>(configuration.GetSection($"Config:{SchedulingOptions.Name}"));
        // services.AddOptions<SchedulingOptions>().Bind(configuration.GetSection($"Config:{SchedulingOptions.Name}"));
        public PeriodicWorker(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            var schedulingOptions = serviceProvider
                .GetService<IOptionsMonitor<SchedulingOptions>>();
            if (schedulingOptions != null)
                _scheduling = schedulingOptions.CurrentValue;
            else if (_scheduling is null)
                _scheduling = _configuration
                    .GetSection(SectionName)
                    .Get<SchedulingOptions>();
        }

        protected virtual T ParseConfiguration<T>(IConfiguration configuration, string sectionName = "") where T : class
        {
            T emailOptions;
            try
            {
                emailOptions = configuration.GetRequiredSection(sectionName).Get<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse all the required service configuration values.");
                throw;
            }
            return (T)Convert.ChangeType(emailOptions, typeof(T));
        }

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
            if (_startDelay > TimeSpan.Zero && !_appToken.IsCancellationRequested)
            {
                string startTime = _scheduledStart.ToString(DateTimeFormat); // fixes the culture format
                _logger.LogTrace("{0} is waiting {1:c} for the start time of {2}.", _name, _startDelay, startTime);
                _timer = new PeriodicTimer(_startDelay);
                try
                {
                    await _timer!.WaitForNextTickAsync(_appToken);
                }
                catch (OperationCanceledException) { _logger.LogTrace("PeriodicWorker timer cancelled"); }
            }
            if (!_appToken.IsCancellationRequested)
            {
                await base.ExecuteJobAsync();
                await base.ScheduleNextJobAsync();
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken = default)
        {
            _timer?.Dispose();
            return base.StopAsync(cancellationToken);
        }

        public virtual void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
