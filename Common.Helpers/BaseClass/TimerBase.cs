using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Common.Helpers.Models;

namespace Common.Helpers.BaseClass
{
    // https://docs.microsoft.com/en-us/dotnet/core/extensions/timer-service
    public interface ITimedService<T>
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }

    public abstract class TimerBase<T> : IHostedService, ITimedService<T>
    {
        public const string DateTimeFormatLong = "yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff' 'K"; //:O has fffffffK
        public const string DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss"; //:s has 'T'
        public const string DateOnlyFormat = "yyyy'-'MM'-'dd";
        public const string TimeOnlyFormatLong = "HH':'mm':'ss'.'fff";
        public const string TimeOnlyFormat = "HH':'mm':'ss";

        protected readonly string _name;
        protected readonly ILogger _logger;
        protected readonly IConfiguration _configuration;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IServiceScope _serviceScope;
        protected readonly IServiceProvider _scopedProvider;
        protected readonly IHostApplicationLifetime _appLifetime;
        protected readonly Task _completedTask = Task.CompletedTask;
        protected readonly CancellationToken _appToken;
        protected IOptions<SchedulingOptions> _schedulingOptions;
        protected CancellationTokenSource _cts;
        protected SchedulingOptions _scheduling;
        protected string _schedule;
        private TimeSpan _timeout;
        protected TimeSpan _restartDelay;
        protected DateTime _scheduledStart;
        private DateTime _scheduledFinish;
        protected TimeSpan _startDelay;
        protected TimeSpan _finishDelay;
        private int _executionCount = 0;

        public TimerBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _name = typeof(T)?.Name ?? nameof(TimerBase<T>);
            //serviceProvider.SetLoggerFactory();
            //_logger = LogProvider.GetLogger(_name);
            _logger = serviceProvider.GetRequiredService<ILogger<TimerBase<T>>>();
            _configuration = serviceProvider.GetRequiredService<IConfiguration>();
            _appLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();
            _appToken = _appLifetime.ApplicationStopping;
            _scopedProvider = serviceProvider.CreateScope().ServiceProvider;
            _schedulingOptions = _scopedProvider.GetService<IOptions<SchedulingOptions>>();
        }

        protected abstract void GetScheduler(string schedule);
        public virtual async Task StartAsync(CancellationToken cancellationToken = default)
        {
            bool readyToRun = await ScheduleFirstJob();

            if (!readyToRun && !_appToken.IsCancellationRequested)
                _appLifetime.StopApplication();
        }

        protected virtual void UpdateScheduling()
        {
            if (_schedulingOptions != null)
            {
                using (var scope = _scopedProvider.CreateScope())
                {
                    _schedulingOptions = scope.ServiceProvider
                        .GetService<IOptionsSnapshot<SchedulingOptions>>();
                    if (_schedulingOptions != null)
                        _scheduling = _schedulingOptions.Value;
                }
            }
        }

        private void ValidateScheduling()
        {
            UpdateScheduling();

            if (_scheduling is SchedulingOptions cronScheduling)
            {
                _schedule = cronScheduling.Schedule;
                _timeout = cronScheduling.Timeout;
                _restartDelay = cronScheduling.RestartDelay;
            }

            var minRestartDelay = TimeSpan.FromSeconds(1);
            if (_restartDelay < minRestartDelay)
            {
                _logger.LogWarning("{0} restart delay ({1:c}) cannot be less than {2:c}.", _name, _timeout, minRestartDelay);
                _restartDelay = minRestartDelay;
            }

            if (_timeout < TimeSpan.Zero)
            {
                _logger.LogWarning("{0} run time ({1:c}) cannot be negative.", _name, _timeout);
                _timeout = TimeSpan.Zero;
            }
        }

        private async Task<bool> ScheduleFirstJob()
        {
            bool readyToRun = ScheduleJob();

            var dateTimeNow = DateTime.Now;
            if (_scheduledStart > dateTimeNow)
            {
                var relativeStart = _scheduledStart.GetRelativeTime();
                if (_scheduling.AutoStart || dateTimeNow.GetRelativeTime() > relativeStart)
                {
                    _scheduledStart = relativeStart.GetAbsoluteTime();
                    _scheduledFinish = _scheduledStart.Add(_timeout);
                    _finishDelay = _scheduledFinish.GetDelayFromNow();

                    if (_scheduling.AutoStart)
                    {
                        _scheduledStart = dateTimeNow;
                        _startDelay = TimeSpan.Zero;
                        _logger.LogTrace("Updated scheduled finish time ({0:s}) and finish delay ({1:c}), starting now.", _scheduledFinish, _finishDelay);
                    }
                    else
                    {
                        _logger.LogTrace("Updated scheduled start ({0:s}) and finish ({1:s}) time.", _scheduledStart, _scheduledFinish);
                        _startDelay = _scheduledStart.GetDelayFromNow();
                        _logger.LogTrace("Updated start ({0:c}) and finish ({1:c}) delay.", _startDelay, _finishDelay);
                    }
                }
            }

            if (readyToRun)
            {
                readyToRun &= CheckIsReadyToRun();
                await StartTimerAsync();
            }

            return readyToRun;
        }

        protected abstract DateTimeOffset? GetScheduledStart();
        protected virtual bool ScheduleJob()
        {
            //_logger.LogTrace("{0} is scheduling the next job.", _name);
            ValidateScheduling();
            GetScheduler(_schedule);
            var scheduledStart = GetScheduledStart();
            if (scheduledStart != null)
            {
                _scheduledStart = scheduledStart.Value.DateTime;
                _scheduledFinish = _scheduledStart.Add(_timeout);
                _startDelay = _scheduledStart.GetDelayFromNow();
                _finishDelay = _timeout;
                //_logger.LogTrace($"{_name} Cron scheduled start time = {_scheduledStart}, run time = {_cronRunTime:c}, so scheduled finish time = {_scheduledFinish}.");
            }
            else
            {
                _logger.LogWarning("{0} start and finish times have not been set, check the Cron expression.", _name);
                _startDelay = Timeout.InfiniteTimeSpan;
                _finishDelay = TimeSpan.Zero;
            }
            return CheckIsReadyToRun();
        }

        private bool CheckIsReadyToRun()
        {
            bool readyToRun = false;
            if (_appToken.IsCancellationRequested)
                _logger.LogTrace("{0} has been cancelled.", _name);
            else if (_startDelay < TimeSpan.Zero)
                _logger.LogWarning("{0} start delay ({1}) cannot be negative.", _name, _startDelay);
            else if (_finishDelay < TimeSpan.Zero)
                _logger.LogWarning("{0} finish delay ({1}) cannot be negative.", _name, _finishDelay);
            else
                readyToRun = true;
            return readyToRun;
        }

        protected abstract Task StartTimerAsync();

        protected virtual async Task ScheduleNextJobAsync()
        {
            if (ScheduleJob())
                await StartTimerAsync();
        }

        public abstract Task ExecuteAsync(CancellationToken cancellationToken);

        protected virtual async Task ExecuteJobAsync()
        {
            var done = new CancellationTokenSource(_finishDelay);
            _cts = CancellationTokenSource.CreateLinkedTokenSource(_appToken, done.Token);
            var invokeCount = Interlocked.Increment(ref _executionCount);
            //using (_logger.BeginScope($"TimerBase_Started{DateTime.Now:s}"))
            _logger.LogDebug("{0} {1} scheduled to work until {2}.",
                _name, invokeCount, _scheduledFinish.ToString(DateTimeFormat));
            try
            {
                await ExecuteAsync(_cts.Token);
                _logger.LogTrace("{0}_{1} finished at {2}.",
                    _name, invokeCount, DateTime.Now.ToString(DateTimeFormat));
            }
            catch (OperationCanceledException) // includes TaskCanceledException
            {
                _logger.LogTrace("{0}_{1} cancelled at {2}.",
                    _name, invokeCount, DateTime.Now.ToString(DateTimeFormat));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{0} failed to complete, trying again in {1:c}.", _name, _restartDelay);
                string startTime = DateTime.Now.Add(_restartDelay).ToDateTimeString(); // fixes the culture format
                _logger.LogDebug("{0} is waiting for the restart time of {1}.", _name, startTime);
                await Task.Delay(_restartDelay, _cts.Token);
                await ExecuteJobAsync();
            }
        }

        public virtual Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (!_appLifetime.ApplicationStopping.IsCancellationRequested)
                _appLifetime.StopApplication();
            _logger.LogTrace("{0} stopped.", _name);
            return _completedTask;
        }
    }
}
