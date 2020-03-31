using System;

namespace Severino.Template.Hangfire.Models
{
    /// <summary>
    /// Classe base para entidades
    /// </summary>
    public class BaseModel : IModelBase
    {
        /// <summary>
        /// Código da entidade
        /// </summary>
        public virtual Guid Id {  get; set; } = Guid.NewGuid();

        /// <summary>
        /// Data e hora da criação do registro
        /// </summary>
        public virtual DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Data e hora da última atualização do registro
        /// </summary>
        public virtual DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}