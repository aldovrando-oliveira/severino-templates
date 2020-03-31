using Newtonsoft.Json;

namespace Severino.Template.Hangfire.Infra.Api.Logger.Dto
{
    public class Trace
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}