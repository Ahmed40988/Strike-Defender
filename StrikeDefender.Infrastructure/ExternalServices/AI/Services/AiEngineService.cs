using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Application.Rules.RuleDTO;
using StrikeDefender.Domain.Attacks;
using StrikeDefender.Domain.Rules;
using StrikeDefender.Infrastructure.ExternalServices.AI.Helpers;
using System.Text.RegularExpressions;

namespace StrikeDefender.Infrastructure.ExternalServices.AI.Services
{
    public class AiEngineService : IAiEngineService
    {
        private readonly List<IAiProvider> _providers;
        private readonly AiRateLimiter _rateLimiter;
        private readonly AiUsageTracker _usage;
        private readonly ILogger<AiEngineService> _logger;
        private static int _ruleId;
        private static bool _initialized = false;
        private readonly IGenericRepository<ParsedWafRule> _rulesRepository;


        public AiEngineService(
       IEnumerable<IAiProvider> providers,
       AiRateLimiter rateLimiter,
       AiUsageTracker usage,
       ILogger<AiEngineService> logger, IGenericRepository<ParsedWafRule> rulesRepository)
        {
            _providers = providers
                .OrderBy(p => p.Name == "Gemini" ? 0 :
                              p.Name == "OpenRouter" ? 1 :
                              2)
                .ToList();

            _rateLimiter = rateLimiter;
            _usage = usage;
            _logger = logger;
            _rulesRepository = rulesRepository;
        }

        public async Task<ErrorOr<List<string>>> GenerateAttacksAsync(
            string prompt,
            CancellationToken ct = default)
        {
            // daily limit
            if (!_usage.CanRequest())
                return Error.Failure(
                    code: "AI.DailyLimit",
                    description: "AI daily limit reached. Try again tomorrow.");

            // rate limit
            var allowed = await _rateLimiter.WaitAsync(ct);

            if (!allowed)
                return Error.Failure(
                    code: "AI.RateLimit",
                    description: "Too many AI requests. Please wait few seconds.");

            try
            {
                var result = await ExecuteWithRetry(prompt, ct);

                // Check if the result is an error or if the value is empty
                if (result.IsError || result.Value.Count == 0)
                    return Error.Failure(
                        code: "AI.Empty",
                        description: "AI returned empty response.");

                _usage.Increment();

                return result; // success auto wrapped
            }
            catch (HttpRequestException)
            {
                return Error.Failure(
                    code: "AI.Connection",
                    description: "Failed to connect to AI provider.");
            }
            catch (TaskCanceledException)
            {
                return Error.Failure(
                    code: "AI.Timeout",
                    description: "AI request timeout.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI Engine failed");

                return Error.Failure(
                    code: "AI.Unknown",
                    description: ex.Message);
            }
        }


        public async Task<ErrorOr<List<string>>> GenerateRulesAsync(
    List<AttackPayloadDto> attacks,
    string prompt,
    CancellationToken ct = default)
        {
            if (attacks is null || !attacks.Any())
                return Error.Validation(
                    code: "AI.Attacks.Empty",
                    description: "Attacks list cannot be empty.");

            await EnsureInitializedAsync();
            // daily limit
            if (!_usage.CanRequest())
                return Error.Failure(
                    code: "AI.DailyLimit",
                    description: "AI daily limit reached. Try again tomorrow.");

            // rate limit
            var allowed = await _rateLimiter.WaitAsync(ct);

            if (!allowed)
                return Error.Failure(
                    code: "AI.RateLimit",
                    description: "Too many AI requests. Please wait few seconds.");

            try
            {
                var fullPrompt = BuildRulesPrompt(attacks, prompt);

                var result = await ExecuteWithRetry(fullPrompt, ct);

                if (result.IsError || result.Value.Count == 0)
                    return Error.Failure(
                        code: "AI.Empty",
                        description: "AI returned empty rules.");

      
                var cleanedRules = result.Value
                    .Select(NormalizeAndInjectId)
                    .ToList();

                _usage.Increment();

                return cleanedRules;
            }
            catch (HttpRequestException)
            {
                return Error.Failure(
                    code: "AI.Connection",
                    description: "Failed to connect to AI provider.");
            }
            catch (TaskCanceledException)
            {
                return Error.Failure(
                    code: "AI.Timeout",
                    description: "AI request timeout.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI Rule Generation failed");

                return Error.Failure(
                    code: "AI.Unknown",
                    description: ex.Message);
            }
        }
        private async Task<ErrorOr<List<string>>> ExecuteWithRetry(
      string prompt,
      CancellationToken ct)
        {
            foreach (var provider in _providers)
            {
                var retries = 2;

                for (int i = 0; i < retries; i++)
                {
                    try
                    {
                        _logger.LogInformation("Trying provider: {Provider}", provider.Name);

                        var result = await provider.SendAsync(prompt, ct);

                        if (!result.IsError && result.Value.Count > 0)
                        {
                            _logger.LogInformation("Success from: {Provider}", provider.Name);
                            return result;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            "Provider {Provider} try {Try} failed: {Msg}",
                            provider.Name,
                            i + 1,
                            ex.Message);
                    }

                    await Task.Delay(1000, ct);
                }

                _logger.LogWarning("Provider {Provider} exhausted, switching...", provider.Name);
            }

            return Error.Failure(
                code: "AI.AllProvidersFailed",
                description: "All AI providers failed.");
        }



        private string BuildRulesPrompt(List<AttackPayloadDto> attacks, string basePrompt)
        {
            var attacksText = string.Join("\n", attacks.Select(a => a.Payload));

            return $@"
                {basePrompt}

            Generate ModSecurity WAF rules for the following attacks:

            {attacksText}

            Rules format STRICT:
            - One rule per line
            - No extra spaces
            - Format EXACTLY like:
            SecRule ARGS ""@rx pattern"" ""id:xxxx,phase:2,deny,status:403,msg:'desc'""
            ";
                    }

        private string NormalizeAndInjectId(string rule)
        {
            rule = rule.Trim();

            var newId = Interlocked.Increment(ref _ruleId);

            return Regex.Replace(rule, @"id:\d+", $"id:{newId}");
        }

        private async Task EnsureInitializedAsync()
        {
            if (_initialized)
                return;

            var hasAny = await _rulesRepository.Query().AnyAsync();

            var maxId = hasAny
                ? await _rulesRepository.Query().MaxAsync(r => r.RuleId)
                : 1000000;

            _ruleId = maxId;
            _initialized = true;
        }

    }

}