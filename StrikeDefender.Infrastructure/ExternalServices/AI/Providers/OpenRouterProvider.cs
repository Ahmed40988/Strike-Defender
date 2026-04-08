using ErrorOr;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Infrastructure.ExternalServices.AI.Configurations;
using StrikeDefender.Infrastructure.ExternalServices.AI.Services;
using System.Text;
using System.Text.Json;

namespace StrikeDefender.Infrastructure.ExternalServices.AI.Providers
{
    public class OpenRouterProvider : IAiProvider
    {
        private readonly ILogger<OpenRouterProvider> _logger;
        private readonly HttpClient _http;
        private readonly OpenRouterOptions _opt;

        public string Name => "OpenRouter";

        public OpenRouterProvider(HttpClient http, IOptions<OpenRouterOptions> opt,ILogger<OpenRouterProvider> logger)
        {
            _http = http;
            _logger = logger;
            _opt = opt.Value;
        }

        public async Task<ErrorOr<List<string>>> SendAsync(
            string prompt,
            CancellationToken ct)
        {
            var request = new
            {
                model = _opt.Model, // مثال: "mistralai/mixtral-8x7b"
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var req = new HttpRequestMessage(HttpMethod.Post, _opt.BaseUrl)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json")
            };

            req.Headers.Add("Authorization", $"Bearer {_opt.ApiKey}");

            var res = await _http.SendAsync(req, ct);

            var body = await res.Content.ReadAsStringAsync(ct);

            if (!res.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Provider {Provider} failed: Status={Status}, Body={Body}",
                    Name,
                    res.StatusCode,
                    body);

                return Error.Failure($"AI.{Name}Error", body);
            }

            var str = await res.Content.ReadAsStringAsync(ct);

            using var doc = JsonDocument.Parse(str);

            var text = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return text!
                .Split('\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }
    }
}