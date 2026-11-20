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

        stateName = ResolvePlayableCombatState(controller, requestedState);
        if (!HasCombatState(controller, stateName) &&
            TryPlaySpAtkOverrideClip(objectId, controller, entity, requestedState))
        {
            return true;
        }

        SetRecentName(objectId, stateName);
        float fade = LocomotionCrossFadeSettings.FixedDuration;
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
            return false;

        BaseAnimationController castBody = controller as BaseAnimationController;
        if (castBody == null || !castBody.TryPlaySpAtkCombat(clip, phaseStateName))
            return false;

        SetRecentName(objectId, phaseStateName);
        return true;
    }

    /// <summary>
    /// Player_Basic only has SpAtk01/02 for a few weapons. Dagger blows such as Deadly Blow
    /// ask for SpAtk04_1HS, and dual asks for *_dual — neither state exists, so CrossFade
    /// no-ops and the skill looks uncast. Play the same-number 1HS swing, or SpAtk01_1HS.
    /// </summary>
    static string ResolvePlayableCombatState(IAnimationController controller, string stateName)
    {
        if (string.IsNullOrEmpty(stateName) || HasCombatState(controller, stateName))
            return stateName;

        int split = stateName.LastIndexOf('_');
        if (split <= 0)
            return stateName;

        string stem = stateName.Substring(0, split + 1);
        // Fists and bare hands must not borrow the club/sword swing when the
        // hand state is missing. That is the "кастеты бьют как дубина" pose.
        if (stateName.EndsWith("_hand", System.StringComparison.Ordinal))
            return stateName;

        // SpAtk06 is the retail self-buff gesture (AnimationCombo_Classic "X":
        // Ultimate Evasion / Dash / Focus Chance / Escape Shackle). In every race's
        // MFighter_anim etc. the only SpAtk06 pose is *_SpAtk06_Hand_<Race>, a hands-up
        // buff with no weapon: equipment never adds a suffix. Asking for SpAtk06_1HS
        // (or _bow / _dual_dagger …) must land on SpAtk06_hand, never on SpAtk01_1HS
        // (a sword swing that also fires AttackShot at the selected mob).
        if (stateName.IndexOf("SpAtk06_", System.StringComparison.Ordinal) == 0 &&
            HasCombatState(controller, SpAtk06HandState))
            return SpAtk06HandState;

        string oneHs = stem + "1HS";
        if (!string.Equals(oneHs, stateName, System.StringComparison.Ordinal) &&
            HasCombatState(controller, oneHs))
            return oneHs;

        if (stem.StartsWith("SpAtk", System.StringComparison.Ordinal) &&
            HasCombatState(controller, "SpAtk01_1HS"))
            return "SpAtk01_1HS";

        return stateName;
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

        return ResolvePlayableCombatState(controller, stateName);
    }

    /// <summary>Retail self-buff gesture state shared by every race's controller.</summary>
    const string SpAtk06HandState = "SpAtk06_hand";

    static bool HasCombatState(IAnimationController controller, string stateName)
    {
        Animator animator = controller != null ? controller.GetAnimator() : null;
        return animator != null && animator.HasState(0, Animator.StringToHash(stateName));
    }
}
