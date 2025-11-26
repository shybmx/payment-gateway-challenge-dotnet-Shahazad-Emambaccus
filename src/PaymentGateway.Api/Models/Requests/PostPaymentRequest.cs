using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest
{
    [Required]
    [Range(11111111111111, 99999999999999, ErrorMessage = "Card number must be 14 digits long")]
    public long CardNumber { get; set; }

    [Required]
    [Range(1, 12, ErrorMessage = "ExpiryMonth must be between 1 and 12")]
    public int ExpiryMonth { get; set; }

    [Required]
    public int ExpiryYear { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be exactly 3 characters long")]
    public string Currency { get; set; }

    [Required]
    public int Amount { get; set; }
    
    [Required]
    [Range(100, 9999, ErrorMessage = "Cvv must be between 3 and 4 digits long")]
    public int Cvv { get; set; }
}