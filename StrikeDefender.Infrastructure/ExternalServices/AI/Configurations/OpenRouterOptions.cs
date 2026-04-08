using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Infrastructure.ExternalServices.AI.Configurations
{
    public class OpenRouterOptions
    {
        public string BaseUrl { get; set; } = "https://openrouter.ai/api/v1/chat/completions";
        public string ApiKey { get; set; } = default!;
        public string Model { get; set; } = "openai/gpt-4o-mini";
    }
}
