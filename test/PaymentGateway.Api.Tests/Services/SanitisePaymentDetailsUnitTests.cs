using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Services
{
    public class SanitisePaymentDetailsUnitTests
    {
        private readonly SanitisePaymentDetails _sanitisePaymentDetails;

        public SanitisePaymentDetailsUnitTests()
        {
            _sanitisePaymentDetails = new SanitisePaymentDetails(); 
        }

        [Theory]
        [InlineData(1234567812345678, 5678)]
        [InlineData(1234, 1234)]
        [InlineData(2, 2)]
        [InlineData(null, 0)]
        public void SanitisedPaymentDetails_Returns_Last_Four_Digits(long cardNumber, int expected)
        {
            int result = _sanitisePaymentDetails.SanitisedPaymentDetails(cardNumber);
            
            Assert.Equal(expected, result);
        }
    }
}
