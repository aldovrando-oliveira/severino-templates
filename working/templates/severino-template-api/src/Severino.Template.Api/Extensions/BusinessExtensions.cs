using Microsoft.Extensions.DependencyInjection;
using Severino.Template.Api.Business;

namespace Severino.Template.Api.Extensions
{
    public static class BusinessExtensions
    {
        public static void AddBusiness(this IServiceCollection services)
        {
            services.AddTransient<CustomerBusiness>();
        }
    }
}