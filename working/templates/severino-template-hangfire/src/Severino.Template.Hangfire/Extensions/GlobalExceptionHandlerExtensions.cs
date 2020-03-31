using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Severino.Template.Hangfire.Infra.Api;

namespace Severino.Template.Hangfire.Extensions
{
    public static class GlobalExceptionHandlerExtensions
    {
        public static void AddGlobalExceptionHandler(this IServiceCollection services)
        {
            services.AddTransient<GlobalExceptionHandlerMiddleware>();
        }

        public static void UseGlobalExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}