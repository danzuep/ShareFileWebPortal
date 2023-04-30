using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Common.Helpers.Models;
using Common.Helpers.BaseClass;

namespace Common.Services.Extensions.DependencyInjection;

public static class SchedulingExtensions
{
    public static IServiceCollection AddHostedScheduledService<THostedService, TScopedService>(
        this IServiceCollection services,
        IConfiguration namedConfigurationSection)
        where TScopedService : class, IScopedProcessingService
    {
        services.Configure<SchedulingOptions>(namedConfigurationSection);
        services.AddHostedService<ScheduledService<THostedService>>();
        services.AddScoped<IScopedProcessingService, TScopedService>();
        return services;
    }

    public static IServiceCollection AddHostedScheduledService<THostedService, TScopedService>(
        this IServiceCollection services,
        Action<SchedulingOptions> configureOptions)
        where TScopedService : class, IScopedProcessingService
    {
        services.Configure(configureOptions);
        return services.AddHostedScheduledService<THostedService, TScopedService>();
    }

    public static IServiceCollection AddHostedScheduledService<THostedService, TScopedService>(
        this IServiceCollection services,
        IConfiguration configuration,
        string configurationSectionName)
        where TScopedService : class, IScopedProcessingService
    {
        services.AddOptions<SchedulingOptions>()
            .Bind(configuration.GetSection(configurationSectionName))
            .Configure(options =>
            { // default option values
                options.Schedule = "0 5 ? * *";
                options.Timeout = TimeSpan.FromHours(23);
                options.RestartDelay = TimeSpan.FromHours(1);
                options.AutoStart = true;
            });
        return services.AddHostedScheduledService<THostedService, TScopedService>();
    }

    public static IServiceCollection AddHostedScheduledService<THostedService, TScopedService>(
        this IServiceCollection services,
        IConfiguration configuration,
        SchedulingOptions userOptions)
        where TScopedService : class, IScopedProcessingService
    {
        services.AddOptions<SchedulingOptions>()
            .Configure(options =>
            {
                options.Schedule = userOptions.Schedule;
                options.Timeout = userOptions.Timeout;
                options.RestartDelay = userOptions.RestartDelay;
                options.AutoStart = userOptions.AutoStart;
            });
        return services.AddHostedScheduledService<THostedService, TScopedService>();
    }

    public static IServiceCollection AddHostedScheduledService<THostedService, TScopedService>(
        this IServiceCollection services) where TScopedService : class, IScopedProcessingService
    {
        services.AddHostedService<ScheduledService<THostedService>>();
        services.AddScoped<IScopedProcessingService, TScopedService>();
        return services;
    }
}
