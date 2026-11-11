using System;

/// <summary>
/// Cycle-safe UI click/open sounds. ACS binds this to AudioManager via AudioPacketServices.
/// </summary>
public static class UiAudio
{
    public static Action<string> PlayUISound;

    public static void Play(string soundName)
    {
        PlayUISound?.Invoke(soundName);
    }
}
