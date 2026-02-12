using AspNetCoreRateLimit;
using Hangfire;
using PageBoostAI.Api.Middleware;

namespace PageBoostAI.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "PageBoost AI API v1");
                options.RoutePrefix = "swagger";
            });
        }

        app.UseHttpsRedirection();
        app.UseCors("PageBoostPolicy");
        app.UseIpRateLimiting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HangfireDashboardAuthFilter() }
        });

        app.MapControllers();
        app.MapHealthChecks("/health");

        return app;
    }
}

public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return httpContext.User.Identity?.IsAuthenticated == true;
    }
}
