using UnityEngine;
using VContainer;
using VContainer.Unity;

public static partial class DiBootstrap
{
    public static void EnsureGameScope(World world = null)
    {
        // EditMode Find misses a component inside its own Awake. The owner passes itself.
        if (world == null)
            world = Object.FindFirstObjectByType<World>(FindObjectsInactive.Include);
        GameLifetimeScope existing = Object.FindFirstObjectByType<GameLifetimeScope>(FindObjectsInactive.Include);

        if (existing != null)
        {
            if (IsGameScopeReady(existing))
            {
                App.GameContainer = existing.Container;
                return;
            }

            Object.DestroyImmediate(existing.gameObject);
            App.GameContainer = null;
        }

        if (world == null)
            return;

        EnsureAppScope();
        AppLifetimeScope app = _ensuredApp;
        if (app == null)
        {
            throw new System.InvalidOperationException(
                "[DiBootstrap] AppLifetimeScope missing — EnsureAppScope did not leave an app scope, so the game scope was not built.");
        }

        GameObject go = new GameObject("GameLifetimeScope");
        using (LifetimeScope.EnqueueParent(app))
            go.AddComponent<GameLifetimeScope>();

        GameLifetimeScope built = go.GetComponent<GameLifetimeScope>();
        if (!IsGameScopeReady(built))
        {
            Object.DestroyImmediate(go);
            App.GameContainer = null;
        }
    }

    public static void EnsureLoginScope()
    {
        if (Object.FindFirstObjectByType<LoginLifetimeScope>() != null)
            return;

        EnsureAppScope();
        if (!App.HasContainer)
        {
            Debug.LogError("[DiBootstrap] EnsureLoginScope: AppLifetimeScope failed to build.");
            return;
        }

        AppLifetimeScope app = Object.FindFirstObjectByType<AppLifetimeScope>();
        L2LoginUI ui = Object.FindFirstObjectByType<L2LoginUI>(FindObjectsInactive.Include);
        GameObject go = new GameObject("LoginLifetimeScope");
        if (ui != null)
            go.transform.SetParent(ui.transform);
        using (LifetimeScope.EnqueueParent(app))
            go.AddComponent<LoginLifetimeScope>();
    }

    private static bool IsGameScopeReady(GameLifetimeScope scope)
    {
        if (scope == null || scope.Container == null)
            return false;

        try
        {
            if (scope.Container.Resolve<World>() == null)
                return false;
            scope.Container.Resolve<WorldPacketApply>();
            scope.Container.Resolve<MessagePacketApply>();
            scope.Container.Resolve<IWorldDomainSink>();
            scope.Container.Resolve<EntityActionMachine>();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
