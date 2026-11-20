/// <summary>Login intro BGM — routed through <see cref="AudioService"/> (Music category).</summary>
public static class LoginForestMusic
{
    public const string FmodEventPath = AudioMusicIds.LoginIntroFmodPath;
    public const string ResourcesClipPath = AudioMusicIds.LoginIntroResourcesPath;

    public static void Play() => AudioPacketServices.Service?.PlayLoginIntro(loop: true);

    public static void PlaySplashOnce() => AudioPacketServices.Service?.PlayLoginIntro(loop: false);

    public static void Stop() => AudioPacketServices.Service?.StopLoginIntro();

    public static void TickVolume() { }
}