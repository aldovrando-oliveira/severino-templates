using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Severino.Template.Api.Repositories;

namespace Severino.Template.Api.Extensions
{
    public static class RepositoriesExtensions
    {
        public static void AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<AppDbContext>(options => 
                options
                    .UseMySql(configuration.GetMySqlConnectionString("template"))
                    .EnableDetailedErrors());
            
            services.AddUnitOfWork<AppDbContext>();
        }
    }
}