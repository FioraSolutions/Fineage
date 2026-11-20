/// <summary>Login intro BGM — <see cref="AudioSoundCatalog.IdLoginIntro"/> (FMOD only).</summary>
public static class LoginForestMusic
{
    public const string FmodEventPath = AudioMusicIds.LoginIntroFmodPath;
    public const string CatalogId = AudioSoundCatalog.IdLoginIntro;

    public static void Play() => AudioPacketServices.Service?.PlayLoginIntro(loop: true);

    public static void PlaySplashOnce() => AudioPacketServices.Service?.PlayLoginIntro(loop: false);

    public static void Stop() => AudioPacketServices.Service?.StopLoginIntro();

    public static void TickVolume() { }
}