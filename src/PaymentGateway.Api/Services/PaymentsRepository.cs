using System.Text;
using Newtonsoft.Json;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.AppSettings;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using Polly;
using Polly.Retry;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository : IPaymentsRepository
{
    private readonly ISanitisePaymentDetails _sanitisePaymentDetails;

    private readonly BankConfiguration _bankConfiguration;    
    private readonly HttpClient _httpClient;
    private AsyncRetryPolicy _retryPolicy;

    private const string postPaymentsEndpoint = "payments";
    private const string getPaymentsEndpoint = "payments?paymentId=";
    private const string mediaType = "application/json";
 
    public PaymentsRepository(ISanitisePaymentDetails sanitisePaymentDetails, BankConfiguration bankConfiguration, HttpClient httpClient)
    {
        _sanitisePaymentDetails = sanitisePaymentDetails;
        _bankConfiguration = bankConfiguration;
        _httpClient = httpClient;
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(bankConfiguration.MaxRetry, retryAttempt => TimeSpan.FromMilliseconds(bankConfiguration.TimeoutInMilliseconds));
    }

    public async Task<PostPaymentResponse> SendPayment(PostPaymentRequest paymentRequest)
    {
        var request = BuildHttpRequest(HttpMethod.Post, postPaymentsEndpoint);

        var bankRequest = new BankRequest
        {
            CardNumber = paymentRequest.CardNumber.ToString(),
            ExpiryDate = $"{paymentRequest.ExpiryMonth}/{paymentRequest.ExpiryYear}",
            Amount = paymentRequest.Amount,
            Currency = paymentRequest.Currency,           
            Cvv = paymentRequest.Cvv.ToString()
        };

        request.Content = new StringContent(JsonConvert.SerializeObject(bankRequest), Encoding.UTF8, mediaType);

        try
        {
            var response = await _retryPolicy.ExecuteAsync(() => _httpClient.SendAsync(request));

            if(response == null || !response.IsSuccessStatusCode)
            {
                return FailedPayment();
            }

            var responseContent = JsonConvert.DeserializeObject<BankResponse>(await response.Content.ReadAsStringAsync());

            if(responseContent == null)
            {
                return FailedPayment();
            }

            return new PostPaymentResponse
            {
                Id = string.IsNullOrEmpty(responseContent.AuthorizationCode) 
                    ? Guid.NewGuid() 
                    : Guid.Parse(responseContent.AuthorizationCode),
                Status = responseContent.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined,
                CardNumberLastFour = _sanitisePaymentDetails.SanitisedPaymentDetails(paymentRequest.CardNumber),
                ExpiryMonth = paymentRequest.ExpiryMonth,
                ExpiryYear = paymentRequest.ExpiryYear,
                Currency = paymentRequest.Currency,
                Amount = paymentRequest.Amount
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<GetPaymentResponse> GetPayment(Guid paymentId)
    {
        var request = BuildHttpRequest(HttpMethod.Get, getPaymentsEndpoint + $"{paymentId}");

        try
        {
            var response = await _retryPolicy.ExecuteAsync(() => _httpClient.SendAsync(request));

            if (response == null || !response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseContent = JsonConvert.DeserializeObject<GetPaymentResponse>(await response.Content.ReadAsStringAsync());

            if (responseContent == null)
            {
                return null;
            }

            return new GetPaymentResponse
            {
                Id = responseContent.Id,
                Status = responseContent.Status,
                Amount = responseContent.Amount,
                Currency = responseContent.Currency,
                CardNumberLastFour = _sanitisePaymentDetails.SanitisedPaymentDetails(responseContent.CardNumberLastFour),
                ExpiryMonth = responseContent.ExpiryMonth,
                ExpiryYear = responseContent.ExpiryYear
            };
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private PostPaymentResponse FailedPayment()
    {
        return new PostPaymentResponse
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Rejected
        };
    }

    private HttpRequestMessage BuildHttpRequest(HttpMethod httpMethod, string endpoint)
    {
        return new HttpRequestMessage(httpMethod, _bankConfiguration.BaseAddress + endpoint);
    }
}