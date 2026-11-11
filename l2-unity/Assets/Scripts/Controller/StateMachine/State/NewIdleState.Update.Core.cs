using UnityEngine;

public partial class NewIdleState : StateBase
{
    private void HandleWaitReturn(object payload = null)
    {
        // Combat swing finished. Always leave attack latch and show wait pose.
        if (DomainPacketServices.LocalPlayer != null)
        {
            DomainPacketServices.LocalPlayer.IsAttack = false;
            DomainPacketServices.LocalPlayer.LastAtkAnimation = null;
        }

        bool fromCombatSmb = IsWaitReturnFromCombatSmb(payload);
        bool targetDead = IsCurrentTargetDead();
        bool useAtkWait = _stateMachine.Player != null && _stateMachine.Player.isAutoAttack;
        var animation = ForRide(useAtkWait ? AnimationNames.ATK_WAIT : AnimationNames.WAIT);

        // Skip only early external WAIT_RETURN (DieDto / skill Complete) while swing still plays.
        // SMB SwitchToIdle uses wall clock — animNorm often < 0.95 when it fires; must CrossFade.
        string swingState = null;
        float swingNorm = 0f;
        bool skipSwing = !fromCombatSmb && TryGetActiveCombatSwing(out swingState, out swingNorm);
        // WaitReturnLog.Handle(
            // "NewIdleState.HandleWaitReturn",
            // fromCombatSmb,
            // targetDead,
            // useAtkWait,
            // animation.ToString(),
            // skipSwing,
            // skipSwing ? swingState + " n=" + swingNorm.ToString("F2") : "-");
        if (skipSwing)
            return;

        // Defer one frame: CrossFade from inside JAtk OnStateUpdate is unreliable, and a
        // same-frame second CrossFade (Complete → CallBack) can leave the Animator stuck.
        PlayAnimationDeferred(animation, fromCombatSmb);
    }

    private static bool IsWaitReturnFromCombatSmb(object payload)
    {
        if (payload is string s &&
            string.Equals(s, WaitReturnFromCombatSmb, System.StringComparison.Ordinal))
        {
            return true;
        }

        return payload is bool b && b;
    }

    private static bool IsCurrentTargetDead()
    {
        if (DomainPacketServices.LocalPlayer == null || ControllerPacketServices.GameWorld == null)
        {
            return false;
        }

        int targetId = DomainPacketServices.LocalPlayer.TargetId;
        if (targetId == 0)
        {
            return false;
        }

        Entity target = ControllerPacketServices.GameWorld.GetEntityNoLockSync(targetId);
        return target == null || target.IsDead();
    }
}
