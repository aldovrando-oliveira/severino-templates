using System;
using System.Runtime.Serialization;

namespace Severino.Template.Hangfire.Exceptions
{
    /// <summary>
    /// Exceção de conflitos nas entidades
    /// </summary>
    [Serializable]
    public class ConflictException : Exception
    {
        /// <summary>
        /// Cria uma nova instância de <see cref="ConflictException"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private ConflictException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        /// <summary>
        /// Cria uma nova instância de <see cref="ConflictException"/>
        /// </summary>
        /// <param name="message">Mensagem descritiva da exceção</param>
        public ConflictException(string message) : base(message)
        {
        }

    }
}