using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Infrastructure.ExternalServices.Payment.DTO;
using System.Net.Http.Json;

namespace StrikeDefender.Infrastructure.ExternalServices.Payment.Helper
{
    public class PaymobTokenProvider : IPaymobTokenProvider
    {
        private readonly HttpClient _httpClient;
        private readonly PaymobSettings _settings;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "PAYMOB_AUTH_TOKEN";

        public PaymobTokenProvider(
            HttpClient httpClient,
            IOptions<PaymobSettings> settings,
            IMemoryCache cache)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _cache = cache;
        }

        public async Task<string> GetTokenAsync()
        {
            if (_cache.TryGetValue(CacheKey, out string token))
                return token;

            var response = await _httpClient.PostAsJsonAsync(
                $"{_settings.BaseUrl}/auth/tokens",
                new { api_key = _settings.ApiKey });

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<PaymobAuthResponse>();

            // cache for 30 minutes
            _cache.Set(CacheKey, data.Token, TimeSpan.FromMinutes(30));

            return data.Token;
        }
    }
}
