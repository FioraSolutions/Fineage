using System;
using System.Globalization;

/// <summary>
/// One line per world/UI/music play, refusal, or voice steal.
/// On when FINEAGE_AUDIO_TRACE=1 or <see cref="FineageLog.AudioTraceEnabled"/>.
/// </summary>
public static class AudioPlayTrace
{
    public static bool? EnabledOverride;

    public static bool IsEnabled
    {
        get
        {
            if (EnabledOverride.HasValue)
                return EnabledOverride.Value;
            if (FineageLog.AudioTraceEnabled)
                return true;
            string env = Environment.GetEnvironmentVariable("FINEAGE_AUDIO_TRACE");
            return env == "1" || string.Equals(env, "true", StringComparison.OrdinalIgnoreCase);
        }
    }

    public static void Write(
        string action,
        float timeSeconds,
        string clipId,
        string source,
        string ownerName,
        int ownerId,
        float distanceMeters,
        float radiusUu,
        float volume,
        int voiceSlot,
        string reason)
    {
        if (!IsEnabled)
            return;
        FineageLog.Info(FineageLogCategory.Audio, Format(
            action, timeSeconds, clipId, source, ownerName, ownerId,
            distanceMeters, radiusUu, volume, voiceSlot, reason));
    }

    public static string Format(
        string action,
        float timeSeconds,
        string clipId,
        string source,
        string ownerName,
        int ownerId,
        float distanceMeters,
        float radiusUu,
        float volume,
        int voiceSlot,
        string reason)
    {
        CultureInfo c = CultureInfo.InvariantCulture;
        return (action ?? "play")
            + " t=" + timeSeconds.ToString("0.000", c)
            + " clip=\"" + (clipId ?? "") + "\""
            + " source=" + (string.IsNullOrEmpty(source) ? "AudioService" : source)
            + " owner=\"" + (ownerName ?? "") + "\""
            + " id=" + ownerId.ToString(c)
            + " dist=" + distanceMeters.ToString("0.00", c)
            + " radiusUu=" + radiusUu.ToString("0.###", c)
            + " vol=" + volume.ToString("0.00", c)
            + " slot=" + voiceSlot.ToString(c)
            + " reason=" + (reason ?? "");
    }

    public static void ClearForTests() => EnabledOverride = null;
}
