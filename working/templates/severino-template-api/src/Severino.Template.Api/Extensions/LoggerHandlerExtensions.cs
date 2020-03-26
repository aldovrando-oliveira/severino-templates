using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Severino.Template.Api.Infra.Api;

namespace Severino.Template.Api.Extensions
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