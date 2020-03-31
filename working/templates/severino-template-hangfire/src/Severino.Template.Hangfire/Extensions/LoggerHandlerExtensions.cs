using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Severino.Template.Hangfire.Infra.Api;

namespace Severino.Template.Hangfire.Extensions
{
    public static class LoggerHandlerExtensions
    {
        public static void AddLoggerHandler(this IServiceCollection services)   
        {
            services.AddTransient<LoggerHandlerMiddleware>();
        }

        public static void UseLoggerHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<LoggerHandlerMiddleware>();
        }
    }
}