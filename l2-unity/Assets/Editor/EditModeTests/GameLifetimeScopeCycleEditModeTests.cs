using System;
using NUnit.Framework;
using UnityEngine;
using VContainer;

/// <summary>
/// Builds the game container through <see cref="GameLifetimeScope.Install"/>,
/// the same registrations as <see cref="GameLifetimeScope"/> Configure.
/// EditMode does not run this graph from <c>Awake</c>: that path can defer Build and publish nothing.
/// </summary>
public sealed class GameLifetimeScopeCycleEditModeTests
{
    GameObject _root;
    GameObject _appGo;
    GameObject _gameGo;
    Container _appContainer;
    IScopedObjectResolver _gameContainer;

    [TearDown]
    public void TearDown()
    {
        if (_gameContainer != null)
            _gameContainer.Dispose();
        if (_appContainer != null)
            _appContainer.Dispose();
        if (_gameGo != null)
            UnityEngine.Object.DestroyImmediate(_gameGo);
        if (_appGo != null)
            UnityEngine.Object.DestroyImmediate(_appGo);
        if (_root != null)
            UnityEngine.Object.DestroyImmediate(_root);
        NetSession[] sessions = UnityEngine.Object.FindObjectsByType<NetSession>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < sessions.Length; i++)
        {
            if (sessions[i] != null)
                UnityEngine.Object.DestroyImmediate(sessions[i].gameObject);
        }

        App.GameContainer = null;
        App.Container = null;
    }

    [Test]
    public void Game_scope_builds_without_a_click_target_cycle()
    {
        _root = new GameObject("GameScopeCycleTest");
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
        // Inactive so World.Awake does not call EnsureGameScope during this build.
        _root.SetActive(false);
        _root.AddComponent<World>();

        _appGo = new GameObject("AppLifetimeScope");
        _appGo.SetActive(false);
        _appGo.AddComponent<EventProcessor>();
        _appGo.AddComponent<SceneLoader>();
        _appGo.AddComponent<LoginClient>();
        _appGo.AddComponent<GameClient>();
        _appGo.AddComponent<GameManager>();
        AppLifetimeScope app = _appGo.AddComponent<AppLifetimeScope>();
        app.autoRun = false;

        _gameGo = new GameObject("GameLifetimeScope");
        _gameGo.SetActive(false);
        GameLifetimeScope game = _gameGo.AddComponent<GameLifetimeScope>();
        game.autoRun = false;

        bool appBuilt = false;
        bool gameBuilt = false;
        try
        {
            var appBuilder = new ContainerBuilder();
            app.Install(appBuilder);
            _appContainer = appBuilder.Build() as Container;
            appBuilt = _appContainer != null;
            if (_appContainer == null)
            {
                Assert.Fail(
                    "AppLifetimeScope.Install did not build a VContainer.Container. " +
                    "autoRun=" + app.autoRun + " active=" + _appGo.activeSelf +
                    " awakeBuildRan=" + (app.Container != null));
            }

            _gameContainer = _appContainer.CreateScope(builder => game.Install(builder));
            gameBuilt = _gameContainer != null;
        }
        catch (Exception ex)
        {
            Assert.Fail(Diagnostic(app, game, appBuilt, gameBuilt) + "\n" + ex);
        }

        Assert.IsNotNull(_gameContainer, Diagnostic(app, game, appBuilt, gameBuilt));
        Assert.IsNotNull(_gameContainer.Resolve<World>());
        Assert.IsNotNull(_gameContainer.Resolve<ClickManager>());
        Assert.IsNotNull(_gameContainer.Resolve<TargetManager>());
        ISpawnedEntities spawned = _gameContainer.Resolve<ISpawnedEntities>();
        Assert.IsNotNull(spawned);
        Assert.IsNull(spawned.Find(1));
    }

    static string Diagnostic(AppLifetimeScope app, GameLifetimeScope game, bool appBuilt, bool gameBuilt)
    {
        return "appBuilt=" + appBuilt +
            " gameBuilt=" + gameBuilt +
            " appAutoRun=" + app.autoRun +
            " appActive=" + app.gameObject.activeSelf +
            " appAwakeContainer=" + (app.Container != null) +
            " gameAutoRun=" + game.autoRun +
            " gameActive=" + game.gameObject.activeSelf +
            " gameAwakeContainer=" + (game.Container != null) +
            " parent=AppLifetimeScope.Install container (not a scene Find)";
    }

    void Plant<T>() where T : Component
    {
        _root.AddComponent<T>();
    }
}
