using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Interfaces
{
    public interface IPaymentsRepository
    {
        Task<PostPaymentResponse> SendPayment(PostPaymentRequest paymentRequest);
        Task<GetPaymentResponse> GetPayment(Guid paymentId);
    }
}
