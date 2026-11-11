using System;

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public partial class AnimationManager : BaseAnimationManager , IAnimationManager
{
    public static void LogAnimationShootFromAnimator(int objectId, string decision, string eventArg, string animatorDetail)
    {
        // Debug.Log(
            // "[ANIM_SHOOT_EVENT] " + decision +
            // $" objectId={objectId} eventArg='{eventArg}' {animatorDetail} " +
            // $"now={Time.time:F3}s frame={Time.frameCount}");
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
}
