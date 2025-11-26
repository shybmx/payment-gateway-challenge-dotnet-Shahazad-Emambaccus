using System.Net;

namespace PaymentGateway.Api.Tests.Helpers
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseMessage;
        private readonly HttpStatusCode _statusCode;

        public MockHttpMessageHandler(string responseMessage, HttpStatusCode statusCode)
        {
            _responseMessage = responseMessage;
            _statusCode = statusCode;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseMessage)
            };

            return response;
        }
    }
}
