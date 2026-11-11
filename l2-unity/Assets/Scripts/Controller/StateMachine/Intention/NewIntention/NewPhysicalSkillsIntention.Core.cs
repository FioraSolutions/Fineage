using System;
using UnityEngine;

public partial class NewPhysicalSkillsIntention : IntentionBase
{
    public NewPhysicalSkillsIntention(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter(object arg0)
    {
        if (arg0.GetType() == typeof(MagicSkillUseDto))
        {
            MagicSkillUseDto useSkill = (MagicSkillUseDto)arg0;

            if (WeaponChargeSkill.IsCharge(useSkill.SkillId))
            {
                // Backup path: charge must never leave current state (ATTACK/RUN/WALK/IDLE).
                PlayerState prevState = _stateMachine.State;
                Intention prevIntention = _stateMachine.Intention;
                ApplySoulshotCharge(useSkill);
                // Debug.Log(
                    // $"{SS_SM_LOG} Intention backup charge skillId={useSkill.SkillId} " +
                    // $"keepState={prevState} intentionNow={_stateMachine.Intention} " +
                    // $"(prefer GameServerPacketHandler.ApplyWeaponChargeShot)");
                return;
            }


            // Debug.Log("NewPhysicalSkillsIntention > use " + useSkill.SkillId);
            if (SetupDurationHelper.IsUsePotion(useSkill))
            {
                SetupDurationHelper.FinishPotionUse(_stateMachine, _stateMachine.Player, useSkill);
                return;
            }

            int objectId = _stateMachine.Player.Identity.Id;
            ControllerPacketServices.Animations.SetSpTimeAtk(objectId , useSkill.HitTime);
            Entity targetEntity = ControllerPacketServices.GameWorld.GetEntityNoLockSync(useSkill.TargetId);
            // Bow SpAtk: live follow until shoot. Other phys skills: one-shot snapshot.
            if (objectId != useSkill.TargetId &&
                targetEntity != null &&
                CombatFacingService.IsPlayerUsingBow(DomainPacketServices.LocalPlayer))
            {
                CombatPacketServices.EnsureFacing().BeginFollow(
                    objectId,
                    ControllerPacketServices.Player.transform,
                    targetEntity.transform);
            }
            else if (targetEntity != null)
            {
                CombatPacketServices.Facing?.EndFollow(objectId, "non-bow-phys");
                ControllerPacketServices.Player.RotateToAttacker(targetEntity.transform.position);
            }

            IfUseSelf(objectId, useSkill);

        }
    }

    private void ApplySoulshotCharge(MagicSkillUseDto useSkill)
    {
        if (_stateMachine.Player == null) return;

        _stateMachine.Player.IsSoulshotCharged = true;
        WeaponChargeSkillPlayback.Play(
            ControllerPacketServices.Effects,
            useSkill.SkillId,
            _stateMachine.Player.GetWeaponTransform(),
            _stateMachine.Player.transform);

        // Debug.Log(
            // $"{SS_SM_LOG} ApplySoulshotCharge skillId={useSkill.SkillId} " +
            // $"state={_stateMachine.State} IsSoulshotCharged=True");
    }
}
