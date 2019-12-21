using System;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Severino.Template.Api.Exceptions;

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
            string errorCode = null;

            if (ex.Data.Contains("ErrorCode"))
                errorCode = ex.Data["ErrorCode"].ToString();

            ErrorViewModel errorResponse;
            HttpStatusCode statusCode;

            switch (ex)
            {
                case EntityNotFoundException entityNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    errorResponse = new ErrorViewModel
                    {
                        Status = (int) HttpStatusCode.NotFound,
                        Message = entityNotFoundException.Message,
                        Details = entityNotFoundException.StackTrace,
                        Code = errorCode ?? ((int) HttpStatusCode.NotFound).ToString()
                    };
                    break;

                case ConflictException conflictException:
                    statusCode = HttpStatusCode.Conflict;
                    errorResponse = new ErrorViewModel
                    {
                        Status = (int) HttpStatusCode.Conflict,
                        Message = conflictException.Message,
                        Code = errorCode ?? ((int) HttpStatusCode.Conflict).ToString()
                    };
                    break;

                case AuthenticationException authenticationException:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorResponse = new ErrorViewModel
                    {
                        Status = (int) HttpStatusCode.Unauthorized,
                        Message = authenticationException.Message,
                        Details = authenticationException.StackTrace,
                        Code = errorCode ?? ((int) HttpStatusCode.Unauthorized).ToString()
                    };
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    errorResponse = new ErrorViewModel
                    {
                        Status = (int) HttpStatusCode.InternalServerError,
                        Message = $"Unexpected error processing request. ({ex.Message})",
                        Details = ex.StackTrace,
                        Code = errorCode ?? ((int) HttpStatusCode.InternalServerError).ToString()
                    };
                    break;
            }

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) statusCode;

            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
    }
}