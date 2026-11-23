using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using VContainer;

/// <summary>
/// Lobby pawns are the shared character prefab. The lobby container must inject motion
/// services and must not require game-scope types such as <see cref="World"/>.
/// </summary>
public sealed class LobbyPawnScopeEditModeTests
{
    const string PawnPath = "Assets/Resources/Data/Animations/Human/FFighter/Pawn_FFighter.prefab";

    GameObject _host;
    GameObject _slot;
    GameObject _pawn;
    IObjectResolver _app;
    IScopedObjectResolver _lobby;
    IObjectResolver _savedApp;
    IObjectResolver _savedGame;
    CharacterCreator _creator;

    [SetUp]
    public void SetUp()
    {
        _savedApp = App.Container;
        _savedGame = App.GameContainer;
        App.Container = null;
        App.GameContainer = null;

        _host = new GameObject("lobby-scope");
        _creator = _host.AddComponent<CharacterCreator>();
        CharacterBuilder builder = _host.AddComponent<CharacterBuilder>();

        var appBuilder = new ContainerBuilder();
        MotionPipelineRegistration.Register(appBuilder);
        appBuilder.Register<ModelTable>(Lifetime.Singleton);
        appBuilder.Register<LogongrpTable>(Lifetime.Singleton);
        _app = appBuilder.Build();
        _lobby = _app.CreateScope(child =>
        {
            child.RegisterComponent(_creator);
            child.RegisterComponent(builder);
        });
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            if (_lobby != null)
                _lobby.Dispose();
            if (_app != null)
                _app.Dispose();
            if (_pawn != null)
            {
                Animator animator = _pawn.GetComponentInChildren<Animator>(true);
                if (animator != null)
                    L2ActorPointLights.UnregisterActorRoot(animator.transform);
                L2ActorPointLights.UnregisterActorRoot(_pawn.transform);
            }
            if (_slot != null)
                UnityEngine.Object.DestroyImmediate(_slot);
            if (_pawn != null)
                UnityEngine.Object.DestroyImmediate(_pawn);
            if (_host != null)
                UnityEngine.Object.DestroyImmediate(_host);
        }
        finally
        {
            App.Container = _savedApp;
            App.GameContainer = _savedGame;
            HierarchyScopeInjection.UnbindLobby(_lobby);
        }
    }

    [Test]
    public void PlacePawn_with_lobby_container_binds_motion_without_game_scope_types()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PawnPath);
        Assert.IsNotNull(prefab, PawnPath + " missing — the lobby pawn prefab was not loaded.");
        _pawn = UnityEngine.Object.Instantiate(prefab);
        Assert.IsFalse(_pawn.activeSelf, "Pawn_FFighter starts inactive so PlacePawn is the activation path.");

        UserGear gear = _pawn.GetComponent<UserGear>();
        Assert.IsNotNull(gear, "Pawn_FFighter has no UserGear.");
        // Lobby roster equips a weapon suffix before PlacePawn. The wait pose uses that suffix.
        SetWeaponAnim(gear, "hand");

        IReadOnlyList<Type> required = HierarchyScopeInjection.RequiredTypes(_lobby, _pawn);
        IReadOnlyList<Type> other = HierarchyScopeInjection.OtherScopeTypes(_lobby, _pawn);

        AssertContains(required, typeof(IAnimationManager));
        AssertContains(required, typeof(ModelTable));
        AssertNotContains(required, typeof(World));
        AssertNotContains(required, typeof(IWorldSpawnContext));
        AssertContains(other, typeof(World));
        AssertDisjoint(required, other);

        for (int i = 0; i < required.Count; i++)
        {
            Type type = required[i];
            object resolved;
            Assert.IsTrue(
                _lobby.TryResolve(type, out resolved),
                "lobby scope did not resolve " + type.Name);
            Assert.IsNotNull(resolved, type.Name);
        }

        object world;
        Assert.IsFalse(
            _lobby.TryResolve(typeof(World), out world),
            "lobby scope registered World — game scope leaked into the lobby container.");

        _slot = new GameObject("lobby-slots");
        _creator.PlacePawn(_pawn, new Logongrp(), "FFighter", _slot);

        IAnimationManager animations = ReadAnimations(_pawn.GetComponent<SelectableCharacterEntity>());
        Assert.AreSame(_lobby.Resolve<IAnimationManager>(), animations);

        BaseAnimationController body = _pawn.GetComponent<BaseAnimationController>();
        Assert.IsNotNull(body);
        Assert.IsTrue(body.IsMotionGraphBound, "PlacePawn did not bind the shared motion graph.");
        Animator animator = body.GetAnimator();
        Assert.IsNotNull(animator, "lobby pawn has no Animator.");
        Assert.IsNotNull(animator.avatar, "lobby pawn Animator has no Avatar.");
        Assert.IsTrue(animator.avatar.isValid, "lobby pawn Avatar is invalid.");
    }

    static void SetWeaponAnim(UserGear gear, string suffix)
    {
        FieldInfo field = typeof(Gear).GetField(
            "_weaponAnim",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, "Gear._weaponAnim missing — the lobby wait pose suffix could not be set.");
        field.SetValue(gear, suffix);
    }

    static IAnimationManager ReadAnimations(SelectableCharacterEntity entity)
    {
        Assert.IsNotNull(entity);
        FieldInfo field = typeof(SelectableCharacterEntity).GetField(
            "_animations",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, "SelectableCharacterEntity._animations missing.");
        return field.GetValue(entity) as IAnimationManager;
    }

    static void AssertContains(IReadOnlyList<Type> types, Type type)
    {
        for (int i = 0; i < types.Count; i++)
        {
            if (types[i] == type)
                return;
        }

        Assert.Fail("expected " + type.Name);
    }

    static void AssertNotContains(IReadOnlyList<Type> types, Type type)
    {
        for (int i = 0; i < types.Count; i++)
        {
            if (types[i] == type)
                Assert.Fail(type.Name + " is required by lobby injection.");
        }
    }

    static void AssertDisjoint(IReadOnlyList<Type> left, IReadOnlyList<Type> right)
    {
        for (int i = 0; i < left.Count; i++)
        {
            for (int j = 0; j < right.Count; j++)
            {
                if (left[i] == right[j])
                    Assert.Fail(left[i].Name + " is both required and outside the lobby scope.");
            }
        }
    }
}
