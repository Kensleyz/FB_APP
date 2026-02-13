using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Infrastructure.BackgroundJobs;
using PageBoostAI.Infrastructure.Caching;
using PageBoostAI.Infrastructure.ExternalServices;
using PageBoostAI.Infrastructure.Identity;
using PageBoostAI.Infrastructure.ExternalServices.Email;
using AppInterfaces = PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Infrastructure.Persistence;
using PageBoostAI.Infrastructure.Persistence.Repositories;

namespace PageBoostAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {        // Database
        var connectionString = configuration["DATABASE_URL"]
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? "Host=postgres;Port=5432;Database=pageboost_db;Username=pageboost;Password=pageboost_dev";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFacebookPageRepository, FacebookPageRepository>();
        services.AddScoped<IContentScheduleRepository, ContentScheduleRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
        services.AddScoped<IUsageMetricsRepository, UsageMetricsRepository>();

        // Identity
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IEncryptionService, EncryptionService>();

        // Email
        services.AddSingleton<AppInterfaces.IEmailService, EmailService>();

        // External Services
        services.AddHttpClient<IAnthropicService, AnthropicService>();
        services.AddHttpClient<IFacebookGraphService, FacebookGraphService>();
        services.AddHttpClient<IUnsplashService, UnsplashService>();
        services.AddSingleton<IPayFastService, PayFastService>();        // Background Jobs (Hangfire)
        // Storage configuration is deferred to API layer after services are built
        services.AddScoped<IPostPublishingJob, PostPublishingJob>();
        services.AddScoped<ITokenRefreshJob, TokenRefreshJob>();

        // Caching (Redis)
        var redisUrl = configuration["REDIS_URL"];
        if (!string.IsNullOrEmpty(redisUrl))
        {
            services.AddSingleton<ICacheService, RedisCacheService>();
        }

        return services;
    }
}
