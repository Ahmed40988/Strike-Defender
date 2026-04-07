using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StrikeDefender.Domain.Rules
{
    public static class WafRuleParser
    {
        public static ParsedWafRule? TryParse(string raw)
        {
            try
            {
                // مثال:
                // SecRule ARGS "@rx (?i)(union select|or 1=1)" \
                // "id:1000001,phase:2,deny,status:403,msg:'SQLi attempt'"

                var mainPattern = @"SecRule\s+(\w+)\s+""(@\w+)\s+(.+?)""";
                var actionPattern = @"id:(\d+),phase:(\d+),(\w+),status:(\d+),msg:'(.+)'";

                var mainMatch = Regex.Match(raw, mainPattern);
                var actionMatch = Regex.Match(raw, actionPattern);

                if (!mainMatch.Success || !actionMatch.Success)
                    return null;
                return new ParsedWafRule(
                    target: mainMatch.Groups[1].Value,
                    @operator: mainMatch.Groups[2].Value,
                    pattern: mainMatch.Groups[3].Value,
                    ruleId: int.Parse(actionMatch.Groups[1].Value),
                    phase: int.Parse(actionMatch.Groups[2].Value),
                    action: actionMatch.Groups[3].Value,
                    statusCode: int.Parse(actionMatch.Groups[4].Value),
                    message: actionMatch.Groups[5].Value
                );
            }
            catch
            {
                return null; // ❗ fail safe
            }
        }
    }
    }

