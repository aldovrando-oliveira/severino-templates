using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Severino.Template.Api.Infra.Api;

namespace Severino.Template.Api.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        public override BadRequestObjectResult BadRequest(ModelStateDictionary modelState)
        {
            ModelError error = modelState.SelectMany(x => x.Value.Errors).First();

            ErrorViewModel response = new ErrorViewModel
            {
                Status = (int) HttpStatusCode.BadRequest,
                Code = ((int) HttpStatusCode.BadRequest).ToString(),
                Message = error.ErrorMessage,
                Details = error.Exception.ToString()
            };
                    
            return new BadRequestObjectResult(response);
        }
    }
}