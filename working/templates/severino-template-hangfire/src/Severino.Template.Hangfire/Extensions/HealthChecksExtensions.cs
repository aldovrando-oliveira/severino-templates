using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Severino.Template.Hangfire.Extensions
{
    /// <summary>
    /// Extensões para configuração do monitoramente de integradidade da aplicação
    /// </summary>
    public static class HealthChecksExtensions
    {
        /// <summary>
        /// Adiciona os serviços que devem ser validados no healthcheck
        /// </summary>
        /// <param name="services">Contâiner de injeção de dependência</param>
        /// <param name="configuration">Configurações da aplicação</param>
        public static void AddAppHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddMySql(configuration.GetMySqlConnectionString("template"),
                "TemplateDb",
                HealthStatus.Unhealthy,
                new string[] { "database" });
        }

        /// <summary>
        /// Adiciona e configura a url do monitoramento
        /// </summary>
        /// <param name="app">Configuração das aplicação</param>
        public static void UseAppHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/healthchecks", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = WriteResponseHealthCheckAsync
            });
        }

        /// <summary>
        /// Escreve a resposta do HealthCheck
        /// </summary>
        /// <param name="context">Contexto da requisição de resposta</param>
        /// <param name="report">Report com o resultado do HealthCheck</param>
        /// <returns></returns>
        private static async Task WriteResponseHealthCheckAsync(HttpContext context, HealthReport report)
        {
            var response = JsonConvert.SerializeObject(new
            {
                Status = report.Status.ToString(),
                report.TotalDuration,
                entries = report.Entries.Select(item => new
                {
                    Name = item.Key,
                    item.Value.Data,
                    Status = item.Value.Status.ToString(),
                    item.Value.Duration,
                    Description = item.Value.Description ?? item.Value.Exception?.Message,
                    Exception = item.Value.Exception?.StackTrace
                })
            }, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(response);
        }
    }
}