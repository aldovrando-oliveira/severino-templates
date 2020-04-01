using Microsoft.Extensions.DependencyInjection;
using Severino.Template.Hangfire.Business;

namespace Severino.Template.Hangfire.Extensions
{
    public static class BusinessExtensions
    {
        public static void AddBusiness(this IServiceCollection services)
        {
            services.AddTransient<ICustomerBusiness, CustomerBusiness>();
            services.AddTransient<ICallbackBusiness, CallbackBusiness>();
        }
    }
}