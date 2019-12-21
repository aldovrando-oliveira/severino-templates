using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Severino.Template.Api.Infra.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Severino.Template.Api.Extensions
{
    /// <summary>
    /// Extensões para configuração da documentação das APIs
    /// </summary>
    public static class DocumentationExtensions
    {
        /// <summary>
        /// Adiciona a documentação no container de injeção de dependência
        /// </summary>
        /// <param name="services">Container de injeção de dependência</param>
        public static void AddDocumentations(this IServiceCollection services, IConfiguration configuration)
        {
            var apiOptions = configuration.GetSection("ApiDocumentation");

            services.Configure<ApiDocumentationOptions>(apiOptions);
            
            services.AddSwaggerGen(config =>
                {
                    config.SwaggerDoc("v1", 
                        new OpenApiInfo 
                        { 
                            Title = apiOptions.GetValue<string>("AppName"), 
                            Description = apiOptions.GetValue<string>("Description"),
                            Version = "v1",
                            Contact = new OpenApiContact
                            {
                                Name = apiOptions.GetValue<string>("Owner"),
                                Url = new System.Uri(apiOptions.GetValue<string>("Url")),
                                Email = apiOptions.GetValue<string>("Email")
                            }
                        });

                    config.DescribeAllParametersInCamelCase();
                    config.CustomSchemaIds(x => x.FullName);
                    config.OrderActionsBy(x => x.GroupName);
                });
        }

        /// <summary>
        /// Adiciona o middleware para exibição da documentação das APIs
        /// </summary>
        /// <param name="app"></param>
        public static void UseDocumentations(this IApplicationBuilder app)
        {
            var apiOptions = app.ApplicationServices.GetService<IOptions<ApiDocumentationOptions>>().Value;
            
            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.DocumentTitle = apiOptions.AppName;
                config.SwaggerEndpoint(apiOptions.DocJson, apiOptions.AppName);
                config.RoutePrefix = apiOptions.DocRoute;
                config.DisplayRequestDuration();
                config.DocExpansion(DocExpansion.None);
                config.EnableValidator();
            });
        }
    }}