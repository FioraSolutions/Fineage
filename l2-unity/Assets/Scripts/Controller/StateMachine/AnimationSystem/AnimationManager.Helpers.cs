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

        string requestedState = stateName;
        if (!HasCombatState(controller, requestedState) &&
            TryPlaySpAtkOverrideClip(objectId, controller, entity, requestedState))
        {
            return true;
        }

        string raceName = controller != null ? controller.GetAnimatorName() : null;
        int skillId = entity != null && entity.GetMagicCastData() != null
            ? entity.GetMagicCastData().SkillId
            : 0;
        stateName = SkillAnimationResolver.ResolvePlayableCombatState(
            controller,
            requestedState,
            raceName,
            skillId,
            s => HasCombatState(controller, s));
        if (!HasCombatState(controller, stateName) &&
            TryPlaySpAtkOverrideClip(objectId, controller, entity, requestedState))
        {
            return true;
        }

        if (!HasCombatState(controller, stateName) &&
            stateName.IndexOf("SpAtk", StringComparison.Ordinal) >= 0 &&
            SkillAnimationDatabase.GetSpAtkCombatClip(stateName, controller.GetAnimatorName()) == null)
        {
            Debug.LogError(
                "[AnimationManager] SpAtk combat unresolved objectId=" + objectId
                + " race=" + controller.GetAnimatorName()
                + " state=" + stateName
                + " weaponSuffix=" + ExtractWeaponSuffix(stateName)
                + " (no animator state and no race override clip).");
        }

        SetRecentName(objectId, stateName);
        float fade = LocomotionCrossFadeSettings.FixedDuration;
        if (!HasCombatState(controller, stateName) &&
            stateName.IndexOf("SpAtk", StringComparison.Ordinal) == 0 &&
            SkillAnimationDatabase.GetSpAtkCombatClip(stateName, raceName) == null)
        {
            SkillAnimationResolver.LogMissingCombatClip(skillId, raceName, stateName);
            return false;
        }

        PlayerBasicAttackCrossFade.TryPlay(controller, stateName, fade);
        // Debug.Log(
            // $"AnimationManager> start crossfade {family} player {entity.name} " +
            // $"state={stateName} duration={fade:F3}s");
        return true;
    }

    bool TryPlaySpAtkOverrideClip(
        int objectId,
        IAnimationController controller,
        Entity entity,
        string phaseStateName)
    {
        if (controller == null ||
            string.IsNullOrEmpty(phaseStateName) ||
            phaseStateName.IndexOf("SpAtk", System.StringComparison.Ordinal) != 0)
        {
            return false;
        }

        string raceName = controller.GetAnimatorName();
        AnimationClip clip = SkillAnimationDatabase.GetSpAtkCombatClip(phaseStateName, raceName);
        if (clip == null)
        {
                int skillId = entity != null && entity.GetMagicCastData() != null
                ? entity.GetMagicCastData().SkillId
                : 0;
            SkillAnimationResolver.LogMissingCombatClip(skillId, raceName, phaseStateName);
            return false;
        }

        BaseAnimationController castBody = controller as BaseAnimationController;
        if (castBody == null || !castBody.TryPlaySpAtkCombat(clip, phaseStateName))
            return false;

        SetRecentName(objectId, phaseStateName);
        return true;
    }

    static string ExtractWeaponSuffix(string stateName)
    {
        int split = stateName != null ? stateName.LastIndexOf('_') : -1;
        return split > 0 ? stateName.Substring(split + 1) : "";
    }

    /// <summary>
    /// Finish event name the SMB will report for <paramref name="triggerName"/>. Physical self skills such
    /// as Ultimate Evasion (X → SpAtk06) resolve to the shared <c>SpAtk06_hand</c> buff gesture, whose SMB
    /// reports that state name. Awaiting the raw tag would leave the runner stuck and keep the cast
    /// channel held.
    /// </summary>
    private string ResolveAwaitFinishName(int objectId, string triggerName)
    {
        string finalName = GetFinalNameAnim(objectId, triggerName);
        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null)
            return finalName;

        string stateName;
        if (!PlayerBasicAttackAnim.TryResolve(finalName, out stateName) &&
            !PlayerSpAtkAnim.TryResolve(finalName, out stateName))
            return finalName;

        if (controller != null &&
            !HasCombatState(controller, stateName) &&
            SkillAnimationDatabase.GetSpAtkCombatClip(stateName, controller.GetAnimatorName()) != null)
            return stateName;

        string raceName = controller != null ? controller.GetAnimatorName() : null;
        Entity entity = GetEntity(objectId);
        int skillId = entity != null && entity.GetMagicCastData() != null
            ? entity.GetMagicCastData().SkillId
            : 0;
        return SkillAnimationResolver.ResolvePlayableCombatState(
            controller,
            stateName,
            raceName,
            skillId,
            s => HasCombatState(controller, s));
    }

    /// <summary>Retail self-buff gesture state shared by every race's controller.</summary>
    internal const string SpAtk06HandState = "SpAtk06_hand";

    static bool HasCombatState(IAnimationController controller, string stateName)
    {
        Animator animator = controller != null ? controller.GetAnimator() : null;
        return animator != null && animator.HasState(0, Animator.StringToHash(stateName));
    }
}
