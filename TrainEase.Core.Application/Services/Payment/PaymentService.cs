using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TrainEase.Core.Application.Abstrations.Model.Payment;
using TrainEase.Core.Application.Abstrations.Services;
using TrainEase.Core.Application.Exceptions;
using TrainEase.Core.Domain.Contracts;
using TrainEase.Core.Domain.Entities;
using TrainEase.Core.Domain.Entities.Enums;

namespace TrainEase.Core.Application.Services.Payment
{
    public class PaymentService(
        IUnitOfWork unitOfWork,
        IOptions<PaymobSettings> paymobSettings,
        HttpClient httpClient) : IPaymentService
    {
        private readonly PaymobSettings _settings = paymobSettings.Value;
        private const int PendingBookingExpirationMinutes = 15;
        public async Task<PaymentResultDto> InitiatePaymentAsync(int bookingId, string userId)
        {
           

            var bookingRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Booking, int>();
            var paymentRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Payment, int>();

            var booking = await bookingRepo
                .GetQueryable(b => b.Id == bookingId && !b.IsDeleted)
                .Include(b => b.User)
                .Include(b => b.Payment)
                .FirstOrDefaultAsync();

            if (booking is null)
                throw new NotFoundException("Booking", bookingId);

            if (booking.UserId != userId)
                throw new UnAuthorizedException("This booking does not belong to you.");

            if (booking.Status == BookingStatus.Pending &&
                booking.BookingDate < DateTime.UtcNow.AddMinutes(-PendingBookingExpirationMinutes))
            {
                booking.Status = BookingStatus.Cancelled;
                booking.UpdatedAt = DateTime.UtcNow;
                bookingRepo.Update(booking);

                var stalePayments = await paymentRepo
                    .GetQueryable(p => p.BookingId == bookingId && !p.IsDeleted && p.Status == PaymentStatus.Pending)
                    .ToListAsync();

                foreach (var stalePayment in stalePayments)
                {
                    stalePayment.Status = PaymentStatus.Failed;
                    stalePayment.UpdatedAt = DateTime.UtcNow;
                    paymentRepo.Update(stalePayment);
                }

                await unitOfWork.CompeleteAsync();

                throw new BadRequestException("This booking expired after 15 minutes and was cancelled.");
            }

            if (booking.Status != BookingStatus.Pending)
                throw new BadRequestException("This booking is already paid or cancelled.");

            var latestPayment = await paymentRepo
                .GetQueryable(p => p.BookingId == bookingId && !p.IsDeleted)
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();

            if (latestPayment is not null && latestPayment.Status == PaymentStatus.Paid)
                throw new BadRequestException("This booking is already paid.");

            if (latestPayment is not null && latestPayment.Status == PaymentStatus.Pending)
            {
                latestPayment.Status = PaymentStatus.Failed;
                latestPayment.UpdatedAt = DateTime.UtcNow;
                paymentRepo.Update(latestPayment);
                await unitOfWork.CompeleteAsync();
            }

            var authToken = await GetAuthTokenAsync();
            var orderId = await CreateOrderAsync(authToken, booking);
            var paymentKey = await GetPaymentKeyAsync(authToken, orderId, booking);

            var payment = new TrainEase.Core.Domain.Entities.Payment
            {
                BookingId = bookingId,
                Amount = booking.TotalAmount,
                Status = PaymentStatus.Pending,
                Method = PaymentMethod.Card,
                PaymentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await paymentRepo.AddAsync(payment);
            await unitOfWork.CompeleteAsync();

            return new PaymentResultDto
            {
                PaymentKey = paymentKey,
                IframeUrl = $"https://accept.paymob.com/api/acceptance/iframes/{_settings.IframeId}?payment_token={paymentKey}"
            };
        }

        public async Task HandleWebhookAsync(string payload, string hmacSignature)
        {
            if (string.IsNullOrWhiteSpace(payload))
                throw new BadRequestException("Webhook payload is empty.");

            if (string.IsNullOrWhiteSpace(hmacSignature))
                throw new UnAuthorizedException("Missing HMAC signature.");

            if (!VerifyHmac(payload, hmacSignature))
                throw new UnAuthorizedException("Invalid HMAC signature.");

            var webhookData = JsonSerializer.Deserialize<JsonElement>(payload);
            var transactionObj = webhookData.GetProperty("obj");

            var success = transactionObj.GetProperty("success").GetBoolean();
            var pending = transactionObj.GetProperty("pending").GetBoolean();
            var amountCents = transactionObj.GetProperty("amount_cents").GetInt32();

            var merchantOrderId = transactionObj
                .GetProperty("order")
                .GetProperty("merchant_order_id")
                .GetString();

            if (!int.TryParse(merchantOrderId, out int bookingId))
                return;

            var bookingRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Booking, int>();
            var booking = await bookingRepo
                .GetQueryable(b => b.Id == bookingId && !b.IsDeleted)
                .FirstOrDefaultAsync();

            if (booking is null)
                return;

            // لو Paymob لسه شايف العملية pending، ما نعملش أي تحديث
            if (pending)
                return;

            var expectedAmountCents = (int)(booking.TotalAmount * 100);
            if (amountCents != expectedAmountCents)
                throw new BadRequestException("Webhook amount does not match booking total.");

            var paymentRepo = unitOfWork.GetRepository<TrainEase.Core.Domain.Entities.Payment, int>();
            var payment = await paymentRepo
                .GetQueryable(p => p.BookingId == bookingId && !p.IsDeleted)
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();

            if (payment is null)
                return;

            var invoiceRepo = unitOfWork.GetRepository<Invoice, int>();
            var existingInvoice = await invoiceRepo
                .GetQueryable(i => i.BookingId == bookingId && !i.IsDeleted)
                .FirstOrDefaultAsync();

            // idempotency: لو العملية اتعالجت قبل كده بنجاح، اخرج
            if (booking.Status == BookingStatus.Confirmed &&
                payment.Status == PaymentStatus.Paid &&
                existingInvoice is not null)
            {
                return;
            }

            if (success)
            {
                booking.Status = BookingStatus.Confirmed;
                payment.Status = PaymentStatus.Paid;
                payment.PaymentDate = DateTime.UtcNow;

                if (existingInvoice is null)
                {
                    var invoice = new Invoice
                    {
                        BookingId = bookingId,
                        InvoiceNumber = $"INV-{bookingId}-{DateTime.UtcNow:yyyyMMddHHmmss}",
                        NetPrice = booking.TotalAmount,
                        Tax = booking.TotalAmount * 0.14m,
                        Total = booking.TotalAmount * 1.14m,
                        CreatedAt = DateTime.UtcNow
                    };

                    await invoiceRepo.AddAsync(invoice);
                }
            }
            else
            {
                // لو الحجز اتأكد بالفعل، ما نرجعوش cancelled بسبب webhook قديمة
                if (booking.Status != BookingStatus.Confirmed)
                    booking.Status = BookingStatus.Cancelled;

                if (payment.Status != PaymentStatus.Paid)
                    payment.Status = PaymentStatus.Failed;
            }

            booking.UpdatedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            bookingRepo.Update(booking);
            paymentRepo.Update(payment);

            await unitOfWork.CompeleteAsync();
        }

        private async Task<string> GetAuthTokenAsync()
        {
            var response = await httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/auth/tokens",
                new { api_key = _settings.ApiKey });

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (!content.TryGetProperty("token", out var tokenElement))
                throw new BadRequestException("Paymob auth token was not returned.");

            return tokenElement.GetString()!;
        }

        private async Task<long> CreateOrderAsync(string authToken, TrainEase.Core.Domain.Entities.Booking booking)
        {
            var body = new
            {
                auth_token = authToken,
                delivery_needed = false,
                amount_cents = (int)(booking.TotalAmount * 100),
                currency = "EGP",
                merchant_order_id = booking.Id!.ToString(),
                items = new[]
                {
                    new
                    {
                        name = $"Booking #{booking.Id}",
                        amount_cents = (int)(booking.TotalAmount * 100),
                        description = "Train Ticket",
                        quantity = 1
                    }
                }
            };

            var response = await httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/ecommerce/orders", body);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (!content.TryGetProperty("id", out var idElement))
                throw new BadRequestException("Paymob order id was not returned.");

            return idElement.GetInt64();
        }

        private async Task<string> GetPaymentKeyAsync(string authToken, long orderId, TrainEase.Core.Domain.Entities.Booking booking)
        {
            var body = new
            {
                auth_token = authToken,
                amount_cents = (int)(booking.TotalAmount * 100),
                expiration = 3600,
                order_id = orderId,
                billing_data = new
                {
                    email = booking.User?.Email ?? "NA",
                    first_name = booking.User?.FullName ?? "NA",
                    last_name = "NA",
                    phone_number = booking.User?.PhoneNumber ?? "NA",
                    apartment = "NA",
                    floor = "NA",
                    street = "NA",
                    building = "NA",
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "NA",
                    country = "EG",
                    state = "NA"
                },
                currency = "EGP",
                integration_id = _settings.IntegrationId
            };

            var response = await httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/acceptance/payment_keys", body);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (!content.TryGetProperty("token", out var tokenElement))
                throw new BadRequestException("Paymob payment key was not returned.");

            return tokenElement.GetString()!;
        }

        private bool VerifyHmac(string payload, string receivedHmac)
        {
            var webhookData = JsonSerializer.Deserialize<JsonElement>(payload);
            var transactionObj = webhookData.GetProperty("obj");

            var concatenated = string.Concat(
                transactionObj.GetProperty("amount_cents").GetInt32(),
                transactionObj.GetProperty("created_at").GetString(),
                transactionObj.GetProperty("currency").GetString(),
                transactionObj.GetProperty("error_occured").GetBoolean().ToString().ToLower(),
                transactionObj.GetProperty("has_parent_transaction").GetBoolean().ToString().ToLower(),
                transactionObj.GetProperty("id").GetInt64(),
                transactionObj.GetProperty("integration_id").GetInt32(),
                transactionObj.GetProperty("is_3d_secure").GetBoolean().ToString().ToLower(),
                transactionObj.GetProperty("is_auth").GetBoolean().ToString().ToLower(),
                transactionObj.GetProperty("is_capture").GetBoolean().ToString().ToLower(),
                transactionObj.GetProperty("is_refunded").GetBoolean().ToString().ToLower(),
                transactionObj.GetProperty("is_standalone_payment").GetBoolean().ToString().ToLower(),
                transactionObj.GetProperty("is_voided").GetBoolean().ToString().ToLower(),
                transactionObj.GetProperty("order").GetProperty("id").GetInt64(),
                transactionObj.GetProperty("owner").GetInt32(),
                transactionObj.GetProperty("pending").GetBoolean().ToString().ToLower(),
                transactionObj.GetProperty("source_data").GetProperty("pan").GetString(),
                transactionObj.GetProperty("source_data").GetProperty("sub_type").GetString(),
                transactionObj.GetProperty("source_data").GetProperty("type").GetString(),
                transactionObj.GetProperty("success").GetBoolean().ToString().ToLower()
            );

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_settings.HmacSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
            var computed = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return string.Equals(computed, receivedHmac, StringComparison.OrdinalIgnoreCase);
        }
    }
}