using ErrorOr;
using MailKit.Search;
using Microsoft.Extensions.Options;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Domain.Payments;
using StrikeDefender.Domain.Plans;
using StrikeDefender.Domain.Users;
using StrikeDefender.Infrastructure.ExternalServices.Payment.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

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
                IPaymobTokenProvider tokenProvider,IGenericRepository<PaymentTransaction> genericRepository,IUnitOfWork unitOfWork)
            {
                _httpClient = httpClient;
                _settings = settings.Value;
                _tokenProvider = tokenProvider;
                _paymentRepo = genericRepository;
            _unitOfWork = unitOfWork;
        }

      
        public async Task<ErrorOr<string>>ProcessPaymentAsync(AppUser user, Plan plan)
        {
            var authToken = await _tokenProvider.GetTokenAsync();

                var paymobOrderId = await CreateOrderAsync(authToken, plan);

                var paymentKey = await GetPaymentKeyAsync(authToken, paymobOrderId,user, plan);

            var transactionResult = PaymentTransaction.Create(
                paymobOrderId,
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
                                durationDays =plan.DurationInDays
                            }
                        }
                    });

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<PaymobOrderResponse>();
                return data.Id;
            }

            private async Task<string> GetPaymentKeyAsync(string authToken, int orderId,AppUser user, Plan plan)
            {
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
                            first_name = user.FullName ?? "NA",
                            last_name =  "NA",
                            email = user.Email ?? "test@test.com",
                            phone_number = user.PhoneNumber ?? "01000000000",
                            apartment = "NA",
                            floor = "NA",
                            street = "NA",
                            building = "NA",
                            shipping_method = "NA",
                            city = "Cairo",
                            country = "EG",
                            postal_code = "NA"
                        },
                        currency = "EGP",
                        integration_id = _settings.IntegrationId
                    });

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadFromJsonAsync<PaymobPaymentKeyResponse>();
                return data.Token;
            }

    }
    }

