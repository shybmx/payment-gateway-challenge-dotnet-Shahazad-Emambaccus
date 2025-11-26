using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentsRepository _paymentsRepository;

    public PaymentsController(IPaymentsRepository paymentsRepository)
    {
        _paymentsRepository = paymentsRepository;
    }

    [HttpPost("ProcessPayment")]
    public async Task<ActionResult<PostPaymentResponse>> ProcessPaymentAsync([FromBody] PostPaymentRequest paymentRequest)
    {
        if (paymentRequest == null || !ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var paymentResponse = await _paymentsRepository.SendPayment(paymentRequest);

        if (paymentResponse.Status == PaymentStatus.Rejected) 
        { 
            return new BadRequestObjectResult(paymentResponse);
        }

        return new OkObjectResult(paymentResponse);
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = await _paymentsRepository.GetPayment(id);

        if (payment == null)
        {
            return new NotFoundObjectResult($"Payment with ID {id} not found");
        }

        return new OkObjectResult(payment);
    }
}