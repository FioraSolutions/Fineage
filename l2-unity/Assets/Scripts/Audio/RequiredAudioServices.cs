using System.Collections.Generic;
using Fineage.Networking;

/// <summary>
/// Required audio/music managers — log once per service+caller, no silent null fallbacks.
/// </summary>
public static class RequiredAudioServices
{
    static readonly HashSet<string> Logged = new HashSet<string>();

    public static AudioManager RequireAudio(string caller)
    {
        AudioManager audio = AudioPacketServices.Audio;
        if (audio != null)
            return audio;

        LogOnce("AudioManager", caller);
        return null;
    }

    public static MusicManager RequireMusic(string caller)
    {
        MusicManager music = MusicPacketServices.Music;
        if (music != null)
            return music;

        LogOnce("MusicManager", caller);
        return null;
    }

    static void LogOnce(string service, string caller)
    {
        string key = RequiredAudioServiceLog.FormatKey(service, caller);
        if (!Logged.Add(key))
            return;

        FineageLog.Error(
            FineageLogCategory.World,
            "Required " + service + " missing — caller=" + caller
            + ". AudioSessionBootstrap.EnsureSession() should run at app start.");
    }
}