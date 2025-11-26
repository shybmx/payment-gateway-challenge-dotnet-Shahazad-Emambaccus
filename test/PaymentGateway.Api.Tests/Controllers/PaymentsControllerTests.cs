using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests.Controllers;

public class PaymentsControllerTests
{
    private readonly Mock<IPaymentsRepository> _paymentRepositoryMock;
    private readonly PostPaymentRequest _postPaymentRequest;
    private readonly PaymentsController _paymentsController;

    public PaymentsControllerTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentsRepository>();
        _paymentRepositoryMock.Setup(x => x.SendPayment(It.IsAny<PostPaymentRequest>()))
            .ReturnsAsync(new PostPaymentResponse());

        _postPaymentRequest = new PostPaymentRequest
        {
            CardNumber = 11111111111111,
            ExpiryMonth = 1,
            ExpiryYear = 3000,
            Currency = "GBP",
            Amount = 1,
            Cvv = 123
        };

        _paymentsController = new PaymentsController(_paymentRepositoryMock.Object);
    }

    [Fact]
    public async Task ProcessPaymentAsync_Returns_BadRequest_When_Request_Is_Invalid()
    {
        _paymentsController.ModelState.AddModelError("Error", "Invalid Model");

        var response = await _paymentsController.ProcessPaymentAsync(null);

        Assert.IsType<BadRequestObjectResult>(response.Result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_Calls_SendPayment_Once()
    {
        await _paymentsController.ProcessPaymentAsync(_postPaymentRequest);
        _paymentRepositoryMock.Verify(x => x.SendPayment(It.IsAny<PostPaymentRequest>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPaymentAsync_Returns_BadRequest_When_PaymentResponse_Is_Rejected()
    {
        _paymentRepositoryMock.Setup(x => x.SendPayment(It.IsAny<PostPaymentRequest>()))
            .ReturnsAsync(new PostPaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.Rejected
            });

        var response = await _paymentsController.ProcessPaymentAsync(_postPaymentRequest);

        Assert.IsType<BadRequestObjectResult>(response.Result);
    }

    [Fact]
    public async Task ProcessPaymentAsync_Returns_Ok_When_Payment_Is_Processed()
    {
        var response = await _paymentsController.ProcessPaymentAsync(_postPaymentRequest);

        Assert.IsType<OkObjectResult>(response.Result);
    }

    [Fact]
    public async Task GetPaymentAsync_Returns_NotFound_When_Payment_Does_Not_Exist()
    {
        _paymentRepositoryMock.Setup(x => x.GetPayment(It.IsAny<Guid>()))
            .ReturnsAsync((GetPaymentResponse)null);

        var response = await _paymentsController.GetPaymentAsync(Guid.NewGuid());

        Assert.IsType<NotFoundObjectResult>(response.Result);
    }

    [Fact]
    public async Task GetPaymentAsync_Returns_Ok_When_Payment_Exists()
    {
        _paymentRepositoryMock.Setup(x => x.GetPayment(It.IsAny<Guid>()))
            .ReturnsAsync(new GetPaymentResponse());

        var response = await _paymentsController.GetPaymentAsync(Guid.NewGuid());

        Assert.IsType<OkObjectResult>(response.Result);
    }

    [Fact]
    public async Task GetPaymentAsync_Calls_GetPayment_Once()
    {
        await _paymentsController.GetPaymentAsync(Guid.NewGuid());
        _paymentRepositoryMock.Verify(x => x.GetPayment(It.IsAny<Guid>()), Times.Once);
    }
}