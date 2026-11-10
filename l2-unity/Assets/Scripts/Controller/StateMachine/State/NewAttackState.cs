
using UnityEngine;



public class NewAttackState : AbstractAttackEvents
{

  
    public NewAttackState(PlayerStateMachine stateMachine) :
        base(stateMachine.GetObjectId() , 
        SpecialAnimationNames.GetSpecialsAttackAnimations() , 
        stateMachine)
    {

    }


    public override void Update()
    {

    }

    public override void HandleEvent(Event evt , object payload = null)
    {
        switch (evt)
        {
            case Event.READY_TO_ACT:
                // Debug.Log("Attack Sate to Intention> начало новой atk пришел запрос от сервера");
                Entity targetEntity = payload as Entity;
                if (targetEntity == null && _stateMachine.Player != null)
                    targetEntity = _stateMachine.Player.GetTargetEntity();
                if (targetEntity == null)
                {
                    // Missing pawn. A dead pawn still plays the swing.
                    if (DomainPacketServices.LocalPlayer != null)
                    {
                        DomainPacketServices.LocalPlayer.IsAttack = false;
                    }
                    PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_IDLE);
                    PlayerStatePacketServices.PlayerState.NotifyEvent(Event.WAIT_RETURN);
                    break;
                }

                // Spine/arm aim-at-target. Disabled: keep attack pose from the clip only.
                // AttackTimingHelper.RotateFaceToMonster(_stateMachine.Player);

                int targetEntityId = targetEntity.Identity != null ? targetEntity.Identity.Id : 0;
                float attackDurationMs = AttackTimingHelper.ResolveServerLikeAttackDurationMs(_stateMachine.Player);
                float hitFraction = AttackTimingHelper.ResolveHitFractionByWeapon(_stateMachine.Player);
                float pAtkSpd = _stateMachine.Player.Stats != null ? _stateMachine.Player.Stats.BasePAtkSpeed : 0f;
                // ReplayAttackTrace.Write(
                    // "START_ANIMATION",
                    // _stateMachine.Player,
                    // targetEntity,
                    // "cycleMs=" + attackDurationMs.ToString("F1") +
                    // " hitFraction=" + hitFraction.ToString("F3"));

                // Debug.Log(
                    // $"[ATK_TIMING_CMP] BeginAttack packet→serverCycle " +
                    // $"pAtkSpd={pAtkSpd:F1} serverTimeAtkMs={attackDurationMs:F1} " +
                    // $"(L2J 500000/pAtkSpd) serverHitMs={attackDurationMs * hitFraction:F1} " +
                    // $"hitFraction={hitFraction:F2} targetId={targetEntityId}");

                SwordCollisionPacketServices.Collision.BeginAttack(
                    _stateMachine.Player.Identity.Id,
                    targetEntityId,
                    _stateMachine.Player.transform,
                    targetEntity.transform,
                    attackDurationMs,
                    hitFraction);

                // Melee Hit/SoulShot: AttackShot on jatk*_1HS/_2HS/_dual/_pole (not wall-clock).
                // RegisterSwordCollision(_stateMachine.Player);

                DomainPacketServices.LocalPlayer.RefreshRandomPAttack();
                Animation random = DomainPacketServices.LocalPlayer.RandomName;
                ControllerPacketServices.Animations.PlayAnimationTrigger(_stateMachine.GetObjectId() , random.ToString());

                break;
            case Event.WAIT_RETURN:
                // WhoDied while still ATTACKING: clear latch only. Pose returns at swing end
                // (PlayerStateJAtk.SwitchToIdle). Forcing atkwait here cuts the finishing blow.
                // WaitReturnLog.Dump("NewAttackState.WAIT_RETURN latch-only");
                if (DomainPacketServices.LocalPlayer != null)
                {
                    DomainPacketServices.LocalPlayer.IsAttack = false;
                    DomainPacketServices.LocalPlayer.LastAtkAnimation = null;
                }
                break;
            case Event.CANCEL:
                // Debug.Log("Attack Sate to Intention> Отмена скорее всего запрос пришел из ActionFaild");
                PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_IDLE);
                PlayerStatePacketServices.PlayerState.NotifyEvent(Event.WAIT_RETURN);
                DomainPacketServices.LocalPlayer.LastAtkAnimation = null;
                break;

        }
    }
}
