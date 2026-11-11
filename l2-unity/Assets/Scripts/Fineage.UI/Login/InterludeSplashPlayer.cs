using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Retail Interlude startup splash. Frames are systextures/sp_32b_01..11
/// (Lineage2Ver121), flattened from the client extract. Music is music/intro.ogg.
/// Off unless <c>-fineageSplash</c>, <c>FINEAGE_SPLASH=1</c>, or the proof shot
/// file exists. Normal play goes straight to the login window.
/// </summary>
public sealed partial class InterludeSplashPlayer : MonoBehaviour
{
    public const string ShotPathFile = "/tmp/fineage-interlude-splash.outpath";

    const float FrameSeconds = 2.25f;
    const float SkipLockSeconds = 1.5f;

    static readonly string[] Frames =
    {
        "Data/UI/Splash/Interlude/sp_32b_01",
        "Data/UI/Splash/Interlude/sp_32b_02",
        "Data/UI/Splash/Interlude/sp_32b_03",
        "Data/UI/Splash/Interlude/sp_32b_04",
        "Data/UI/Splash/Interlude/sp_32b_05",
        "Data/UI/Splash/Interlude/sp_32b_06",
        "Data/UI/Splash/Interlude/sp_32b_07",
        "Data/UI/Splash/Interlude/sp_32b_08",
        "Data/UI/Splash/Interlude/sp_32b_09",
        "Data/UI/Splash/Interlude/sp_32b_10",
        "Data/UI/Splash/Interlude/sp_32b_11",
    };

    static bool _played;

    VisualElement _overlay;
    VisualElement _frame;
    Texture2D[] _textures;
    AudioSource _music;
    bool _dismissed;
    float _shownAt;

    public static void Present(VisualElement documentRoot)
    {
        if (!ShouldPlay() || _played || documentRoot == null)
            return;
        if (documentRoot.Q("InterludeSplash") != null)
            return;

        var textures = new Texture2D[Frames.Length];
        int count = 0;
        for (int i = 0; i < Frames.Length; i++)
        {
            textures[i] = Resources.Load<Texture2D>(Frames[i]);
            if (textures[i] != null)
                count++;
        }

        if (count == 0)
        {
            Debug.LogWarning("[InterludeSplash] no frames under Resources/Data/UI/Splash/Interlude");
            return;
        }

        _played = true;
        var host = new GameObject("InterludeSplash");
        var player = host.AddComponent<InterludeSplashPlayer>();
        player.Begin(documentRoot, textures);
    }

    /// <summary>Proof shots and <c>-fineageSplash</c> still play the intro. Everyone else skips it.</summary>
}
