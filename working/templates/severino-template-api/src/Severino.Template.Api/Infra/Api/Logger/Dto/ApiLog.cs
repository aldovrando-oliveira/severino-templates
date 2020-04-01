using Newtonsoft.Json;

namespace Severino.Template.Api.Infra.Api.Logger.Dto
{
    public class ApiLog
    {
        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("team")]
        public string Team { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("trace")]
        public Trace Trace { get; set; }

        [JsonProperty("http")]
        public HttpLog Http { get; set; }
    }
}