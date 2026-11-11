using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// ACS façade over <see cref="AppContainers"/> plus GameScope inject (#131).
/// </summary>
public static class App
{
    public static IObjectResolver Container
    {
        get { return AppContainers.Container; }
        set { AppContainers.Container = value; }
    }

    public static IObjectResolver GameContainer
    {
        get { return AppContainers.GameContainer; }
        set { AppContainers.GameContainer = value; }
    }

    public static bool HasContainer => AppContainers.HasContainer;

    public static T Resolve<T>() => AppContainers.Resolve<T>();

    public static void InjectGameObject(GameObject go)
    {
        if (go == null)
            return;
        if (AppContainers.GameContainer == null)
        {
            GameLifetimeScope scope = Object.FindFirstObjectByType<GameLifetimeScope>(FindObjectsInactive.Include);
            if (scope != null)
                AppContainers.GameContainer = scope.Container;
        }
        if (AppContainers.GameContainer != null)
            AppContainers.GameContainer.InjectGameObject(go);
    }
}
