using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Helpers.BaseClass
{
    // https://docs.microsoft.com/en-us/dotnet/core/extensions/scoped-service
    public interface IScopedProcessingService
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }

    public abstract class ScopedProcessingBase : IScopedProcessingService
    {
        protected readonly IServiceProvider _serviceProvider;

        public ScopedProcessingBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteScopedProcessAsync(CancellationToken ct = default)
        {
            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var scopedProcessingService = serviceScope.ServiceProvider
                    .GetService<IScopedProcessingService>();
                if (scopedProcessingService != null)
                    await scopedProcessingService.ExecuteAsync(ct);
            }
        }

        public abstract Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
