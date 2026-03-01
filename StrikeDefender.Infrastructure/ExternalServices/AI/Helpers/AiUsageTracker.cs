using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Infrastructure.ExternalServices.AI.Helpers
{
    public class AiUsageTracker
    {
        private int _requestsToday = 0;
        private readonly int _dailyLimit = 1200;

        public bool CanRequest()
        {
            return _requestsToday < _dailyLimit;
        }

        public void Increment()
        {
            Interlocked.Increment(ref _requestsToday);
        }
    }
}
