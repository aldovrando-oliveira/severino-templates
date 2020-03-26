using System;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Severino.Template.Api.Exceptions;
using Severino.Template.Api.Infra.Api.ViewMModels;
using Severino.Template.Api.ViewModels;

namespace Severino.Template.Api.Infra.Api
{
    public class GlobalExceptionHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error: {0}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            string errorCode = ex.Data["ErrorCode"]?.ToString();
            string userMessage = ex.Data["UserMessage"]?.ToString() ?? ex.Message;
            string developerMessage = ex.Data["DeveloperMessage"]?.ToString() ?? ex.Message;
            string moreInfo = ex.Data["MoreInfo"]?.ToString() ?? ex.StackTrace;

            ErrorViewModel errorResponse = new ErrorViewModel
            {
                UserMessage = userMessage,
                DeveloperMessage = developerMessage,
                MoreInfo = moreInfo
            };
            
            HttpStatusCode statusCode;

            switch (ex)
            {
                case EntityNotFoundException entityNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    errorResponse.ErrorCode = errorCode ?? ((int) statusCode).ToString();
                    break;

                case ConflictException conflictException:
                    statusCode = HttpStatusCode.Conflict;
                    errorResponse.ErrorCode = errorCode ?? ((int) statusCode).ToString();
                    break;

                case AuthenticationException authenticationException:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorResponse.ErrorCode = errorCode ?? ((int) statusCode).ToString();
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    errorResponse.UserMessage = $"Unexpected error processing request. ({ex.Message})";
                    errorResponse.ErrorCode = errorCode ?? ((int) statusCode).ToString();
                    break;
            }

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