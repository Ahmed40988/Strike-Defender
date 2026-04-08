using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Infrastructure.ExternalServices.AI.Configurations
{
    public class GroqOptions
    {
        public string BaseUrl { get; set; } = "https://api.groq.com/openai/v1/chat/completions";
        public string ApiKey { get; set; } = default!;
        public string Model { get; set; } = "llama-3.3-70b-versatile";
    }
}
