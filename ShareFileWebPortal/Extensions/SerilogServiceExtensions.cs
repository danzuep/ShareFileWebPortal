using Serilog;
using Serilog.Events;
using Serilog.Templates;

public static class SerilogServiceExtensions
{
    public static void AddSerilog(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, loggerConfiguration) =>
            loggerConfiguration.WriteTo.Debug().WriteTo.Console()
                .ReadFrom.Configuration(context.Configuration));
    }

    /// <summary>
    /// Register the Serilog service for custom logging.
    /// </summary>
    public static IServiceCollection AddSerilogService(
        this IServiceCollection services, LoggerConfiguration loggerConfiguration)
    {
        Log.Logger = loggerConfiguration.CreateLogger();
        AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
        return services.AddSingleton(Log.Logger);
    }

    public static IServiceCollection AddSerilogService(
        this IServiceCollection services, HostBuilderContext hostContext)
    {
        return services.AddSerilogService(hostContext.Configuration);
    }

    public static IServiceCollection AddSerilogService(
        this IServiceCollection services, IConfiguration configuration)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration);
        return services.AddSerilogService(loggerConfiguration);
    }

    public static IServiceCollection AddSerilogService(
        this IServiceCollection services, LoggerConfiguration loggerConfiguration, IConfiguration configuration)
    {
        loggerConfiguration = loggerConfiguration.ReadFrom.Configuration(configuration);
        return services.AddSerilogService(loggerConfiguration);
    }

    public static IServiceCollection AddSerilogService(
        this IServiceCollection services)
    {
        var configuration = new LoggerConfiguration().BuildBasicSerilogConfiguration();
        return services.AddSerilogService(configuration);
    }

    public static LoggerConfiguration BuildBasicSerilogConfiguration(
        this LoggerConfiguration loggerConfiguration)
    {
        return loggerConfiguration
            //.MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Debug(
                outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message}{NewLine}{Exception}")
            .WriteTo.Console(new ExpressionTemplate(
                template: "[{@t:HH:mm:ss.fff} {@l:u3}{#if SourceContext is not null} ({SourceContext}){#end}] {@m}\n{@x}"),
                restrictedToMinimumLevel: LogEventLevel.Information)
            .WriteTo.File(@"Logs\log-.txt",
                outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level,-4:u3}] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true);
    }
}