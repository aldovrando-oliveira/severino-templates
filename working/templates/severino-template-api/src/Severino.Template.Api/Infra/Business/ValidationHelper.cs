using System;

namespace Severino.Template.Api.Infra.Business
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Valida se o parâmetro informado é valido
        /// </summary>
        /// <param name="guid">Valor Guid que deve ser validado</param>
        /// <param name="name">Nome do parâmetro</param>
        /// <param name="acceptEmpty">Indica se deve ser considerado o valor Guid.Empty</param>
        public static void ThrowIfGuidInvalid(Guid guid, string name, bool acceptEmpty = false)
        {
            if (Guid.Empty == guid && !acceptEmpty)
                throw new ArgumentException("Parameter Invalid", name);
        }

        /// <summary>
        /// Valida se o parâmetro informado não é nulo
        /// </summary>
        /// <param name="parameter">Parâmetro que será validado</param>
        /// <param name="name">Nome do parâmetro</param>
        public static void ThrowIfParameterNull(object parameter, string name)
        {
            if (parameter == null)
                throw new ArgumentNullException(name);
        }

        /// <summary>
        /// Valida se o parâmetro informado não é nulo
        /// </summary>
        /// <param name="parameter">Parâmetro que será validado</param>
        /// <param name="name">Nome do parâmetro</param>
        public static void ThrowIfParameterNull(string parameter, string name)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                throw new ArgumentNullException(name);
        }
    }
}