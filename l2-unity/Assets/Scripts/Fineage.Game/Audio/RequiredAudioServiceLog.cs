/// <summary>
/// One-shot log keys when a required audio service is missing (#session bootstrap).
/// </summary>
public static class RequiredAudioServiceLog
{
    public static string FormatKey(string service, string caller) =>
        service + "@" + caller;
}