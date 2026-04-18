using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainEase.Core.Application.Abstrations.Model.Payment;

namespace TrainEase.Core.Application.Abstrations.Services
{
    public interface IPaymentService
    {
        // بتبدأ الـ payment وبترجع الـ iframe url
        Task<PaymentResultDto> InitiatePaymentAsync(int bookingId, string userId);

        // Paymob بيستدعيها لما المستخدم يدفع
        Task HandleWebhookAsync(string payload, string hmacSignature);
    }
}
