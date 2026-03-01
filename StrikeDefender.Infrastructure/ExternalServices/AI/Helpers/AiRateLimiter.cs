using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace StrikeDefender.Infrastructure.ExternalServices.AI.Helpers
{
    public class AiRateLimiter
    {
        private readonly RateLimiter _rateLimiter;

        public AiRateLimiter()
        {
            _rateLimiter = new SlidingWindowRateLimiter(
                new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 10, // max requests
                    Window = TimeSpan.FromSeconds(60),
                    SegmentsPerWindow = 2,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 50
                });
        }

        public async Task<bool> WaitAsync(CancellationToken ct)
        {
            var lease = await _rateLimiter.AcquireAsync(1, ct);
            return lease.IsAcquired;
        }
    }
}
