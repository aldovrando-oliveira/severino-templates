using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Severino.Template.Hangfire.Extensions
{
    /// <summary>
    /// Extensão para manipulação das evoluções do banco de dados
    /// </summary>
    public static class MigrationsExtensions
    {
        /// <summary>
        /// Executa as evoluções de banco de dados pendentes
        /// </summary>
        /// <param name="app">Objeto com as configurações da aplicação</param>
        public static void UseMigrations<TContext>(this IApplicationBuilder app) where TContext : DbContext
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var logger = scope.ServiceProvider.GetService<ILoggerFactory>().CreateLogger(nameof(UseMigrations));
                var dbContext = scope.ServiceProvider.GetService<TContext>();
                IEnumerable<string> pendingMigrations;

                try
                {
                    logger.LogInformation("Recuperando evoluções de banco pendentes");
                    pendingMigrations = dbContext.Database.GetPendingMigrations();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro inesperado ao consultar evoluções de banco pendentes: {0}", ex.Message);
                    throw;
                }

                if (!pendingMigrations.Any())
                {
                    logger.LogInformation("Não há evoluções de banco pendentes");
                    return;
                }

                try
                {
                    logger.LogInformation("Aplicando {0} evoluções de banco pendentes", pendingMigrations.Count());
                    dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro inesperado ao aplicar evoluções de banco pendentes: {0}", ex.Message);
                    throw;
                }
            }
        }
    }
}