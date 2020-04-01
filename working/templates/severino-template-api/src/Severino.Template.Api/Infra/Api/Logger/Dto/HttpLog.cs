using Newtonsoft.Json;

namespace Severino.Template.Api.Infra.Api.Logger.Dto
{
    public class HttpLog
    {
        [JsonProperty("latency_seconds")]
        public double LatencySeconds { get; set; }

        [JsonProperty("request_method")]
        public string RequestMethod { get; set; }

        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("protocol")]
        public string Protocol { get; set; }

        [JsonProperty("request_body")]
        public string RequestBody { get; set; }

        [JsonProperty("request_header")]
        public string RequestHeader { get; set; }

        [JsonProperty("response_body")]
        public string ResponseBody { get; set; }

        [JsonProperty("response_header")]
        public string ResponseHeader { get; set; }

    }
}