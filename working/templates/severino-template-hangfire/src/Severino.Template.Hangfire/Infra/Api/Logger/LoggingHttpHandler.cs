using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Severino.Template.Hangfire.Infra.Api.Logger.Dto;

namespace Severino.Template.Hangfire.Infra.Api.Logger
{
    public class LoggingHttpHandler : DelegatingHandler
    {
        private ILogger _logger;

        public LoggingHttpHandler(HttpMessageHandler innerHandler, ILoggerFactory loggerFactory)
            : base(innerHandler)
        {
            _logger = loggerFactory.CreateLogger<LoggingHttpHandler>();
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var log = new ApiLog
            {
                App = ProjectName,
                Team = "checkout-lojas",
                Kind = "client",
                Version = CurrentVersion,
                Level = "INFO"
            };

            Stopwatch watch = new Stopwatch();
            watch.Start();
            var response = await base.SendAsync(request, cancellationToken);
            watch.Stop();

            try
            {
                var logHttp = new HttpLog();
                logHttp.LatencySeconds = watch.ElapsedMilliseconds / 1000D;
                logHttp.RequestMethod = request.Method.Method;
                logHttp.StatusCode = (int) response.StatusCode;
                logHttp.Url = request.RequestUri.OriginalString;
                logHttp.Path = request.RequestUri.PathAndQuery;
                logHttp.Protocol = request.RequestUri.Scheme;
                logHttp.RequestHeader = JsonConvert.SerializeObject(request.Headers);
                logHttp.RequestBody = request.Content != null ? await request.Content.ReadAsStringAsync() : null;
                logHttp.ResponseHeader = JsonConvert.SerializeObject(response.Headers);
                logHttp.ResponseBody = response.Content != null ? await response.Content.ReadAsStringAsync() : null;

                log.Http = logHttp;

                string logMessage = JsonConvert.SerializeObject(log, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                _logger.LogInformation(logMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while try register http client logger: {0}", ex.Message);
            }

            return response;
        }

        private static string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private static string ProjectName => Assembly.GetExecutingAssembly().GetName().Name;
    }
}