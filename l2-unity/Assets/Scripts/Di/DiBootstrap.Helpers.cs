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
            RequireGameScopeReady(existing);
            App.GameContainer = existing.Container;
            return;
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
        RequireGameScopeReady(built);
        App.GameContainer = built.Container;
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

    static void RequireGameScopeReady(GameLifetimeScope scope)
    {
        if (scope == null || scope.Container == null)
        {
            throw new System.InvalidOperationException(
                "[DiBootstrap] GameLifetimeScope did not publish a container — LifetimeScope.Awake did not finish Build, so the game scope was not ready.");
        }

        try
        {
            if (scope.Container.Resolve<World>() == null)
            {
                throw new System.InvalidOperationException(
                    "[DiBootstrap] GameLifetimeScope World resolve returned null, so the game scope was not ready.");
            }

            scope.Container.Resolve<WorldPacketApply>();
            scope.Container.Resolve<MessagePacketApply>();
            scope.Container.Resolve<IWorldDomainSink>();
            scope.Container.Resolve<EntityActionMachine>();
        }
        catch (System.InvalidOperationException)
        {
            throw;
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(
                "[DiBootstrap] GameLifetimeScope resolve failed — the game scope was not ready. " + ex);
            throw;
        }
    }
}
