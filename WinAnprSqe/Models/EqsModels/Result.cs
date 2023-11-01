using Newtonsoft.Json;

namespace WinAnprSqe.Models.EqsModels
{
    public class Result
    {
        [JsonProperty("customer")]
        public Customer Customer { get; set; }
    }
}