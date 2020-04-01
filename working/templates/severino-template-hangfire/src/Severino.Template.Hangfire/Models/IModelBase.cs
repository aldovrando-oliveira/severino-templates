using System;
namespace Severino.Template.Hangfire.Models
{
    /// <summary>
    /// Contrato base para entidades
    /// </summary>
    public interface IModelBase
    {
        /// <summary>
        /// Código da entidade
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Data e hora da criação do registro
        /// </summary>
        DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Data e hora da última atualização do registro
        /// </summary>
        DateTimeOffset UpdatedAt { get; set; }
    }
}