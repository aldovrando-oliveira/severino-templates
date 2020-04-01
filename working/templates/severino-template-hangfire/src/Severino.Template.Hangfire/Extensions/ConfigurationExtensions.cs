using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Severino.Template.Hangfire.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Retorna a string de conexão com o banco de dados MySql
        /// </summary>
        /// <param name="configuration">Objeto com as configurações do ambiente</param>
        /// <param name="database">Nome do banco de dados utilizado para buscar as configurações</param>
        /// <returns></returns>
        public static string GetMySqlConnectionString(this IConfiguration configuration, string database)
        {
            MySqlConnectionStringBuilder mySqlConnection = new MySqlConnectionStringBuilder();
            string databaseName = database.ToUpper();
            
            mySqlConnection.Database = configuration.GetValue<string>($"DB_{databaseName}_DATABASE");
            mySqlConnection.UserID = configuration.GetValue<string>($"DB_{databaseName}_USER");
            mySqlConnection.Password = configuration.GetValue<string>($"DB_{databaseName}_PASSWORD");
            mySqlConnection.Server = configuration.GetValue<string>($"DB_{databaseName}_SERVER");
            mySqlConnection.Port = configuration.GetValue<uint>($"DB_{databaseName}_PORT");

            bool isPooling = configuration.GetValue<bool>($"DB_{databaseName}_POOLING");

            if (isPooling)
            {
                mySqlConnection.MaximumPoolSize = configuration.GetValue<uint>($"DB_{databaseName}_POOLING_MAX");
                mySqlConnection.MaximumPoolSize = configuration.GetValue<uint>($"DB_{databaseName}_POOLING_MIN");
                mySqlConnection.Pooling = true;
            }

            return mySqlConnection.ToString();
        }
        
        /// <summary>
        /// Recupera a configuração das variaveis de ambiente
        /// </summary>
        /// <param name="configuration">objeto com as configurações do ambiente</param>
        /// <param name="config">Nome da configuração que está sendo procurada</param>
        /// <param name="required">Se a configuração é obrigatória</param>
        /// <typeparam name="T">Tipo da configuração</typeparam>
        /// <returns></returns>
        public static T GetConfiguration<T>(this IConfiguration configuration, string config, bool required = true)
        {
            var environmentVariable = configuration[config.ToUpper()];

            if (required && string.IsNullOrWhiteSpace(environmentVariable))
            {
                // TODO: criar exceção customizada
                throw new System.Exception($"Environment variable '{config.ToUpper()}' don't parametrized");
            }

            return configuration.GetValue<T>(config.ToUpper());
        }
    }
}