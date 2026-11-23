using System;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using VContainer;

/// <summary>
/// Builds the real <see cref="GameLifetimeScope"/> the way <see cref="World.Awake"/> does.
/// The old graph was World → ClickManager → TargetManager → World.
/// </summary>
public sealed class GameLifetimeScopeCycleEditModeTests
{
    GameObject _root;

    [SetUp]
    public void SetUp()
    {
        _root = new GameObject("GameScopeCycleTest");
    }

    [TearDown]
    public void TearDown()
    {
        DestroyAll<GameLifetimeScope>();
        DestroyAll<AppLifetimeScope>();
        DestroyAll<NetSession>();
        if (_root != null)
            UnityEngine.Object.DestroyImmediate(_root);
        App.GameContainer = null;
        App.Container = null;
    }

    [Test]
    public void Game_scope_builds_without_a_click_target_cycle()
    {
        // EditMode does not run BeforeSceneLoad, so the play-mode startup hook is unset.
        // GameManager.Awake builds the app scope on this object the same way a player run does.
        Action<GameObject> previousEnsure = AppScopeStartup.Ensure;
        AppScopeStartup.Ensure = DiBootstrap.EnsureAppScope;
        var logs = new StringBuilder();
        Application.LogCallback onLog = (condition, stackTrace, type) =>
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
                logs.AppendLine(condition);
        };
        Application.logMessageReceived += onLog;
        try
        {
            Plant<SceneLoader>();
            Plant<LoginClient>();
            Plant<GameClient>();
            Plant<GameManager>();
            Plant<VisibilityManager>();
            Plant<EffectManager>();
            Plant<MoveAllCharacters>();
            Plant<NameplatesManager>();
            Plant<InputManager>();
            Plant<ClickManager>();
            Plant<TargetManager>();
            Plant<CameraController>();
            Plant<CharacterInfoWindow>();
            Plant<CharacterBuilder>();
            Plant<Geodata>();
            Plant<WorldCombat>();
            Plant<ObjectPoolManager>();
            Plant<GravityNpc>();
            Plant<DeadManager>();

            try
            {
                _root.AddComponent<World>();
            }
            catch (Exception ex)
            {
                if (MentionsCycle(ex))
                    Assert.Fail(ex + "\n" + logs);
                throw;
            }
        }
        finally
        {
            Application.logMessageReceived -= onLog;
            AppScopeStartup.Ensure = previousEnsure;
        }

        Assert.IsNotNull(
            App.GameContainer,
            "GameLifetimeScope did not publish App.GameContainer.\n" + logs);
        Assert.IsNotNull(App.GameContainer.Resolve<World>());
        Assert.IsNotNull(App.GameContainer.Resolve<ClickManager>());
        Assert.IsNotNull(App.GameContainer.Resolve<TargetManager>());
        ISpawnedEntities spawned = App.GameContainer.Resolve<ISpawnedEntities>();
        Assert.IsNotNull(spawned);
        Assert.IsNull(spawned.Find(1));
    }

    void Plant<T>() where T : Component
    {
        _root.AddComponent<T>();
    }

    static void DestroyAll<T>() where T : Component
    {
        T[] found = UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < found.Length; i++)
        {
            if (found[i] != null)
                UnityEngine.Object.DestroyImmediate(found[i].gameObject);
        }
    }

    static bool MentionsCycle(Exception ex)
    {
        for (Exception cursor = ex; cursor != null; cursor = cursor.InnerException)
        {
            if (cursor.Message != null &&
                cursor.Message.IndexOf("Circular dependency", StringComparison.Ordinal) >= 0)
                return true;
        }

        return false;
    }
}
