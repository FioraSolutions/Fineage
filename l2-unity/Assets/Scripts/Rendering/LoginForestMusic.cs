using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Interlude login BGM — retail <c>music/intro.ogg</c> (FMOD <c>event:/Music/l2_lobby/intro</c>).
/// Same asset as splash; keeps looping on the credentials screen when splash is skipped.
/// </summary>
public static class LoginForestMusic
{
    public const string FmodEventPath = "event:/Music/l2_lobby/intro";
    public const string ResourcesClipPath = "Data/Audio/intro";

    static EventInstance _fmod;
    static bool _fmodLive;
    static AudioSource _unitySource;
    static GameObject _unityHost;

    public static void Play()
    {
        Stop();

        if (TryPlayFmod())
            return;

        AudioClip clip = Resources.Load<AudioClip>(ResourcesClipPath);
        if (clip == null)
        {
            Debug.LogWarning("[LoginForest] intro music missing (FMOD " + FmodEventPath
                + " and Resources/" + ResourcesClipPath + ")");
            return;
        }

        _unityHost = new GameObject("LoginForestMusic");
        UnityEngine.Object.DontDestroyOnLoad(_unityHost);
        _unitySource = _unityHost.AddComponent<AudioSource>();
        _unitySource.clip = clip;
        _unitySource.loop = true;
        _unitySource.playOnAwake = false;
        _unitySource.spatialBlend = 0f;
        _unitySource.volume = GameSettings.Music;
        _unitySource.Play();
        Debug.Log("[LoginForest] playing intro via AudioClip loop=" + clip.length.ToString("0.0") + "s");
    }

    public static void Stop()
    {
        if (_fmodLive)
        {
            try
            {
                _fmod.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _fmod.release();
            }
            catch (Exception)
            {
                /* bank may already be torn down */
            }

            _fmodLive = false;
        }

        if (_unitySource != null)
        {
            _unitySource.Stop();
            _unitySource = null;
        }

        if (_unityHost != null)
        {
            UnityEngine.Object.Destroy(_unityHost);
            _unityHost = null;
        }
    }

    public static void TickVolume()
    {
        float vol = GameSettings.Music;
        if (_fmodLive)
        {
            try
            {
                _fmod.setVolume(vol);
            }
            catch (Exception)
            {
                /* ignore */
            }
        }

        if (_unitySource != null)
            _unitySource.volume = vol;
    }

    static bool TryPlayFmod()
    {
        try
        {
            EventReference er = RuntimeManager.PathToEventReference(FmodEventPath);
            if (er.IsNull)
                return false;

            _fmod = RuntimeManager.CreateInstance(er);
            _fmod.setVolume(GameSettings.Music);
            _fmod.start();
            _fmodLive = true;
            Debug.Log("[LoginForest] playing " + FmodEventPath);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[LoginForest] FMOD intro failed: " + ex.Message);
            return false;
        }
    }
}
