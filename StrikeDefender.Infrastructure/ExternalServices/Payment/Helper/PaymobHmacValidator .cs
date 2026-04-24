using Microsoft.Extensions.Options;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Application.Payments.DTO;
using System.Security.Cryptography;
using System.Text;

namespace StrikeDefender.Infrastructure.ExternalServices.Payment.Helper
{
    public class PaymobHmacValidator : IPaymobHmacValidator
    {
        private readonly PaymobSettings _settings;

        public PaymobHmacValidator(IOptions<PaymobSettings> settings)
        {
            _settings = settings.Value;
        }

        public bool Validate(PaymobWebhookRequest request)
        {
            var hmacKey = _settings.HMAC;

            // ✅ استخدم البيانات الصح من DTO الجديد
            var data = $"{request.AmountCents}{request.OrderId}{request.Success}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(hmacKey));

            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));

            var computed = BitConverter.ToString(hash)
                .Replace("-", "")
                .ToLower();

            return computed == request.Hmac.ToLower();
        }
    }
}