/// <summary>
/// Resolves <c>SystemMessage</c> chat lines. Non-empty <paramref name="text"/> is shown as sent
/// (already localized on the server). Empty text looks up <c>sysmsg.&lt;id&gt;</c> in the client catalog.
/// </summary>
public static class NetSystemLines
{
    public static string Resolve(int id, string text, string[] parms)
    {
        if (!string.IsNullOrEmpty(text))
            return ApplyParams(text, parms);
        string key = "sysmsg." + id;
        string template = L10nRuntime.Get(key);
        return ApplyParams(template, parms);
    }

    public static string BuffRefusal(string code)
    {
        if (string.IsNullOrEmpty(code))
            return "";
        string key = "sysmsg.buff." + code;
        return L10nRuntime.Get(key);
    }

    static string ApplyParams(string template, string[] parms)
    {
        if (parms == null || parms.Length == 0 || string.IsNullOrEmpty(template))
            return template ?? "";
        string line = template;
        for (int i = 0; i < parms.Length; i++)
            line = line.Replace("{" + i + "}", parms[i] ?? "");
        return line;
    }
}