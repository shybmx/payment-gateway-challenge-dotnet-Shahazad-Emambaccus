using Newtonsoft.Json;

namespace PaymentGateway.Api.Models.Responses
{
    public class BankResponse
    {
        [JsonProperty("authorized")]
        public bool Authorized { get; set; }

        [JsonProperty("authorization_code")]
        public string AuthorizationCode { get; set; }
    }
}
