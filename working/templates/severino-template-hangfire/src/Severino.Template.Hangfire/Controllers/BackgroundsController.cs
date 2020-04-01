using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Severino.Template.Hangfire.Business;

namespace Severino.Template.Hangfire.Controllers
{
    /// <summary>
    /// Serviço que inicia um processamento em segundo plano
    /// </summary>
    [ApiController]
    [Route("api/v1/backgrounds")]
    public class BackgroundsController : CustomControllerBase
    {
        private IBackgroundJobClient _jobClient;

        /// <summary>
        /// Cria uma nova instância de <see cref="BackgroundsController"/>
        /// </summary>
        /// <param name="jobClient"></param>
        public BackgroundsController(IBackgroundJobClient jobClient)
        {
            _jobClient = jobClient;
        }

        /// <summary>
        /// Cria um novo item para ser processado em segundo plano
        /// </summary>
        /// <param name="randon">Código que será utilizado pelo processamento</param>
        /// <returns></returns>
        [HttpPost("{randon}")]
        public IActionResult CreateBackgroundProcessAsync(int randon)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _jobClient.Enqueue<ICallbackBusiness>(x => x.ProcessBackground(randon));

            return Ok();
        }
    }
}