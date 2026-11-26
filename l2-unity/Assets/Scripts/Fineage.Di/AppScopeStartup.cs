using System;
using UnityEngine;

/// <summary>
/// App-scope startup hook (#131 S34).
/// <c>DiBootstrap</c> assigns <see cref="Ensure"/> before the first scene.
/// <c>GameManager</c> does not name <c>AppLifetimeScope</c>.
/// </summary>
public static class AppScopeStartup
{
    public static Action<GameObject> Ensure;

    public static void Run(GameObject host)
    {
        if (Ensure == null)
        {
            Debug.LogError("[AppScopeStartup] Ensure was not set — DiBootstrap.RegisterStartup did not run, so AppLifetimeScope was not created.");
            return;
        }

        Ensure(host);
    }
}
