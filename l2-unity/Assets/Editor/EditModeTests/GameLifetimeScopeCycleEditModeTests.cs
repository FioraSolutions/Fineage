using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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
    List<Type> _gameScopeTypes;

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

            _gameContainer = _appContainer.CreateScope(builder =>
            {
                game.Install(builder);
                _gameScopeTypes = GameScopeTypesToResolve(builder);
            });
            gameBuilt = _gameContainer != null;
        }
        catch (Exception ex)
        {
            Assert.Fail(Diagnostic(app, game, appBuilt, gameBuilt) + "\n" + ex);
        }

        Assert.IsNotNull(_gameContainer, Diagnostic(app, game, appBuilt, gameBuilt));
        AssertResolvesEveryGameScopeType(_gameContainer, _gameScopeTypes);
        Assert.IsInstanceOf<VisibilityManager>(_gameContainer.Resolve<IVisibilityRoster>());
        Assert.IsNotNull(_gameContainer.Resolve<World>());
        Assert.IsNotNull(_gameContainer.Resolve<ClickManager>());
        Assert.IsNotNull(_gameContainer.Resolve<TargetManager>());
        ISpawnedEntities spawned = _gameContainer.Resolve<ISpawnedEntities>();
        Assert.IsNotNull(spawned);
        Assert.IsNull(spawned.Find(1));
    }

    /// <summary>
    /// Service types the game scope registered, plus <c>[Inject]</c> dependencies of the types it constructs.
    /// Factory registrations that return a live scene object are resolved, but their fields are not:
    /// VContainer does not inject a null factory result.
    /// </summary>
    static List<Type> GameScopeTypesToResolve(IContainerBuilder builder)
    {
        var types = new List<Type>();
        for (int i = 0; i < builder.Count; i++)
        {
            Registration built = builder[i].Build();
            if (built.Key != null)
                continue;

            if (built.InterfaceTypes != null && built.InterfaceTypes.Count > 0)
            {
                for (int t = 0; t < built.InterfaceTypes.Count; t++)
                    AddResolvable(types, built.InterfaceTypes[t]);
            }
            else
            {
                AddResolvable(types, built.ImplementationType);
            }

            string provider = built.Provider != null ? built.Provider.GetType().Name : "";
            // Factories and RegisterInstance do not inject. Walking their members resolves
            // constructor arguments that were never container services (ILateBound<T> closures).
            bool constructed = provider == "InstanceProvider";
            bool injectsMembers = constructed
                || provider == "ExistingComponentProvider"
                || provider == "FindComponentProvider"
                || provider == "NewGameObjectProvider"
                || provider == "PrefabComponentProvider";
            if (!injectsMembers)
                continue;

            CollectInjected(built.ImplementationType, types, constructed);
        }

        return types;
    }

    static void AssertResolvesEveryGameScopeType(IObjectResolver container, List<Type> types)
    {
        Assert.IsNotNull(types, "GameLifetimeScope.Install did not record its registrations.");
        var failures = new StringBuilder();
        for (int i = 0; i < types.Count; i++)
        {
            Type type = types[i];
            try
            {
                container.Resolve(type);
            }
            catch (Exception ex)
            {
                failures.Append(type.FullName);
                failures.Append(": ");
                failures.Append(ex.GetType().Name);
                failures.Append(": ");
                failures.Append(ex.Message);
                failures.Append('\n');
            }
        }

        if (failures.Length > 0)
        {
            Assert.Fail(
                "Game scope did not register every injected type. Missing registrations must fail here, not at runtime.\n" +
                failures);
        }
    }

    static void CollectInjected(Type type, List<Type> into, bool includeConstructor)
    {
        if (type == null || type.ContainsGenericParameters || type.IsInterface)
            return;

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        for (Type current = type; current != null && current != typeof(object); current = current.BaseType)
        {
            FieldInfo[] fields = current.GetFields(flags);
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].IsDefined(typeof(InjectAttribute), false))
                    AddResolvable(into, fields[i].FieldType);
            }

            PropertyInfo[] properties = current.GetProperties(flags);
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].IsDefined(typeof(InjectAttribute), false))
                    AddResolvable(into, properties[i].PropertyType);
            }

            MethodInfo[] methods = current.GetMethods(flags);
            for (int i = 0; i < methods.Length; i++)
            {
                if (!methods[i].IsDefined(typeof(InjectAttribute), false))
                    continue;
                ParameterInfo[] parameters = methods[i].GetParameters();
                for (int p = 0; p < parameters.Length; p++)
                    AddResolvable(into, parameters[p].ParameterType);
            }
        }

        if (!includeConstructor)
            return;

        ConstructorInfo chosen = null;
        ConstructorInfo[] ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        int best = -1;
        for (int i = 0; i < ctors.Length; i++)
        {
            if (ctors[i].IsDefined(typeof(InjectAttribute), false))
            {
                chosen = ctors[i];
                break;
            }

            int count = ctors[i].GetParameters().Length;
            if (count > best)
            {
                best = count;
                chosen = ctors[i];
            }
        }

        if (chosen == null)
            return;

        ParameterInfo[] ctorParameters = chosen.GetParameters();
        for (int i = 0; i < ctorParameters.Length; i++)
            AddResolvable(into, ctorParameters[i].ParameterType);
    }

    static void AddResolvable(List<Type> types, Type type)
    {
        if (type == null || type.ContainsGenericParameters || type.IsPointer || type.IsByRef)
            return;
        if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            return;
        if (type.Name.IndexOf('<') >= 0)
            return;
        for (int i = 0; i < types.Count; i++)
        {
            if (types[i] == type)
                return;
        }

        types.Add(type);
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
