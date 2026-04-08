using ErrorOr;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Infrastructure.ExternalServices.AI.Configurations;
using System.Text;
using System.Text.Json;

namespace StrikeDefender.Infrastructure.ExternalServices.AI.Providers
{
    public class GeminiProvider : IAiProvider
    {
        private readonly HttpClient _http;
        private readonly ILogger<GeminiProvider> _logger;
        private readonly GeminiOptions _opt;

        public string Name => "Gemini";

        public GeminiProvider(
            HttpClient http,
            IOptions<GeminiOptions> opt,
            ILogger<GeminiProvider> logger)
        {
            _http = http;
            _opt = opt.Value;
            _logger = logger;
        }


        public async Task<ErrorOr<List<string>>> SendAsync(
            string prompt,
            CancellationToken ct)
        {
           var url =
            $"{_opt.BaseUrl}/models/{_opt.Model}:generateContent?key={_opt.ApiKey}";

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = _opt.Temperature,
                    maxOutputTokens = _opt.MaxOutputTokens,
                    topP = _opt.TopP
                }
            };

            var json = JsonSerializer.Serialize(body);

            _logger.LogInformation("Gemini Request: {Json}", json);

            var res = await _http.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json"),
                ct);

            var responseBody = await res.Content.ReadAsStringAsync(ct);

            // 🔥 أهم جزء (Debug + Error Handling)
            if (!res.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Gemini failed: Status={Status}, Body={Body}",
                    res.StatusCode,
                    responseBody);

                return Error.Failure(
                    code: "AI.GeminiError",
                    description: responseBody);
            }

            _logger.LogInformation("Gemini Success Response: {Body}", responseBody);

            try
            {
                using var doc = JsonDocument.Parse(responseBody);

                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                if (string.IsNullOrWhiteSpace(text))
                {
                    _logger.LogWarning("Gemini returned empty text");
                    return Error.Failure("AI.Empty", "Empty response from Gemini");
                }

                return text
                    .Split('\n')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse Gemini response");

                return Error.Failure(
                    code: "AI.ParseError",
                    description: "Failed to parse Gemini response");
            }
        }
    }
}