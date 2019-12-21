using System;
using System.Runtime.Serialization;

namespace Severino.Template.Api.Exceptions
{
    /// <summary>
    /// Exceção quando uma entidade não é encontrada
    /// </summary>
    [Serializable]
    public sealed class EntityNotFoundException : Exception
    {
        /// <summary>
        /// Retorna uma nova instância de <see cref="EntityNotFoundException"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private EntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Entidade que não foi encontrada
        /// </summary>
        public string Entity { get; }

        /// <summary>
        /// Retorna uma nova instância de <see cref="EntityNotFoundException"/>
        /// </summary>
        /// <param name="entity">Nome da entidade que não foi encontrada</param>
        public EntityNotFoundException(string entity)
            : base($"{entity} not found")
        {
            Entity = entity;
        }

        /// <summary>
        /// Retorna uma nova instância de <see cref="EntityNotFoundException"/>
        /// </summary>
        /// <param name="entity">Nome da entidade que não foi encontrada</param>
        /// <param name="message">Mensagem descritiva do erro</param>
        public EntityNotFoundException(string entity, string message)
            : base(message)
        {
            Entity = entity;
        }
    }
}