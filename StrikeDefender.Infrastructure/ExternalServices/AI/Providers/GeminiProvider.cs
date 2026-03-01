using ErrorOr;
using Microsoft.Extensions.Options;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Infrastructure.ExternalServices.AI.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StrikeDefender.Infrastructure.ExternalServices.AI.Providers
{
    public class GeminiProvider : IAiProvider
    {
        private readonly HttpClient _http;
        private readonly GeminiOptions _opt;

        public GeminiProvider(HttpClient http, IOptions<GeminiOptions> opt)
        {
            _http = http;
            _opt = opt.Value;
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

            var res = await _http.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json"),
                ct);

            if (!res.IsSuccessStatusCode)
                return Error.Failure(
                    code: "AI.GeminiError",
                    description: $"Gemini error {res.RequestMessage}");
          
            var str = await res.Content.ReadAsStringAsync(ct);

            using var doc = JsonDocument.Parse(str);

            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text!
                .Split('\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }
    }
}

