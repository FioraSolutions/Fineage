using System;

﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public partial class AnimationManager : BaseAnimationManager , IAnimationManager
{
   
    private static AnimationManager _instance;
    private readonly HashSet<int> _awaitSubscribedObjectIds = new HashSet<int>();
    private readonly Dictionary<int, string> _expectedFinishNameByObjectId = new Dictionary<int, string>();
    private const string SP_TIME_ATK = "sptimeatk";
    private const string CAST_TRIGGER_MID = "CastMid";
    private const string CAST_TRIGGER_END = "CastEnd";
    private const string CAST_TRIGGER_SHOT = "MagicShot";
    private const string CAST_TRIGGER_END_2P = "CastEnd2P";
    private const string CAST_TRIGGER_SHOT_2P = "MagicShot2P";
    private const string SHOT_SOURCE_RUNNER = "Runner";
    private const int OVERRIDE_AWAIT_WARN_TIMEOUT_MS = 5000;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    /// <summary>
    /// Телеметрия: откуда пришёл shoot (параметр ивента на клипе) и что реально играет на animator layer0.
    /// Вызывается из <see cref="BaseAnimationController.OnAnimationShoot"/>.
    /// </summary>
    public static void LogAnimationShootFromAnimator(int objectId, string decision, string eventArg, string animatorDetail)
    {
        // Debug.Log(
            // "[ANIM_SHOOT_EVENT] " + decision +
            // $" objectId={objectId} eventArg='{eventArg}' {animatorDetail} " +
            // $"now={Time.time:F3}s frame={Time.frameCount}");
    }
#endif

    [System.Obsolete("Use [Inject] IAnimationManager or AnimationPacketServices.Animations instead")]
    public static IAnimationManager Instance
    {
        get
        {
            if (_instance == null && App.HasContainer)
            {
                try
                {
                    _instance = App.Resolve<AnimationManager>();
                }
                catch
                {
                }
            }

            if (_instance == null)
                _instance = new AnimationManager();
            return _instance;
        }
    }

    public static void Bind(AnimationManager manager)
    {
        if (manager != null)
            _instance = manager;
    }

    public void PlayAnimation(int objectId , string animationName, bool disableTriggerAfterStart)
    {
        if (GetModel(objectId) == null)
            return;

        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null)
            return;

        if (PlayerStriderAnim.TryResolve(animationName, out bool mountRunning))
        {
            StriderRideService striderRide = AnimationPacketServices.StriderRide;
            if (striderRide != null)
                striderRide.SetMountGait(objectId, mountRunning);
            if (!PlayerLocomotionCrossFade.IsAlreadyPlaying(controller, PlayerStriderAnim.State))
            {
                DesableLastPlayerAnimationElseTrue(objectId, controller);
                controller.SetAnimatorSpeed(1f);
                PlayerLocomotionCrossFade.TryPlay(controller, PlayerStriderAnim.State, 0.12f);
            }
            SetRecentName(objectId, PlayerStriderAnim.State);
            return;
        }

        // Bare names without weapon suffix (death / rebirth) — before GetFinalNameAnim appends _1HS etc.
        if (PlayerDeathAnim.TryResolve(animationName, out string bareStateName))
        {
            PlayExactAnimatorState(objectId, bareStateName);
            return;
        }

        string finalAnimName = GetFinalNameAnim(objectId , animationName );

        Entity entity = GetEntity(objectId);

        // Locomotion: wait_/walk_/run_/atkwait_* → CrossFadeInFixedTime (bool graph bypass).
        if (PlayerLocomotionAnim.TryResolve(finalAnimName, out string locoStateName, out PlayerLocomotionFamily family))
        {
            // Dedicated AtkWait path — breakpoint here to see callers / skip vs play.
            if (family == PlayerLocomotionFamily.AtkWait)
            {
                HandleAtkWaitCrossFade(objectId, controller, locoStateName);
                return;
            }

            if (PlayerLocomotionCrossFade.ShouldSkip(controller, locoStateName, GetCurrentAnimationName(objectId)))
            {
                SetRecentName(objectId, locoStateName);
                // Debug.Log(
                    // $"AnimationManager> skip crossfade {family} player {entity.name} " +
                    // $"state={locoStateName} reason=already_playing");
                return;
            }

            DesableLastPlayerAnimationElseTrue(objectId, controller);
            SetRecentName(objectId, locoStateName);
            // MagicShot leaves animator.speed scaled (SpeedShot); wait must not keep that rate.
            controller.SetAnimatorSpeed(1f);
            if (family == PlayerLocomotionFamily.Walk || family == PlayerLocomotionFamily.Run)
            {
                ResetMagicCastTriggers(controller);
            }
            float fadeDuration = LocomotionCrossFadeSettings.ResolveDuration(locoStateName, controller);
            PlayerLocomotionCrossFade.TryPlay(controller, locoStateName, fadeDuration);
            // Debug.Log(
                // $"AnimationManager> start crossfade {family} player {entity.name} " +
                // $"state={locoStateName} duration={fadeDuration:F3}s");
            return;
        }

        DesableLastPlayerAnimationElseTrue(objectId, controller);
        SetRecentName(objectId , finalAnimName);
   
        controller.SetBool(finalAnimName, true, entity.name);
        // Debug.Log($"AnimationManager> start bool name player  {entity.name} animation {finalAnimName}");
    }

    /// <summary>
    /// Lobby character select / create pawns (no World Entity id).
    /// Full name e.g. wait_hand, walk_1HS — same CrossFade path as in-world locomotion.
    /// </summary>
    public void PlayLobbyLocomotion(IAnimationController controller, string stateOrPrefixWithWeapon)
    {
        if (controller == null || string.IsNullOrEmpty(stateOrPrefixWithWeapon))
        {
            return;
        }

        AnimationEventForwarder.BindAnimator(controller);

        if (!PlayerLocomotionAnim.TryResolve(stateOrPrefixWithWeapon, out string stateName, out PlayerLocomotionFamily family))
        {
            // Debug.LogWarning(
                // $"AnimationManager> lobby locomotion unresolved '{stateOrPrefixWithWeapon}'");
            return;
        }

        // Always play — lobby has no recent-name cache; force pose on place / walk stop.
        float fadeDuration = LocomotionCrossFadeSettings.ResolveDuration(stateName, controller);
        controller.CrossFadeInFixedTime(stateName, fadeDuration);
        // Debug.Log(
            // $"AnimationManager> start crossfade lobby {family} state={stateName} " +
            // $"duration={fadeDuration:F3}s");
    }

    /// <summary>
    /// AtkWait only. Does not skip on "already playing" — code owns the pose after attack
    /// (animator exit-transition + PlayAnimation used to race and sometimes skip).
    /// </summary>
    private void HandleAtkWaitCrossFade(
        int objectId,
        IAnimationController controller,
        string atkWaitStateName)
    {
        DesableLastPlayerAnimationElseTrue(objectId, controller);
        SetRecentName(objectId, atkWaitStateName);

        // Leave attack playback rate so atkwait does not stay frozen at last swing speed.
        AnimPlayback.PrepareGesture(controller);

        // Always CrossFade — even if animator already in atkwait (stale transition / same pose).
        // Dual jatk often still mid-clip when wall cycle ends — longer blend softens the cut.
        float duration = LocomotionCrossFadeSettings.ResolveDuration(atkWaitStateName, controller);
        controller.CrossFadeInFixedTime(atkWaitStateName, duration);
        // WaitReturnLog.Play(objectId, atkWaitStateName);
    }

    public void PlayAnimationTrigger(int objectId, string animationName)
    {
        IAnimationController controller = GetRegisteredController(objectId);
        Entity entity = GetEntity(objectId);
        if (controller == null)
            return;

        string triggerName = GetFinalNameAnim(objectId , animationName);
        DesableLastPlayerAnimationElseTrue(objectId, controller);

        // Basic melee (jatk*) and SpAtk* → CrossFade; MagicShot/Cast stay on Trigger.
        if (TryPlayCombatCrossFade(objectId, controller, entity, triggerName))
            return;

        controller.ToggleAnimationTrigger(triggerName);
        SetRecentName(objectId, triggerName);
        // Debug.Log($"AnimationManager> start trigger name player  {(entity != null ? entity.name : objectId.ToString())} animation {triggerName}");
    }

    /// <summary>
    /// jatk01/02/03_* and SpAtk01/02_* → CrossFade + priority lock.
    /// </summary>
    private bool TryPlayCombatCrossFade(
        int objectId,
        IAnimationController controller,
        Entity entity,
        string resolvedStateName)
    {
        string stateName = null;
        if (!PlayerBasicAttackAnim.TryResolve(resolvedStateName, out stateName) &&
            !PlayerSpAtkAnim.TryResolve(resolvedStateName, out stateName))
        {
            return false;
        }

        SetRecentName(objectId, stateName);
        float fade = LocomotionCrossFadeSettings.FixedDuration;
        PlayerBasicAttackCrossFade.TryPlay(controller, stateName, fade);
        // Debug.Log(
            // $"AnimationManager> start crossfade {family} player {entity.name} " +
            // $"state={stateName} duration={fade:F3}s");
        return true;
    }


    //Async Wait End Event
}
