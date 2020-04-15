using System;

namespace Severino.Template.Api.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Incluí o código de erro na exceção
        /// </summary>
        /// <param name="exception">Exceção onde será adicionado o código</param>
        /// <param name="errorCode">Código do erro</param>
        /// <returns>Retorna a exceção com o código de erro</returns>
        /// <exception cref="ArgumentNullException">Quando os parâmetros 'exception' ou 'errorCode' não estão preenchidos</exception>
        public static Exception WithErrorCode(this Exception exception, string errorCode)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            if (string.IsNullOrWhiteSpace(errorCode))
                throw new ArgumentNullException(nameof(errorCode));

            if (exception.Data.Contains("ErrorCode"))
                exception.Data["ErrorCode"] = errorCode;
            else
                exception.Data.Add("ErrorCode", errorCode);
            
            return exception;
        }

        /// <summary>
        /// Inclui mensagem tratada para o usuário
        /// </summary>
        /// <param name="exception">Exceção que terá a mensagem incluída</param>
        /// <param name="userMessage">Mensagem para o usuário</param>
        /// <returns>Retorna a exceção com a mensagem para o usuário</returns>
        /// <exception cref="ArgumentNullException">Quando os parâmetros 'exception' ou 'userMessage' não estão preenchidos</exception>
        public static Exception WithUserMessage(this Exception exception, string userMessage)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            if (string.IsNullOrWhiteSpace(userMessage))
                throw new ArgumentNullException(nameof(userMessage));

            if (exception.Data.Contains("UserMessage"))
                exception.Data["UserMessage"] = userMessage;
            else
                exception.Data.Add("UserMessage", userMessage);
            
            return exception;
        }
        
        /// <summary>
        /// Inclui mensagem tratada para o desenvolvedor
        /// </summary>
        /// <param name="exception">Exceção que terá a mensagem incluída</param>
        /// <param name="developerMessage">Mensagem para o desenvolvedor</param>
        /// <returns>Retorna a exceção com a mensagem para o desenvolvedor</returns>
        /// <exception cref="ArgumentNullException">Quando os parâmetros 'exception' ou 'developerMessage' não estão preenchidos</exception>
        public static Exception WithDeveloperMessage(this Exception exception, string developerMessage)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            if (string.IsNullOrWhiteSpace(developerMessage))
                throw new ArgumentNullException(nameof(developerMessage));

            if (exception.Data.Contains("DeveloperMessage"))
                exception.Data["DeveloperMessage"] = developerMessage;
            else
                exception.Data.Add("DeveloperMessage", developerMessage);
            
            return exception;
        }
        
        /// <summary>
        /// Inclui informações adicionais à exceção
        /// </summary>
        /// <param name="exception">Exceção que será complementada</param>
        /// <param name="moreInfo">Informações adicionais da exceção</param>
        /// <returns>Retorna a exceção com as informações adicionais incluídas</returns>
        /// <exception cref="ArgumentNullException">Quando os parâmetros 'exception' ou 'moreInfo' não estão preenchidos</exception>
        public static Exception WithMoreInfo(this Exception exception, string moreInfo)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            if (string.IsNullOrWhiteSpace(moreInfo))
                throw new ArgumentNullException(nameof(moreInfo));

            if (exception.Data.Contains("MoreInfo"))
                exception.Data["MoreInfo"] = moreInfo;
            else
                exception.Data.Add("MoreInfo", moreInfo);
            
            return exception;
        }
        
    }
}