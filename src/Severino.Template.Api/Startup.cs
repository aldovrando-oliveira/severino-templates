using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Severino.Template.Api.Extensions;
using Severino.Template.Api.Repositories;

namespace Severino.Template.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGlobalExceptionHandler();
            services.AddRepositories(Configuration);
            services.AddBusiness();
            services.AddDocumentations(Configuration);
            services.AddAppHealthChecks(Configuration);
            services.AddAppMetrics(Configuration);
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseGlobalExceptionHandlerMiddleware();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseDocumentations();
            app.UseAuthorization();
            app.UseAppHealthChecks();
            app.UseAppMetrics();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            app.UseMigrations<AppDbContext>();
        }
    }
}
