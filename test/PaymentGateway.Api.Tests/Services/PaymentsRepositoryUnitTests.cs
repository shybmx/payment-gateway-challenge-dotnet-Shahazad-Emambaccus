using System.Net;
using System.Text.Json;
using Moq;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.AppSettings;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Tests.Helpers;

namespace PaymentGateway.Api.Tests.Services
{
    public class PaymentsRepositoryUnitTests
    {
        private PaymentsRepository _paymentsRepository;

        private readonly Mock<ISanitisePaymentDetails> _sanitisePaymentDetailsMock;
        private readonly BankConfiguration _bankConfiguration;

        private readonly PostPaymentRequest _postPaymentRequest;
        private readonly BankResponse _bankResponse;

        public PaymentsRepositoryUnitTests()
        {
            _sanitisePaymentDetailsMock = new Mock<ISanitisePaymentDetails>();
            _bankConfiguration = new BankConfiguration { BaseAddress = "https://mockbankapi.com/", MaxRetry = 3, TimeoutInMilliseconds = 500 };

            _postPaymentRequest = new PostPaymentRequest
            {
                Amount = 100,
                Currency = "USD",
            };

            _bankResponse = new BankResponse
            {
                Authorized = true,
                AuthorizationCode = Guid.NewGuid().ToString()
            };

            _sanitisePaymentDetailsMock.Setup(x => x.SanitisedPaymentDetails(It.IsAny<long>()))
                .Returns((long cardNumber) => 1234);
        }

        [Fact]
        public async Task SendPayment_Returns_Correct_Object_Result_With_Authorized()
        {
            var mockHttpHandler = new MockHttpMessageHandler(JsonSerializer.Serialize(_bankResponse), HttpStatusCode.OK);

            _paymentsRepository = new PaymentsRepository(_sanitisePaymentDetailsMock.Object,
                _bankConfiguration,
                new HttpClient(mockHttpHandler));

            var result = await _paymentsRepository.SendPayment(_postPaymentRequest);

            Assert.True(result.Status.Equals(PaymentStatus.Authorized));
            Assert.NotNull(result);
        }

        [Fact]
        public async Task SendPayment_Returns_Correct_Object_When_Call_To_Bank_Fails_With_Rejected()
        {
            var mockHttpHandler = new MockHttpMessageHandler(string.Empty, HttpStatusCode.InternalServerError);

            _paymentsRepository = new PaymentsRepository(_sanitisePaymentDetailsMock.Object,
                _bankConfiguration,
                new HttpClient(mockHttpHandler));

            var result = await _paymentsRepository.SendPayment(_postPaymentRequest);

            Assert.NotNull(result);
            Assert.True(result.Status.Equals(PaymentStatus.Rejected));
        }        

        [Fact]
        public async Task SendPayment_Returns_Correct_Object_When_Call_To_Bank_Returns_Null_With_Rejected()
        {
            var mockHttpHandler = new MockHttpMessageHandler(string.Empty, HttpStatusCode.OK);

            _paymentsRepository = new PaymentsRepository(_sanitisePaymentDetailsMock.Object,
                _bankConfiguration,
                new HttpClient(mockHttpHandler));

            var result = await _paymentsRepository.SendPayment(_postPaymentRequest);

            Assert.NotNull(result);
            Assert.True(result.Status.Equals(PaymentStatus.Rejected));
        }

        [Fact]
        public async Task SendPayment_Throws_Exception_When_Call_To_Bank_Throws_Exception()
        {
            _paymentsRepository = new PaymentsRepository(_sanitisePaymentDetailsMock.Object,
                _bankConfiguration,
                new HttpClient());

            await Assert.ThrowsAsync<Exception>(async () => await _paymentsRepository.SendPayment(_postPaymentRequest));
        }

        [Fact]
        public async Task SendPayment_Calls_SanitisePaymentDetails_Method_Once()
        {
            var mockHttpHandler = new MockHttpMessageHandler(JsonSerializer.Serialize(_bankResponse), HttpStatusCode.OK);
            _paymentsRepository = new PaymentsRepository(_sanitisePaymentDetailsMock.Object,
                _bankConfiguration,
                new HttpClient(mockHttpHandler));
            var result = await _paymentsRepository.SendPayment(_postPaymentRequest);
            _sanitisePaymentDetailsMock.Verify(x => x.SanitisedPaymentDetails(It.IsAny<long>()), Times.Once);
        }

        [Fact]
        public async Task GetPayment_Returns_Correct_Object_Result()
        {
            var mockHttpHandler = new MockHttpMessageHandler(JsonSerializer.Serialize(_bankResponse), HttpStatusCode.OK);

            _paymentsRepository = new PaymentsRepository(_sanitisePaymentDetailsMock.Object,
                _bankConfiguration,
                new HttpClient(mockHttpHandler));

            var result = await _paymentsRepository.GetPayment(Guid.Parse(_bankResponse.AuthorizationCode));

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetPayment_Returns_Null_When_Call_To_Bank_Fails()
        {
            var mockHttpHandler = new MockHttpMessageHandler(string.Empty, HttpStatusCode.InternalServerError);

            _paymentsRepository = new PaymentsRepository(_sanitisePaymentDetailsMock.Object,
                _bankConfiguration,
                new HttpClient(mockHttpHandler));

            var result = await _paymentsRepository.GetPayment(Guid.Parse(_bankResponse.AuthorizationCode));

            Assert.Null(result);
        }

        [Fact]
        public async Task GetPayment_Returns_Null_When_Call_To_Bank_Returns_Null()
        {
            var mockHttpHandler = new MockHttpMessageHandler(string.Empty, HttpStatusCode.OK);

            _paymentsRepository = new PaymentsRepository(_sanitisePaymentDetailsMock.Object,
                _bankConfiguration,
                new HttpClient(mockHttpHandler));

            var result = await _paymentsRepository.GetPayment(Guid.Parse(_bankResponse.AuthorizationCode));

            Assert.Null(result);
        }

        [Fact]
        public async Task GetPayment_Throws_Exception_When_Call_To_Bank_Throws_Exception()
        {
            _paymentsRepository = new PaymentsRepository(_sanitisePaymentDetailsMock.Object,
                _bankConfiguration,
                new HttpClient());

            await Assert.ThrowsAsync<Exception>(async () => await _paymentsRepository.GetPayment(Guid.Parse(_bankResponse.AuthorizationCode)));
        }

        [Fact]
        public async Task GetPayment_Calls_SanitisePaymentDetails_Method_Once()
        {
            var mockHttpHandler = new MockHttpMessageHandler(JsonSerializer.Serialize(_bankResponse), HttpStatusCode.OK);

            _paymentsRepository = new PaymentsRepository(_sanitisePaymentDetailsMock.Object,
                _bankConfiguration,
                new HttpClient(mockHttpHandler));

            var result = await _paymentsRepository.GetPayment(Guid.Parse(_bankResponse.AuthorizationCode));

            _sanitisePaymentDetailsMock.Verify(x => x.SanitisedPaymentDetails(It.IsAny<long>()), Times.Once);
        }
    }
}
