using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Severino.Template.Api.Infra.Api.ViewMModels;
using Severino.Template.Api.ViewModels;

namespace Severino.Template.Api.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        public override BadRequestObjectResult BadRequest(ModelStateDictionary modelState)
        {
            var response = new BaseViewModel();

            List<ErrorViewModel> errors = new List<ErrorViewModel>();

            foreach (var item in modelState.SelectMany(x => x.Value.Errors))
            {
                errors.Add(new ErrorViewModel
                {
                    ErrorCode = ((int) HttpStatusCode.BadRequest).ToString(),
                    UserMessage = item.ErrorMessage,
                    DeveloperMessage = item.ErrorMessage,
                    MoreInfo = item.Exception.ToString()
                });
            }

            response.Errors = errors.ToArray();

            return new BadRequestObjectResult(response);
        }
    }
}