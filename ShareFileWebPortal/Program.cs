using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
//using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using Common.Helpers;
using Common.Helpers.BaseClass;
using Data.ShareFile;
using Data.WebPortal;
using ShareFileWebPortal.Data;
using ShareFileWebPortal.Profiles;
using ShareFileWebPortal.Services;
using ShareFileWebPortal.Models;

Log.Logger = new LoggerConfiguration()
    .BuildBasicSerilogConfiguration()
    .CreateBootstrapLogger();
Log.Debug("Bootstrap logger loaded");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, configuration) =>
    configuration//.WriteTo.Debug().WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));//,writeToProviders: true
var services = builder.Services;
var config = builder.Configuration;
services.Configure<AppsettingsOptions>(config);
//services.SetLoggerFactory();
Log.Debug("Serilog service added");

// Add services to the container.
services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
// By default, all incoming requests will be authorized according to the default policy.
services.AddAuthorization(options => { options.FallbackPolicy = options.DefaultPolicy; });

services.AddRazorPages();
services.AddServerSideBlazor();
services.AddSingleton<ShareFileApiService>();
//services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
services.AddDistributedMemoryCache();
services.AddLocalization();
services.AddDbContext<ShareFileRepository>();
services.AddAutoMapper(cfg => cfg.AddProfile<ShareFileProfile>());//typeof(Program)//(AppDomain.CurrentDomain.GetAssemblies());
//services.AddSerilogService(builder.Configuration);
//Log.Debug("Serilog service added");

services.Configure<RequestLocalizationOptions>(options =>
{
  var supportedCultures = new[] { new System.Globalization.CultureInfo("en-001") };
  options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-001");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});


var app = builder.Build();
app.Services.SetLoggerFactory();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseSerilogRequestLogging();

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

if (!app.Environment.IsDevelopment() ||
  config.GetSection("Config:EnableAuthentication").Get<bool>())
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

Log.Debug("Starting web host");
try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}