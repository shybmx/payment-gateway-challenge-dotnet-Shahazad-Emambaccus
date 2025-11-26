using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

using PaymentGateway.Api.Middleware;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.Middleware
{
    public class ValidateExpiryMiddlewareUnitTests
    {
        private readonly IAsyncActionFilter _validateExpiryDateMiddleware;
        private readonly ActionExecutingContext _context;
        private readonly PostPaymentRequest _paymentRequest;
        
        private const string ModelStateKey = "ExpiryDate";

        public ValidateExpiryMiddlewareUnitTests()
        {
            _context = new ActionExecutingContext(
            new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor(), new ModelStateDictionary()), new List<IFilterMetadata>(),
            new Dictionary<string, object>(), new object());

            _validateExpiryDateMiddleware = new ValidateExpiryMiddleware();
            _paymentRequest = new PostPaymentRequest
            {
                CardNumber = 4111111111111111,
                ExpiryMonth = 12,
                ExpiryYear = DateTime.UtcNow.Year + 1,
                Cvv = 123,
                Amount = 1000,
                Currency = "USD"
            };

            _context.ActionArguments["paymentRequest"] = _paymentRequest;
        }

        [Fact]
        public async Task OnActionExecutionAsync_WhenRequestIsValid_DoesNotAddFlagsToRequest()
        {
            await _validateExpiryDateMiddleware.OnActionExecutionAsync(_context, () => Task.FromResult<ActionExecutedContext>(null));
            
            Assert.False(_context.ModelState.ContainsKey(ModelStateKey));
        }

        [Theory]
        [InlineData(1, 2000, true)] 
        [InlineData(1, 3000, false)]
        [InlineData(1, 2026, false)]
        public async Task OnActionExecutionAsync_WhenRequestIsInValid_ChecksFlagAddedCorrectly(int expiryMonth, int expiryYear, bool expected)
        {
            _paymentRequest.ExpiryMonth = expiryMonth;
            _paymentRequest.ExpiryYear = expiryYear;
            
            await _validateExpiryDateMiddleware.OnActionExecutionAsync(_context, () => Task.FromResult<ActionExecutedContext>(null));

            Assert.Equal(_context.ModelState.ContainsKey(ModelStateKey), expected);
        }

    }
}
