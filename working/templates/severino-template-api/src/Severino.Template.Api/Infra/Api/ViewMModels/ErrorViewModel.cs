namespace Severino.Template.Api.Infra.Api.ViewMModels
{
    /// <summary>
    /// View Model que representa o erro
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Mensagem de erro tratada para o usuário
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// Mensagem de erro com informações para o desenvoledor
        /// </summary>
        public string DeveloperMessage { get; set; }
        
        /// <summary>
        /// Código de identificação do erro
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Informações complementares do erro
        /// </summary>
        public string MoreInfo { get; set; }
    }
}