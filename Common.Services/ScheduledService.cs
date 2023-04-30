using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Common.Helpers.Models;
using Common.Helpers.BaseClass;

namespace Common.Services
{
    public class ScheduledService<T> : PeriodicWorker<T>
    {
        public static string ServiceName { get; set; } = String.Empty;
        public string ConfigSectionName { get; set; } = $"Config:{SchedulingOptions.Name}";

        public ScheduledService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            if (_scheduling is null)
                _scheduling = _configuration.GetSection(ConfigSectionName).Get<SchedulingOptions>();
        }

        private async Task ExecuteScopedProcessAsync(CancellationToken ct = default)
        {
            using var serviceScope = _serviceProvider.CreateScope();
            var scopedProcessingService = serviceScope.ServiceProvider.GetService<IScopedProcessingService>();
            if (scopedProcessingService == null)
                _logger.LogWarning("{0} not found.", nameof(IScopedProcessingService));
            else
                await scopedProcessingService.ExecuteAsync(ct);
        }

        public override async Task ExecuteAsync(CancellationToken ct = default)
        {
            try
            {
                await ExecuteScopedProcessAsync(ct);
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                throw;
                //string message = string.Format("{0} {1} {2}: {3}.", ServiceName, _name, ex.GetType().Name, ex.Message);
                //using var emailClient = _serviceProvider.GetService<EmailClient>();
                //if (emailClient != null)
                //    await emailClient.EmailStackTrace<T>(ex, message, LogLevel.Critical);
                //else
                //    _logger.LogCritical(ex, message);
                //_logger.LogInformation("{0} is waiting {1:c} for the restart time of {2:s}.",
                //    _name, _restartDelay, DateTime.Now.Add(_restartDelay));
                //await Task.Delay(_restartDelay, _ct);
                //await ExecuteAsync(ct);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
