namespace PaymentGateway.Api.Models.AppSettings
{
    public class BankConfiguration
    {
        public string BaseAddress { get; set; }
        public int MaxRetry { get; set; }
        public int TimeoutInMilliseconds { get; set; }
    }
}
