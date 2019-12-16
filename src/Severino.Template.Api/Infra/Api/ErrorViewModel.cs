namespace Severino.Template.Api.Infra.Api
{
    /// <summary>
    /// View Model que representa o erro
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// HTTP Status de erro retorno da requisição 
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Mensagem descritiva do erro
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Código de identificação do erro
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Informações complementares do erro
        /// </summary>
        public string Details { get; set; }
    }
}