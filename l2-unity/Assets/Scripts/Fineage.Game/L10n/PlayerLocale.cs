using System.Text.RegularExpressions;

/// <summary>
/// Player language tag. Default is <see cref="Default"/> (<c>en</c>).
/// Matches the backend <c>Fineage.Game.L10n.PlayerLocale</c> rules.
/// </summary>
public static class PlayerLocale
{
    public const string Default = "en";

    static readonly Regex TagPattern = new Regex(
        "^[a-z]{2}(-[a-z0-9]{2,8})?$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public static bool TryNormalize(string raw, out string locale)
    {
        locale = Default;
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        string text = raw.Trim().ToLowerInvariant();
        if (!TagPattern.IsMatch(text))
            return false;

        locale = text;
        return true;
    }

    public static string Normalize(string raw) =>
        TryNormalize(raw, out string locale) ? locale : Default;

    public static bool IsDefault(string locale) =>
        string.Equals(Normalize(locale), Default, System.StringComparison.Ordinal);
}