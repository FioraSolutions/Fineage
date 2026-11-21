using UnityEngine;

/// <summary>
/// One DDOL <c>Sound.prefab</c> for the whole session (char select, world, zone unload).
/// Menu still carries a copy for editor play; bootstrap ensures managers exist if Menu never loaded.
/// </summary>
public static class AudioSessionBootstrap
{
    static bool _bootstrapping;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void BootstrapBeforeFirstScene() => EnsureSession();

    public static void EnsureSession()
    {
        if (!Application.isPlaying)
            return;

        if (!AudioSessionRules.ShouldCreatePrefab(HasAudioManager(), HasMusicManager()))
            return;

        if (_bootstrapping)
            return;

        _bootstrapping = true;
        try
        {
            if (!AudioSessionRules.ShouldCreatePrefab(HasAudioManager(), HasMusicManager()))
                return;

            GameObject prefab = Resources.Load<GameObject>(AudioSessionRules.SoundPrefabResourcesPath);
            if (prefab == null)
            {
                FineageLog.Error(
                    FineageLogCategory.World,
                    "AudioSessionBootstrap: missing Resources/" + AudioSessionRules.SoundPrefabResourcesPath);
                return;
            }

            GameObject session = Object.Instantiate(prefab);
            session.name = "Sound (session)";
            Object.DontDestroyOnLoad(session);
            FineageLog.Info(FineageLogCategory.World, "AudioSessionBootstrap: instantiated persistent Sound session");
        }
        finally
        {
            _bootstrapping = false;
        }
    }

    static bool HasAudioManager() =>
        Object.FindFirstObjectByType<AudioManager>(FindObjectsInactive.Include) != null;

    static bool HasMusicManager() =>
        Object.FindFirstObjectByType<MusicManager>(FindObjectsInactive.Include) != null;
}
