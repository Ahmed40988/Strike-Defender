using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Infrastructure.ExternalServices.AI.Configurations
{
    public class GeminiOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-2.5-flash-lite";
        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta";

        public double Temperature { get; set; } = 0.4;
        public int MaxOutputTokens { get; set; } = 300;
        public double TopP { get; set; } = 0.9;
    }
}
