using UnityEngine;

public class NewAttackIntention : IntentionBase
{
    public NewAttackIntention(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter(object arg0)
    {
        L2PawnRange.ClearIgnoredPawn(DomainPacketServices.LocalPlayer);
        if (!IsSuccessAttack(arg0))
        {
            _stateMachine.ChangeIntention(Intention.INTENTION_IDLE); 
            return;
        }
        


        if (arg0.GetType() == typeof(AttackDto))
        {
            // Debug.Log("NewAttackIntention > State " + PlayerStatePacketServices.PlayerState.State);
            AttackDto myModel = (AttackDto)arg0;
            int targetId = myModel.TargetId;

            Entity targetEntity = ControllerPacketServices.GameWorld.GetEntityNoLockSync(targetId);

            if (targetEntity != null)
            {
                SelectPacketTarget(targetEntity);
                // ReplayAttackTrace.Write(
                    // "TARGET_SELECTED",
                    // DomainPacketServices.LocalPlayer,
                    // targetEntity,
                    // "playerTarget=" +
                    // (DomainPacketServices.LocalPlayer != null && DomainPacketServices.LocalPlayer.Target != null));

                // Bow: live follow until OnAnimationShoot. Melee: one-shot snapshot only.
                if (CombatFacingService.IsPlayerUsingBow(DomainPacketServices.LocalPlayer))
                {
                    int localId = DomainPacketServices.LocalPlayer.Identity.Id;
                    CombatPacketServices.EnsureFacing().BeginFollow(
                        localId,
                        ControllerPacketServices.Player.transform,
                        targetEntity.transform);
                }
                else
                {
                    CombatPacketServices.Facing?.EndFollowLocal("melee-snapshot");
                    ControllerPacketServices.Player.RotateToAttacker(targetEntity.transform.position);
                }
            }

            Hit playerHit = myModel.FirstHit;
            if (targetEntity == null || playerHit == null)
            {
                _stateMachine.ChangeIntention(Intention.INTENTION_IDLE);
                return;
            }

            // Attack carries visual damage only. The official client changes health from
            // StatusUpdate and enters death only from Die; it doesn't predict death here.
            var hits = CombatPacketServices.Hits;
            if (hits != null)
                hits.RememberLocalAttack(myModel);

            DomainPacketServices.LocalPlayer.IsAttack = true;
            if (ControllerPacketServices.Targets != null)
                ControllerPacketServices.Targets.SetAttackTarget();
            DomainPacketServices.LocalPlayer.SetSelfHit(playerHit);
            // AttackDto packet SS flag — recharge every swing (otherwise only first Hit shows SoulShot).
            DomainPacketServices.LocalPlayer.IsSoulshotCharged = playerHit.hasSoulshot();
            // Debug.Log(
                // $"[ATK_HIT_CHAIN] 0.AttackPacket ss={playerHit.hasSoulshot()} miss={playerHit.isMiss()} " +
                // $"crit={playerHit.isCrit()} dmg={playerHit.Damage}");

            if (_stateMachine.State != PlayerState.ATTACKING)
            {
                _stateMachine.ChangeState(PlayerState.ATTACKING);
            }
            // ReplayAttackTrace.Write(
                // "READY_TO_ACT",
                // DomainPacketServices.LocalPlayer,
                // targetEntity,
                // "damage=" + playerHit.Damage);
            _stateMachine.NotifyEvent(Event.READY_TO_ACT, targetEntity);

        }
    }

    static void SelectPacketTarget(Entity targetEntity)
    {
        if (ControllerPacketServices.Targets != null)
        {
            ControllerPacketServices.Targets.SetTarget(new ObjectData(targetEntity.gameObject), "#ffffff");
            return;
        }

        if (DomainPacketServices.LocalPlayer == null)
            return;
        DomainPacketServices.LocalPlayer.TargetId = targetEntity.Identity != null ? targetEntity.Identity.Id : 0;
        DomainPacketServices.LocalPlayer.Target = targetEntity.transform;
    }





    public override void Exit() { }
    public override void Update()
    {

    }
}