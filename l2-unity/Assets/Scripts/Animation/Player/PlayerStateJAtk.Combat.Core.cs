using UnityEngine;

public partial class PlayerStateJAtk : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!AnimatorUtils.IsLocalPlayerAnimator(animator))
            return;

        TryUnsubscribeSwordCollider();

        float elapsedMs = TimeUtils.ConvertSecToMs(Time.time - _startTime);
        float expectedMs = TimeUtils.ConvertSecToMs(_endTime);
        bool earlyWallExit = elapsedMs + 50f < expectedMs;
        float animNormRaw = stateInfo.normalizedTime;
        int entityId = ResolvePlayerEntityId();
        if (entityId > 0 && SwordCollisionPacketServices.Collision != null)
        {
            SwordCollisionPacketServices.Collision.EndAttack(entityId, _attackEpoch);
        }

        bool dieTarget = IsDieTarget();
        bool isAttack = DomainPacketServices.LocalPlayer != null && DomainPacketServices.LocalPlayer.IsAttack;
        string exitCause = _exitViaSwitchToIdle
            ? "SwitchToIdle"
            : (earlyWallExit ? "EXTERNAL_interrupt_before_wall_cycle" : "EXTERNAL_or_blend_after_wall");

        // Debug.Log(
            // $"[ATK_TIMING_CMP] Exit anim={parameterName} epoch={_attackEpoch} " +
            // $"wallElapsedMs={elapsedMs:F1} expectedServerCycleMs={expectedMs:F1} " +
            // $"deltaMs={elapsedMs - expectedMs:F1} " +
            // $"earlyExit={earlyWallExit} swordHit={_isSwordHitLogged} " +
            // $"animNorm={_lastAnimNormalized:F3}");
        // Debug.Log(
            // $"{JATK_TIMING_LOG} Exit anim={parameterName} elapsedMs={elapsedMs:F1} " +
            // $"expectedMs={expectedMs:F1} swordHit={_isSwordHitLogged} epoch={_attackEpoch}");
        // UnityChainLog.OnJAtkExit(DomainPacketServices.LocalPlayer, parameterName, expectedMs, elapsedMs, _lastAnimNormalized);

        // Debug.Log(
            // $"{JATK_EARLY_LOG} Exit anim={parameterName} cause={exitCause} " +
            // $"viaSwitchToIdle={_exitViaSwitchToIdle} earlyWallExit={earlyWallExit} " +
            // $"wallElapsedMs={elapsedMs:F1}/{expectedMs:F1} " +
            // $"animNormMod={_lastAnimNormalized:F3} animNormRaw={animNormRaw:F3} " +
            // $"clipLenSec={_clipLengthSec:F3} patkspd={_linearPatkSpd:F3} " +
            // $"dieTarget={dieTarget} dieLatched={_dieTargetLatched} isAttack={isAttack} " +
            // $"swordHit={_isSwordHitLogged} lastDeny='{_lastSwitchDenyReason}' " +
            // $"frame={Time.frameCount}");

        // Do not clear IsAttack here — next AttackDto packet may already own the combo.
    }

    private void SwitchToIdle(AnimatorStateInfo stateInfo)
    {
        // Only called after wall-clock attack cycle ends. Do not also require anim norm —
        // pole/slow clips can still be <0.9 when server cycle is done; that skipped WAIT_RETURN.
        //
        // Always leave the swing when the server cycle is done. Guarding on !IsAttack created a
        // deadlock: NewAttackIntention sets IsAttack, and only this method cleared it — so the
        // pawn froze in jatk until a second click (#522). The next AttackDto starts the next swing.
        if (_isSwitchIdle)
            return;

        float elapsedMs = TimeUtils.ConvertSecToMs(Time.time - _startTime);
        // Debug.Log(
            // $"{JATK_EARLY_LOG} SwitchToIdle FIRE anim={parameterName} " +
            // $"reason={(IsDieTarget() ? "dieTarget" : "cycle-end")} " +
            // $"wallElapsedMs={elapsedMs:F1}/{_fullCycleMs:F1} animNorm={_lastAnimNormalized:F3} " +
            // $"clipLenSec={_clipLengthSec:F3}");

        if (DomainPacketServices.LocalPlayer != null)
        {
            DomainPacketServices.LocalPlayer.IsAttack = false;
        }

        _isSwitchIdle = true;
        _exitViaSwitchToIdle = true;

        // A Move packet may already have started the walk. Do not pull that back into atkwait.
        if (PlayerStatePacketServices.PlayerState != null && PlayerStatePacketServices.PlayerState.ShouldKeepLocomotion())
        {
            // Debug.Log(
                // $"{JATK_EARLY_LOG} SwitchToIdle SKIP_LOCOMOTION anim={parameterName} " +
                // $"state={PlayerStatePacketServices.PlayerState.State} wallElapsedMs={elapsedMs:F1}/{_fullCycleMs:F1}");
            return;
        }

        PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_IDLE);
        PlayerStatePacketServices.PlayerState.NotifyEvent(Event.WAIT_RETURN, NewIdleState.WaitReturnFromCombatSmb);
    }
}
