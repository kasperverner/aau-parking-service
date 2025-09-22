using AAUParkingService.Jobs;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.MemoryStorage;
using Polly;
using Serilog;
using Serilog.Core;

namespace AAUParkingService.Helpers;

public static class ConfigurationHelper
{
    private const int RetryCount = 3;
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        Logger logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
    }

    public static void ConfigureHangfire(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(config => config.UseMemoryStorage());
        builder.Services.AddHangfireServer();

        builder.Services.AddTransient<ParkingJob>();

        builder.Services.AddHttpClient<ParkingJob>(options =>
                options.BaseAddress = new Uri("https://api.mobile-parking.eu/"))
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());
    }

    static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: RetryCount,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, delay, retryCount, context) =>
                {
                    Log.Warning("Retry {RetryCount} after {Delay}ms due to {Reason}",
                        retryCount, delay.TotalMilliseconds, outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString());
                });
    }

    static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: RetryCount,
                durationOfBreak: TimeSpan.FromSeconds(30));
    }

    public static void UseDashboard(this WebApplication app)
    {
        var username = EnvironmentHelper.GetEnvironmentVariable("HANGFIRE_DASHBOARD_USERNAME");
        var password = EnvironmentHelper.GetEnvironmentVariable("HANGFIRE_DASHBOARD_PASSWORD");

        app.UseHangfireDashboard("/dashboard", new DashboardOptions
        {
            Authorization =
            [
                new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                {
                    RequireSsl = false,
                    SslRedirect = false,
                    LoginCaseSensitive = true,
                    Users =
                    [
                        new BasicAuthAuthorizationUser
                        {
                            Login = username,
                            PasswordClear = password
                        }
                    ]
                })
            ]
        });
    }

    public static void UseJobScheduling(this WebApplication app)
    {
        var schedule = EnvironmentHelper.GetEnvironmentVariableCronExpression("PARKING_JOB_SCHEDULE");

        RecurringJob.AddOrUpdate<ParkingJob>(nameof(ParkingJob), job => job.Execute(), schedule);
    }
}