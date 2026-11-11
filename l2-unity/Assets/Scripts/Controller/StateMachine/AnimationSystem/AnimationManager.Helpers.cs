using System;

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public partial class AnimationManager : BaseAnimationManager , IAnimationManager
{
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

        stateName = ResolvePlayableCombatState(controller, stateName);
        SetRecentName(objectId, stateName);
        float fade = LocomotionCrossFadeSettings.FixedDuration;
        PlayerBasicAttackCrossFade.TryPlay(controller, stateName, fade);
        // Debug.Log(
            // $"AnimationManager> start crossfade {family} player {entity.name} " +
            // $"state={stateName} duration={fade:F3}s");
        return true;
    }

    /// <summary>
    /// Bare hands use WeaponAnim "hand" (wait_hand / atkwait_hand exist), but Player_Basic
    /// has no jatk*_hand / SpAtk*_hand states yet — CrossFade would no-op and the pawn freezes.
    /// Fall back to 1HS swing until hand attack states are wired.
    /// </summary>
    static string ResolvePlayableCombatState(IAnimationController controller, string stateName)
    {
        if (string.IsNullOrEmpty(stateName) ||
            !stateName.EndsWith("_hand", System.StringComparison.Ordinal))
        {
            return stateName;
        }

        Animator animator = controller != null ? controller.GetAnimator() : null;
        if (animator != null && animator.HasState(0, Animator.StringToHash(stateName)))
            return stateName;

        return stateName.Substring(0, stateName.Length - "hand".Length) + "1HS";
    }
}
