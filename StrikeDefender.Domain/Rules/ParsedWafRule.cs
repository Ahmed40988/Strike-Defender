using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Domain.Rules
{
    public class ParsedWafRule
    {
        public Guid Id { get; private set; }
        public string Target { get; private set; } = string.Empty;     // ARGS
        public string Operator { get; private set; } = string.Empty;  // @rx
        public string Pattern { get; private set; } = string.Empty;

        public int RuleId { get; private set; }
        public int Phase { get; private set; }
        public string Action { get; private set; } = string.Empty;
        public int StatusCode { get; private set; }
        public string Message { get; private set; } = string.Empty;

        public ParsedWafRule(
      string target,
      string @operator,
      string pattern,
      int ruleId,
      int phase,
      string action,
      int statusCode,
      string message)
        {
            Id = Guid.NewGuid();
            Target = target;
            Operator = @operator;
            Pattern = pattern;
            RuleId = ruleId;
            Phase = phase;
            Action = action;
            StatusCode = statusCode;
            Message = message;
        }

    }
}
