using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Infrastructure.BackgroundJobs;
using PageBoostAI.Infrastructure.Caching;
using PageBoostAI.Infrastructure.ExternalServices;
using PageBoostAI.Infrastructure.ExternalServices.Email;
using PageBoostAI.Infrastructure.Identity;
using PageBoostAI.Infrastructure.Persistence;
using PageBoostAI.Infrastructure.Persistence.Repositories;
using PageBoostAI.Infrastructure.Services;

namespace PageBoostAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = BuildConnectionString(
            configuration["DATABASE_URL"]
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? "Host=postgres;Port=5432;Database=pageboost_db;Username=pageboost;Password=pageboost_dev");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFacebookPageRepository, FacebookPageRepository>();
        services.AddScoped<IContentScheduleRepository, ContentScheduleRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
        services.AddScoped<IUsageMetricsRepository, UsageMetricsRepository>();

        // Identity & current user
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IEncryptionService, EncryptionService>();

        // Email
        services.AddSingleton<IEmailService, EmailService>();

        // External services (registered against Application interfaces)
        var aiProvider = configuration["AI_PROVIDER"] ?? "groq";
        if (aiProvider.Equals("anthropic", StringComparison.OrdinalIgnoreCase))
            services.AddHttpClient<IAnthropicService, AnthropicService>();
        else
            services.AddHttpClient<IAnthropicService, GroqService>();
        services.AddHttpClient<IFacebookGraphService, FacebookGraphService>();
        services.AddHttpClient<IUnsplashService, UnsplashService>();
        services.AddSingleton<IPayFastService, PayFastService>();
        services.AddSingleton<IImageProcessingService, ImageProcessingService>();

        // Background jobs
        services.AddScoped<IPostPublishingJob, PostPublishingJob>();
        services.AddScoped<ITokenRefreshJob, TokenRefreshJob>();

        // Caching (Redis when available, no-op fallback)
        var redisUrl = configuration["REDIS_URL"];
        if (!string.IsNullOrEmpty(redisUrl))
            services.AddSingleton<ICacheService, RedisCacheService>();
        else
            services.AddSingleton<ICacheService, NoOpCacheService>();

        return services;
    }

    // Converts postgresql://user:pass@host:port/db?sslmode=require to Npgsql key=value format
    private static string BuildConnectionString(string connectionString)
    {
        if (!connectionString.StartsWith("postgresql://") && !connectionString.StartsWith("postgres://"))
            return connectionString;

        Uri uri;
        try { uri = new Uri(connectionString); }
        catch (UriFormatException) { return connectionString; }
        var userInfo = uri.UserInfo.Split(':');
        var user = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
        var database = uri.AbsolutePath.TrimStart('/');

        var builder = new System.Text.StringBuilder(
            $"Host={uri.Host};Port={uri.Port};Database={database};Username={user};Password={password}");

        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        if (query["sslmode"] is { } sslMode)
            builder.Append($";Ssl Mode={sslMode}");
        if (query["sslrootcert"] is { } cert)
            builder.Append($";SSL Certificate={cert}");

        return builder.ToString();
    }
}
