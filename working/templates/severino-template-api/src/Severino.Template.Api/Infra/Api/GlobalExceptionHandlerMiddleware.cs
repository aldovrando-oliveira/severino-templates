using System;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Severino.Template.Api.Exceptions;
using Severino.Template.Api.Infra.Api.ViewMModels;
using Severino.Template.Api.ViewModels;

namespace Severino.Template.Api.Infra.Api
{
    public class GlobalExceptionHandlerMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode statusCode = ex switch
            {
                EntityNotFoundException entityNotFoundException => HttpStatusCode.NotFound,
                ConflictException conflictException => HttpStatusCode.Conflict,
                AuthenticationException authenticationException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            string errorCode = ex.Data["ErrorCode"]?.ToString() ?? ((int) statusCode).ToString();
            
            string userMessage = ex.Data["UserMessage"]?.ToString() ??
                                 (statusCode == HttpStatusCode.InternalServerError
                                     ? $"Erro inesperado ({ex.Message})"
                                     : ex.Message);
            
            string developerMessage = ex.Data["DeveloperMessage"]?.ToString() ??
                                      (statusCode == HttpStatusCode.InternalServerError
                                          ? $"Erro inesperado ({ex.Message})"
                                          : ex.Message);
            
            string moreInfo = ex.Data["MoreInfo"]?.ToString() ?? ex.StackTrace;

            ErrorViewModel errorResponse = new ErrorViewModel
            {
                UserMessage = userMessage,
                DeveloperMessage = developerMessage,
                MoreInfo = moreInfo,
                ErrorCode = errorCode
            };

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) statusCode;

            var response = new BaseViewModel
            {
                Errors = new[] {errorResponse}
            };

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var responseMessage = JsonConvert.SerializeObject(response, settings);

            return context.Response.WriteAsync(responseMessage);
        }
    }
}