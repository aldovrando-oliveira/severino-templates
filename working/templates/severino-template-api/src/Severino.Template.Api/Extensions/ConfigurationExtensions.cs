using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Severino.Template.Api.Extensions
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
    }
}