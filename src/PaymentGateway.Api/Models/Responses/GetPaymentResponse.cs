using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Responses;

public class GetPaymentResponse
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public long CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }

    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be exactly 3 characters long")]
    public string Currency { get; set; }
    public int Amount { get; set; }
}