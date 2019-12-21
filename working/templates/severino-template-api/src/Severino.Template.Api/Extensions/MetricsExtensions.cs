using System;
using App.Metrics;
using App.Metrics.Formatters.InfluxDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Severino.Template.Api.Extensions
{
    public static class MetricsExtensions
    {
        public static void UseAppMetrics(this IApplicationBuilder app)
        {
            app.UseMetricsAllEndpoints();
            app.UseMetricsAllMiddleware();
        }

        public static void AddAppMetrics(this IServiceCollection services, IConfiguration configuration)
        {
            var metrics = AppMetrics.CreateDefaultBuilder();

            metrics.Configuration.Configure(
                options =>
                {
                    options.DefaultContextLabel = configuration["METRICS_CONTEXT"];
                    options.Enabled = true;
                    options.ReportingEnabled = true;
                }
            );

            string influxDbUrl = configuration["INFLUXDB_URL"];

#if DEBUG
            metrics.Report.ToConsole(
                options =>
                {
                    options.FlushInterval = TimeSpan.FromSeconds(20);
                    // options.MetricsOutputFormatter = new MetricsJsonOutputFormatter();
                }
            );
#endif

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