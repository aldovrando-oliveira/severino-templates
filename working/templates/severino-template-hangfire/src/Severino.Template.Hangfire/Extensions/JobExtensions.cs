using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Storage.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Severino.Template.Hangfire.Infra.Jobs;

namespace Severino.Template.Hangfire.Extensions
{
    public static class JobsExtensions
    {
        /// <summary>
        /// Adiciona a configuração para processamento em background
        /// </summary>
        /// <param name="services">Coleção para configuração da injeção de dependência</param>
        /// <param name="configuration">Configurações do ambiente</param>
        public static void AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new MySqlStorageOptions
            {
                TransactionIsolationLevel = IsolationLevel.ReadCommitted,
                QueuePollInterval = TimeSpan.FromSeconds(3),
                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                PrepareSchemaIfNecessary = true,
                DashboardJobListLimit = 10000,
                TablesPrefix = "worker_"
            };

            var storage = new MySqlStorage(configuration.GetMySqlConnectionString("template"), options);

            services.AddHangfire(options => { options.UseStorage(storage); });

            string queues = configuration["WORKER_QUEUES"]?.ToLower() ?? "default";

            services.AddHangfireServer(options =>
                options.Queues = queues.Split(",").ToArray());
        }

        /// <summary>
        /// Configura a aplicação para ser um processador de eventos em background
        /// </summary>
        /// <param name="app">Builder para configuração da aplicação</param>
        /// <param name="configuration">Configurações do ambiente</param>
        public static void UseBackgroundJobs(this IApplicationBuilder app, IConfiguration configuration)
        {
            string queues = configuration["WORKER_QUEUES"]?.ToLower() ?? "default";

            var options = new BackgroundJobServerOptions
            {
                Queues = queues.Split(",").ToArray()
            };

            app.UseHangfireServer(options);
        }

        /// <summary>
        /// Configura a aplicação para disponiblizar um dashboard dos processamentos em background
        /// </summary>
        /// <param name="app">Builder para configuração da aplicação</param>
        /// <param name="configuration">Configurações do ambiente</param>
        public static void UseJobsDashboard(this IApplicationBuilder app, IConfiguration configuration)
        {
            var dashboardPatch = configuration["WORKER_DASH_PATH"] ?? "";

            string adminUser = configuration.GetConfiguration<string>("DASH_USER");
            string adminPass = configuration.GetConfiguration<string>("DASH_PASSWORD");

            var options = new DashboardOptions
            {
                Authorization = new List<IDashboardAuthorizationFilter>
                {
                    new DashboardAuthFilter(adminUser, adminPass)
                }
            };

            app.UseHangfireDashboard(dashboardPatch, options);
        }
    }
}