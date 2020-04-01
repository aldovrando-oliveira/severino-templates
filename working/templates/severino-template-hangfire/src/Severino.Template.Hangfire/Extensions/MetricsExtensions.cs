using System;
using App.Metrics;
using App.Metrics.Formatters.InfluxDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Severino.Template.Hangfire.Extensions
{
    public static class MetricsExtensions
    {
        private static bool _hasMetrics = true;
        
        public static void UseAppMetrics(this IApplicationBuilder app)
        {
            if (!_hasMetrics)
                return;
            
            app.UseMetricsAllEndpoints();
            app.UseMetricsAllMiddleware();
        }

        public static void AddAppMetrics(this IServiceCollection services, IConfiguration configuration)
        {
            string influxDbUrl = configuration["INFLUXDB_URL"];

            if (string.IsNullOrEmpty(influxDbUrl))
            {
                _hasMetrics = false;
                return;
            }

            var metrics = AppMetrics.CreateDefaultBuilder();

            metrics.Configuration.Configure(
                options =>
                {
                    string context = configuration["METRICS_CONTEXT"];

                    if (!string.IsNullOrWhiteSpace(context))
                        options.DefaultContextLabel = context;
                    
                    options.Enabled = true;
                    options.ReportingEnabled = true;
                }
            );

            if (!string.IsNullOrWhiteSpace(influxDbUrl))
            {
                metrics.Report.ToInfluxDb(
                    options =>
                    {
                        options.InfluxDb.BaseUri = new Uri(influxDbUrl);
                        options.InfluxDb.Database = configuration["INFLUXDB_DATABASE"];
                        options.InfluxDb.UserName = configuration["INFLUXDB_USERNAME"];
                        options.InfluxDb.Password = configuration["INFLUXDB_PASSWORD"];
                        options.InfluxDb.CreateDataBaseIfNotExists = true;
                        options.InfluxDb.CreateDatabaseRetentionPolicy =
                            new App.Metrics.Reporting.InfluxDB.RetentionPolicyOptions
                            {
                                Duration = TimeSpan.FromDays(3),
                                Name = "defalult"
                            };
                        options.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
                        options.HttpPolicy.FailuresBeforeBackoff = 5;
                        options.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
                        options.MetricsOutputFormatter = new MetricsInfluxDbLineProtocolOutputFormatter();
                        options.FlushInterval =
                            TimeSpan.FromSeconds(Convert.ToInt32(configuration["INFLUXDB_FLUSH_INTERVAL"] ?? "30"));
                    }
                );
            }

            services.AddMetrics(metrics.Build());
            services.AddMetricsEndpoints(configuration, options =>
            {

            });
            services.AddMetricsTrackingMiddleware(configuration);
            services.AddMetricsReportingHostedService((events, args) =>
            {
                Console.WriteLine("Erro: {0}", args.Exception);
            });
        }
    }
}