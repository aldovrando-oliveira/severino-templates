using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Severino.Template.Hangfire.Business
{
    public interface ICallbackBusiness : IDisposable
    {
        /// <summary>
        /// Processo que será executado em background sob demanda
        /// </summary>
        /// <param name="randon">Número randômico</param>
        /// <returns></returns>
        Task ProcessBackground(int randon);
    }

    public sealed class CallbackBusiness : ICallbackBusiness
    {
        private ILogger<CallbackBusiness> _logger;

        public CallbackBusiness(ILogger<CallbackBusiness> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Processo que será executado em background sob demanda
        /// </summary>
        /// <param name="randon">Número randômico</param>
        /// <returns></returns>
        public Task ProcessBackground(int randon)
        {
            _logger.LogInformation("{0}: Execução em background com o código '{1}'", DateTime.Now, randon);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger = null;
        }
    }
}