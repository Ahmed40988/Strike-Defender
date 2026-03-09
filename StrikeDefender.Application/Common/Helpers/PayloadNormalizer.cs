using System.Text.RegularExpressions;

namespace StrikeDefender.Application.Common.Helpers;

public static class PayloadNormalizer
{
    public static string Normalize(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
            return string.Empty;

        payload = payload.Trim();

        // remove wrapping quotes
        payload = payload.Trim('"');

        // remove trailing comma
        payload = payload.TrimEnd(',');

        // collapse spaces
        payload = Regex.Replace(payload, @"\s+", " ");

        // normalize case
        payload = payload.ToLowerInvariant();

        return payload;
    }
}