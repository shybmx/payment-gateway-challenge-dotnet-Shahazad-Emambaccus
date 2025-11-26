using Microsoft.AspNetCore.Mvc.Filters;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Middleware
{
    public class ValidateExpiryMiddleware : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ModelState.ErrorCount == 0 && context.ActionArguments.TryGetValue("paymentRequest", out var request) &&
                    request is PostPaymentRequest paymentRequest)
            {
                ValidateExpiryDate(context, paymentRequest);
            }

            await next();
        }

        private static void ValidateExpiryDate(ActionExecutingContext context, PostPaymentRequest paymentRequest)
        {
            var currentYear = DateTime.UtcNow.Year;
            var currentMonth = DateTime.UtcNow.Month;
            if (paymentRequest.ExpiryYear < currentYear ||
                paymentRequest.ExpiryYear == currentYear && paymentRequest.ExpiryMonth < currentMonth)
            {
                context.ModelState.AddModelError("ExpiryDate", "The card expiry date is in the past.");
                return;
            }
        }
    }
}
