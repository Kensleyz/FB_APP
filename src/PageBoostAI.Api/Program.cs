using Hangfire;
using PageBoostAI.Api.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console());

    builder.Services.AddApiServices(builder.Configuration);

    var app = builder.Build();

    app.UseApiPipeline();

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate(
            "post-publishing",
            () => Console.WriteLine("PostPublishingJob: checking for scheduled posts..."),
            Cron.Minutely);

        recurringJobManager.AddOrUpdate(
            "token-refresh",
            () => Console.WriteLine("TokenRefreshJob: refreshing Facebook tokens..."),
            "0 */6 * * *");
    });

    Log.Information("PageBoost AI API starting on {Environment}", builder.Environment.EnvironmentName);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
