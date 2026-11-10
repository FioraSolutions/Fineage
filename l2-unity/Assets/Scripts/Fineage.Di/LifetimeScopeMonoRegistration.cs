using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Shared MonoBehaviour registration helpers for App/Game/Login LifetimeScopes (#131).
/// ACS-specific PacketServices fallbacks stay in the calling scope (e.g. GameLifetimeScope).
/// </summary>
public static class LifetimeScopeMonoRegistration
{
    public static void RegisterIfFound<T>(IContainerBuilder builder) where T : MonoBehaviour
    {
        T component = UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (component != null)
            builder.RegisterComponent(component);
    }

    /// <summary>
    /// Prefer a component on <paramref name="host"/>, else first scene match (AppLifetimeScope style).
    /// </summary>
    public static void RegisterIfFoundOnHost<T>(MonoBehaviour host, IContainerBuilder builder)
        where T : MonoBehaviour
    {
        if (host == null)
        {
            RegisterIfFound<T>(builder);
            return;
        }

        T local = host.GetComponent<T>();
        if (local == null)
            local = UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (local != null)
            builder.RegisterComponent(local);
    }

    /// <summary>
    /// Registers a type that may appear after the scope is built (prefab-instantiated MBs).
    /// Optional <paramref name="resolveFallback"/> covers PacketServices / Instance bridges.
    /// </summary>
    public static void RegisterLateBound<T>(IContainerBuilder builder, Func<T> resolveFallback = null)
        where T : MonoBehaviour
    {
        T inScene = UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (inScene != null)
        {
            builder.RegisterComponent(inScene);
            builder.RegisterInstance<ILateBound<T>>(new LateBoundMono<T>(() => inScene));
            return;
        }

        Func<T> resolve = () => ResolveLiveMono(resolveFallback);
        builder.Register<T>(_ => resolve(), Lifetime.Singleton);
        builder.RegisterInstance<ILateBound<T>>(new LateBoundMono<T>(resolve));
    }

    /// <summary>
    /// Scene component when present; otherwise a live singleton via Find + optional fallback.
    /// </summary>
    public static void RegisterLiveMono<T>(IContainerBuilder builder, Func<T> resolveFallback = null)
        where T : MonoBehaviour
    {
        T inScene = UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (inScene != null)
        {
            builder.RegisterComponent(inScene);
            return;
        }

        builder.Register<T>(_ => ResolveLiveMono(resolveFallback), Lifetime.Singleton);
    }

    static T ResolveLiveMono<T>(Func<T> resolveFallback) where T : MonoBehaviour
    {
        T found = UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (found != null)
            return found;
        return resolveFallback != null ? resolveFallback() : null;
    }
}
