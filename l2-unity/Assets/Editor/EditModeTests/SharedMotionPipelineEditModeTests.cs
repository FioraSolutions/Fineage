using System;
using NUnit.Framework;
using UnityEditor.Animations;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Lobby and game both inherit one animation manager and one move roster.
/// A motion clip on that graph must move the bone and leave its scale at the bind pose.
/// </summary>
public sealed class SharedMotionPipelineEditModeTests
{
    const float BindScale = 1f;
    const float ClipScale = 4f;

    GameObject _host;
    GameObject _lobby;
    GameObject _game;
    GameObject _pawn;
    string _controllerPath;

    [TearDown]
    public void TearDown()
    {
        if (_game != null)
            UnityEngine.Object.DestroyImmediate(_game);
        if (_lobby != null)
            UnityEngine.Object.DestroyImmediate(_lobby);
        if (_host != null)
            UnityEngine.Object.DestroyImmediate(_host);
        if (_pawn != null)
            UnityEngine.Object.DestroyImmediate(_pawn);
        if (!string.IsNullOrEmpty(_controllerPath))
            UnityEditor.AssetDatabase.DeleteAsset(_controllerPath);
    }

    [Test]
    public void Lobby_and_game_scopes_share_the_motion_pipeline_and_a_clip_keeps_bone_scale()
    {
        // EditMode does not publish LifetimeScope from Awake, so the container comes from Build.
        _host = new GameObject("motion-app");
        _host.SetActive(false);
        MotionHostScope app = _host.AddComponent<MotionHostScope>();
        app.autoRun = false;
        _host.SetActive(true);
        app.Build();
        Assert.IsNotNull(app.Container, "motion app scope did not build");

        _lobby = new GameObject("motion-lobby");
        GameMotionChildScope lobby = BuildChild(_lobby, app);
        _game = new GameObject("motion-game");
        GameMotionChildScope game = BuildChild(_game, app);

        IAnimationManager lobbyAnimations = lobby.Container.Resolve<IAnimationManager>();
        IAnimationManager gameAnimations = game.Container.Resolve<IAnimationManager>();
        IMoveRoster lobbyMoves = lobby.Container.Resolve<IMoveRoster>();
        IMoveRoster gameMoves = game.Container.Resolve<IMoveRoster>();

        Assert.IsInstanceOf<AnimationManager>(lobbyAnimations);
        Assert.AreSame(lobbyAnimations, gameAnimations);
        Assert.IsInstanceOf<SharedMoveRoster>(lobbyMoves);
        Assert.AreSame(lobbyMoves, gameMoves);

        Transform bone = BuildPawn();
        Vector3 scaleBefore = bone.localScale;
        BaseAnimationController body = _pawn.GetComponent<BaseAnimationController>();
        AnimationClip clip = ScaledClip();

        lobbyAnimations.PlayMotionClip(body, clip);
        body.EvaluateMotionGraph(0.25f);

        Assert.Greater(bone.localPosition.x, 0.05f, "the shared graph did not play the clip");
        Assert.AreEqual(scaleBefore.x, bone.localScale.x, 0.02f);
        Assert.AreEqual(scaleBefore.y, bone.localScale.y, 0.02f);
        Assert.AreEqual(scaleBefore.z, bone.localScale.z, 0.02f);
        Assert.AreEqual(BindScale, bone.localScale.x, 0.02f);
    }

    static GameMotionChildScope BuildChild(GameObject host, LifetimeScope parent)
    {
        host.SetActive(false);
        GameMotionChildScope child;
        using (LifetimeScope.EnqueueParent(parent))
        {
            child = host.AddComponent<GameMotionChildScope>();
            child.autoRun = false;
            host.SetActive(true);
            if (child.Container == null)
                child.Build();
        }

        Assert.IsNotNull(child.Container, host.name + " scope did not build");
        return child;
    }

    Transform BuildPawn()
    {
        _pawn = new GameObject("Pawn");
        Transform bone = new GameObject("Bone").transform;
        bone.SetParent(_pawn.transform, false);
        bone.localScale = Vector3.one;
        bone.localPosition = Vector3.zero;

        Animator animator = _pawn.AddComponent<Animator>();
        Avatar avatar = AvatarBuilder.BuildGenericAvatar(_pawn, "");
        Assert.IsNotNull(avatar, "generic avatar was not built");
        Assert.IsTrue(avatar.isValid, "generic avatar is invalid");

        _controllerPath = "Assets/Editor/EditModeTests/__tmp_motion_" +
                          Guid.NewGuid().ToString("N") + ".controller";
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(_controllerPath);

        animator.avatar = avatar;
        animator.runtimeAnimatorController = controller;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        _pawn.AddComponent<BaseAnimationController>();
        return bone;
    }

    static AnimationClip ScaledClip()
    {
        var clip = new AnimationClip { name = "to-battle", frameRate = 30f, legacy = false };
        var position = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        var scale = AnimationCurve.Linear(0f, ClipScale, 1f, ClipScale);
        clip.SetCurve("Bone", typeof(Transform), "localPosition.x", position);
        clip.SetCurve("Bone", typeof(Transform), "localScale.x", scale);
        clip.SetCurve("Bone", typeof(Transform), "localScale.y", scale);
        clip.SetCurve("Bone", typeof(Transform), "localScale.z", scale);
        return clip;
    }
}

/// <summary>Parent scope that registers the same motion services as the app scope.</summary>
public sealed class MotionHostScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        MotionPipelineRegistration.Register(builder);
    }
}

/// <summary>
/// Same parent link the game scope uses. It does not register a second animation manager or move roster.
/// </summary>
public sealed class GameMotionChildScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
    }
}
