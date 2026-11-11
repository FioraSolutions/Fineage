using UnityEngine;
using VContainer;
using VContainer.Unity;

public static partial class DiBootstrap
{
    public static void EnsureAppScope()
    {
        AppLifetimeScope existing = Object.FindFirstObjectByType<AppLifetimeScope>();
        if (existing != null)
        {
            if (existing.GetComponent<EventProcessor>() == null)
                existing.gameObject.AddComponent<EventProcessor>();
            EnsureFmodStudioListener(existing.gameObject);
            return;
        }

        // App owns the main-thread apply pump. Not Menu/Networking/Game prefabs —
        // those are unloaded or duplicated and must not host EventProcessor.
        GameObject go = new GameObject("AppLifetimeScope");
        Object.DontDestroyOnLoad(go);
        go.AddComponent<EventProcessor>();
        go.AddComponent<AppLifetimeScope>();
        // LoginCameras live in additive l2_lobby; FMOD warns before that loads.
        EnsureFmodStudioListener(go);
    }

    static void EnsureFmodStudioListener(GameObject host)
    {
        if (host == null)
            return;
        if (host.GetComponent<FMODUnity.StudioListener>() == null)
            host.AddComponent<FMODUnity.StudioListener>();
    }
}
