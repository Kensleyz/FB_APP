using Hangfire;
using Microsoft.EntityFrameworkCore;
using PageBoostAI.Api.Extensions;
using PageBoostAI.Infrastructure.BackgroundJobs;
using PageBoostAI.Infrastructure.Persistence;
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

    await using (var scope = app.Services.CreateAsyncScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
    }

    app.UseApiPipeline();

    var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate<IPostPublishingJob>(
        "post-publishing",
        job => job.ExecuteAsync(CancellationToken.None),
        Cron.Minutely);
    recurringJobManager.AddOrUpdate<ITokenRefreshJob>(
        "token-refresh",
        job => job.ExecuteAsync(CancellationToken.None),
        "0 */6 * * *");

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
