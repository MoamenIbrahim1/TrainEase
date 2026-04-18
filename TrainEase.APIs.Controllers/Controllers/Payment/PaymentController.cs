// PaymentController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrainEase.APIs.Controllers.Controllers.Base;
using TrainEase.Core.Application.Abstrations.Services;

namespace TrainEase.APIs.Controllers.Controllers.Payment
{
    public class PaymentController(IServiceManager serviceManager) : BaseApiController
    {
        // المستخدم بيضغط "Confirm & Pay"
        [Authorize]
        [HttpPost("initiate/{bookingId}")]
        public async Task<IActionResult> InitiatePayment(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await serviceManager.PaymentService
                .InitiatePaymentAsync(bookingId, userId);
            return Ok(result);
        }

        // Paymob بيستدعي الـ endpoint ده بعد الدفع
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var payload = await new StreamReader(Request.Body).ReadToEndAsync();
            var hmac = Request.Query["hmac"].ToString();
            await serviceManager.PaymentService.HandleWebhookAsync(payload, hmac);
            return Ok();
        }
    }
}