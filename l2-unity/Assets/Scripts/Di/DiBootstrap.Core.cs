using UnityEngine;
using VContainer;
using VContainer.Unity;

public static partial class DiBootstrap
{
    static AppLifetimeScope _ensuredApp;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RegisterStartup()
    {
        AppScopeStartup.Ensure = EnsureAppScope;
    }

    public static void ForgetAppScope(AppLifetimeScope scope)
    {
        if (ReferenceEquals(_ensuredApp, scope))
            _ensuredApp = null;
    }

    public static void EnsureAppScope(GameObject host = null)
    {
        // A destroyed scope compares equal to null and must not be reused.
        if (_ensuredApp == null)
            _ensuredApp = null;

        AppLifetimeScope existing = _ensuredApp;
        if (existing == null)
            existing = Object.FindFirstObjectByType<AppLifetimeScope>();
        if (existing == null && host != null)
            existing = host.AddComponent<AppLifetimeScope>();
        if (existing != null)
        {
            _ensuredApp = existing;
            if (existing.GetComponent<EventProcessor>() == null)
                existing.gameObject.AddComponent<EventProcessor>();
            EnsureFmodStudioListener(existing.gameObject);
            AudioSessionBootstrap.EnsureSession();
            return;
        }

        // App owns the main-thread apply pump. Not Menu/Networking/Game prefabs —
        // those are unloaded or duplicated and must not host EventProcessor.
        GameObject go = new GameObject("AppLifetimeScope");
        Object.DontDestroyOnLoad(go);
        go.AddComponent<EventProcessor>();
        _ensuredApp = go.AddComponent<AppLifetimeScope>();
        // LoginCameras live in additive l2_lobby; FMOD warns before that loads.
        EnsureFmodStudioListener(go);
        AudioSessionBootstrap.EnsureSession();
    }

    static void EnsureFmodStudioListener(GameObject host)
    {
        if (host == null)
            return;
        if (host.GetComponent<FMODUnity.StudioListener>() == null)
            host.AddComponent<FMODUnity.StudioListener>();
    }
}
