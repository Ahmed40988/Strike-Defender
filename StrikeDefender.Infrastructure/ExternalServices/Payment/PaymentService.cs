using ErrorOr;
using Microsoft.Extensions.Options;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Domain.Payments;
using StrikeDefender.Domain.Plans;
using StrikeDefender.Domain.Users;
using StrikeDefender.Infrastructure.ExternalServices.Payment.DTO;
using System.Net.Http.Json;

namespace StrikeDefender.Infrastructure.ExternalServices.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly PaymobSettings _settings;
        private readonly IPaymobTokenProvider _tokenProvider;
        private readonly IGenericRepository<PaymentTransaction> _paymentRepo;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(
            HttpClient httpClient,
            IOptions<PaymobSettings> settings,
            IPaymobTokenProvider tokenProvider,
            IGenericRepository<PaymentTransaction> paymentRepo,
            IUnitOfWork unitOfWork)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _tokenProvider = tokenProvider;
            _paymentRepo = paymentRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<string>> ProcessPaymentAsync(AppUser user, Plan plan)
        {
            var authToken = await _tokenProvider.GetTokenAsync();

            var orderId = await CreateOrderAsync(authToken, plan);

            var paymentKey = await GetPaymentKeyAsync(authToken, orderId, user, plan);

            var transactionResult = PaymentTransaction.Create(
                orderId,
                user.Id,
                plan.Id,
                plan.Price);

            if (transactionResult.IsError)
                return transactionResult.Errors;

            await _paymentRepo.AddAsync(transactionResult.Value);
            await _unitOfWork.CommitChangesAsync();

            return $"{_settings.BaseUrl}/acceptance/iframes/{_settings.IframeId}?payment_token={paymentKey}";
        }

        private async Task<int> CreateOrderAsync(string authToken, Plan plan)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_settings.BaseUrl}/ecommerce/orders",
                new
                {
                    auth_token = authToken,
                    delivery_needed = "false",
                    amount_cents = (int)(plan.Price * 100),
                    currency = "EGP",
                    items = new List<object>
                    {
                        new
                        {
                            name = plan.Name,
                            amount_cents = (int)(plan.Price * 100),
                            description = "Subscription Plan",
                            durationDays = plan.DurationInDays
                        }
                    }
                });

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Paymob Order Error: {content}");

            var data = await response.Content.ReadFromJsonAsync<PaymobOrderResponse>();
            return data!.Id;
        }

        private async Task<string> GetPaymentKeyAsync(
            string authToken,
            int orderId,
            AppUser user,
            Plan plan)
        {
            // 🔥 sanitize user data
            var firstName = string.IsNullOrWhiteSpace(user.FullName)
                ? "User"
                : user.FullName.Split(' ')[0];

            var email = string.IsNullOrWhiteSpace(user.Email)
                ? "test@test.com"
                : user.Email;

            var phone = NormalizePhone(user.PhoneNumber);

            var response = await _httpClient.PostAsJsonAsync(
                $"{_settings.BaseUrl}/acceptance/payment_keys",
                new
                {
                    auth_token = authToken,
                    amount_cents = (int)(plan.Price * 100),
                    expiration = 3600,
                    order_id = orderId,
                    billing_data = new
                    {
                        first_name = firstName,
                        last_name = "NA",
                        email = email,
                        phone_number = phone,
                        apartment = "NA",
                        floor = "NA",
                        street = "NA",
                        building = "NA",
                        shipping_method = "NA",
                        city = "Cairo",
                        country = "EG",
                        postal_code = "12345"
                    },
                    currency = "EGP",
                    integration_id = _settings.IntegrationId
                });

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Paymob PaymentKey Error: {content}");

            var data = await response.Content.ReadFromJsonAsync<PaymobPaymentKeyResponse>();

            return data!.Token;
        }

        private string NormalizePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "201000000000";

            phone = phone.Replace("+", "").Trim();

            if (phone.StartsWith("0"))
                phone = "20" + phone.Substring(1);

            return phone;
        }
    }
}