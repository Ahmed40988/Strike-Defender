using ErrorOr;
using Microsoft.Extensions.Logging;
using StrikeDefender.Application.Common.Interfaces;
using StrikeDefender.Infrastructure.ExternalServices.AI.Helpers;

namespace StrikeDefender.Infrastructure.ExternalServices.AI.Services
{
    public class AiEngineService: IAiEngineService
    {
        private readonly IAiProvider _provider;
        private readonly AiRateLimiter _rateLimiter;
        private readonly AiUsageTracker _usage;
        private readonly ILogger<AiEngineService> _logger;

        public AiEngineService(
            IAiProvider provider,
            AiRateLimiter rateLimiter,
            AiUsageTracker usage,
            ILogger<AiEngineService> logger)
        {
            _provider = provider;
            _rateLimiter = rateLimiter;
            _usage = usage;
            _logger = logger;
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

    private async Task<ErrorOr<List<string>>> ExecuteWithRetry(
            string prompt,
            CancellationToken ct)
        {
            var retries = 3;

            for (int i = 0; i < retries; i++)
            {
                try
                {
                    return await _provider.SendAsync(prompt, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        "AI try {Try} failed: {Msg}", i + 1, ex.Message);

                    await Task.Delay(1500, ct);
                }
            }

          return Error.Failure(
                    code: "AI.RetryLimit",
                    description: "AI request failed after multiple attempts.");
        }

    }

}

