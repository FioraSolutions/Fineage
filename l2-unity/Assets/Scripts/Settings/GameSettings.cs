using UnityEngine;

/// <summary>Minimal Interlude-like options: audio volumes/mute + resolution/window mode. Stored in PlayerPrefs.</summary>
public static class GameSettings
{
    const string KMaster = "opt.vol.master", KMusic = "opt.vol.music", KSfx = "opt.vol.sfx", KAmb = "opt.vol.ambient", KMute = "opt.vol.mute";
    const string KResW = "opt.res.w", KResH = "opt.res.h", KFull = "opt.res.full";

    public static float Master { get => PlayerPrefs.GetFloat(KMaster, 0.5f); set { PlayerPrefs.SetFloat(KMaster, Mathf.Clamp01(value)); PlayerPrefs.Save(); } }
    public static float Music { get => PlayerPrefs.GetFloat(KMusic, 0.6f); set { PlayerPrefs.SetFloat(KMusic, Mathf.Clamp01(value)); PlayerPrefs.Save(); } }
    public static float Sfx { get => PlayerPrefs.GetFloat(KSfx, 0.7f); set { PlayerPrefs.SetFloat(KSfx, Mathf.Clamp01(value)); PlayerPrefs.Save(); } }
    public static float Ambient { get => PlayerPrefs.GetFloat(KAmb, 0.6f); set { PlayerPrefs.SetFloat(KAmb, Mathf.Clamp01(value)); PlayerPrefs.Save(); } }
    public static bool Mute { get => PlayerPrefs.GetInt(KMute, 0) == 1; set { PlayerPrefs.SetInt(KMute, value ? 1 : 0); PlayerPrefs.Save(); } }

    public static readonly Vector2Int[] Resolutions = {
        new Vector2Int(1024, 768), new Vector2Int(1280, 720), new Vector2Int(1280, 800), new Vector2Int(1280, 1024),
        new Vector2Int(1366, 768), new Vector2Int(1440, 900), new Vector2Int(1600, 900), new Vector2Int(1680, 1050),
        new Vector2Int(1920, 1080), new Vector2Int(2560, 1440),
    };

    public static bool Fullscreen => PlayerPrefs.GetInt(KFull, Screen.fullScreen ? 1 : 0) == 1;

    public static void ApplyResolution(int w, int h, bool fullscreen)
    {
        PlayerPrefs.SetInt(KResW, w); PlayerPrefs.SetInt(KResH, h); PlayerPrefs.SetInt(KFull, fullscreen ? 1 : 0); PlayerPrefs.Save();
        Screen.SetResolution(w, h, fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void ApplySavedResolution()
    {
#if !UNITY_EDITOR
        if (PlayerPrefs.HasKey(KResW))
            Screen.SetResolution(PlayerPrefs.GetInt(KResW), PlayerPrefs.GetInt(KResH),
                PlayerPrefs.GetInt(KFull, 0) == 1 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
#endif
    }
}
