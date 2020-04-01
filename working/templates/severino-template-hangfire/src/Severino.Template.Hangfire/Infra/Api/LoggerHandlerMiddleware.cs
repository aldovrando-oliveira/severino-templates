using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Newtonsoft.Json;
using Severino.Template.Hangfire.Infra.Api.Logger.Dto;

namespace Severino.Template.Hangfire.Infra.Api
{
    public class LoggerHandlerMiddleware : IMiddleware
    {
        private static string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private static string ProjectName => Assembly.GetExecutingAssembly().GetName().Name;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly ILogger<LoggerHandlerMiddleware> _logger;

        public LoggerHandlerMiddleware(ILogger<LoggerHandlerMiddleware> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            HttpLog httpLog;

            httpLog = await LogRequest(context);
            httpLog = await LogResponse(context, next, httpLog);


            if (!context.Request.Path.Value.StartsWith("/api"))
                return;

            var log = new ApiLog
            {
                App = ProjectName,
                Team = "checkout-lojas",
                Kind = "server",
                Version = CurrentVersion,
                Level = "INFO"
            };

            log.Http = httpLog;

            string logMessage = JsonConvert.SerializeObject(log, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            _logger.LogInformation(logMessage);


        }

        private async Task<HttpLog> LogRequest(HttpContext context)
        {
            string requestBody;

            context.Request.EnableBuffering();

            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);

            requestBody = ReadStreamInChunks(requestStream);

            context.Request.Body.Position = 0;

            var logHttp = new HttpLog();
            logHttp.RequestMethod = context.Request.Method;
            logHttp.Url = context.Request.Path;
            logHttp.Path = context.Request.Path;
            logHttp.Protocol = context.Request.Scheme;
            logHttp.RequestHeader = JsonConvert.SerializeObject(context.Request.Headers);
            logHttp.RequestBody = requestBody;

            return logHttp;

        }

        private async Task<HttpLog> LogResponse(HttpContext context, RequestDelegate next, HttpLog log)
        {
            var originalBodyStream = context.Response.Body;

            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            try
            {
                await next(context);
            }
            finally
            {
                watch.Stop();

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                await responseBody.CopyToAsync(originalBodyStream);

                log.LatencySeconds = watch.ElapsedMilliseconds / 1000D;
                log.ResponseBody = responseText;
                log.ResponseHeader = JsonConvert.SerializeObject(context.Response.Headers);
                log.StatusCode = context.Response.StatusCode;
            }

            return log;
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);

            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk,
                                                   0,
                                                   readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }
    }
}