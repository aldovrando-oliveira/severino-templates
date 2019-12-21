using System;

namespace Severino.Template.Api.ViewModels.Customers
{
    /// <summary>
    /// View Model com os dados de clientes
    /// </summary>
    public class CustomerViewModel
    {
        /// <summary>
        /// Código do cliente
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome do cliente
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Data e hora da criação do registro
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Data e hora da última atualização do registro
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }
    }
}