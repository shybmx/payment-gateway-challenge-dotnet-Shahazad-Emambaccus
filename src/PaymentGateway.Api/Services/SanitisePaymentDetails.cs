using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services
{
    public class SanitisePaymentDetails : ISanitisePaymentDetails
    {
        private const int lastFourDigitsLength = 4;

        public int SanitisedPaymentDetails(long cardNumber)
        {
            string cardNumberString = cardNumber.ToString();

            string lastFourDigits = cardNumberString.Length >= lastFourDigitsLength
                ? cardNumberString[^lastFourDigitsLength..]
                : cardNumberString;

            return int.Parse(lastFourDigits);
        }
    }
}
