using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Interfaces
{
    public interface ISanitisePaymentDetails
    {
        int SanitisedPaymentDetails(long cardNumber); 
    }
}
