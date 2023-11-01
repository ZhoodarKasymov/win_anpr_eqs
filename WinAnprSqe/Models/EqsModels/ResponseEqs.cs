using Newtonsoft.Json;

namespace WinAnprSqe.Models.EqsModels
{
    public class ResponseEqs
    {
        [JsonProperty("result")]
        public Result Result { get; set; }

        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}

