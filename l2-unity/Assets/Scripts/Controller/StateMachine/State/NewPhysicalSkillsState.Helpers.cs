using System;
using System.Threading;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class NewPhysicalSkillsState : AbstractAttackEvents
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void HandleEvent(Event evt , object payload = null)
    {
        MagicSkillUseDto useSkill = GetPayload(payload);


        switch (evt)
        {
            case Event.READY_TO_ACT:
                PlayMixCast(useSkill);
                break;
            case Event.CANCEL:
                // Debug.Log("NewPhysicalSkillsState Use Sate> Отмена скорее всего запрос пришел из ActionFaild");

                break;
            case Event.APPLY_SELF_SKILL:
                Skillgrp skillgrp = useSkill.SkillGrp;
                if (SetupDurationHelper.IsUsePotion(useSkill))
                {
                    SetupDurationHelper.FinishPotionUse(_stateMachine, _stateMachine.Player, useSkill);
                }
                else if (SetupDurationHelper.TryGetPlayableCombo(useSkill, out AnimationCombo castCombo) &&
                         useSkill.SkillGrp != null &&
                         SkillExecutorPacketServices.Executor != null)
                {
                    if (SkillPresentationCatalog.IsTwistDoomPhysical(useSkill.SkillId))
                    {
                        string[] cycle = castCombo.GetAnimCycle();
                        Debug.Log(
                            "[SUMMON_ANIM] APPLY_SELF branch=combo skill=2046" +
                            " anim=" + useSkill.SkillGrp.Animation +
                            " hitMs=" + useSkill.HitTime +
                            " cycle=" + (cycle == null ? "null" : string.Join(",", cycle)) +
                            " state=" + _stateMachine.State);
                    }
                    // Same SetupDuration / MagicCastData path as a targeted Mix cast.
                    PlayMixCast(useSkill);
                }
                else if (skillgrp != null)
                {
                    PhysicalSkillPresentation.TryPlay(_stateMachine.Player, useSkill, _events);
                }


                break;
            case Event.APPLY_SOULSHOT_CHARGED:
                // Debug.Log(
                    // $"[SS_CHARGE_SM] State.APPLY_SOULSHOT_CHARGED state={_stateMachine.State} " +
                    // $"intention={_stateMachine.Intention} running={_stateMachine.Player != null && _stateMachine.Player.Running}");
                _stateMachine.Player.IsSoulshotCharged = true;
                WeaponChargeSkillPlayback.Play(
                    ControllerPacketServices.Effects,
                    useSkill.SkillId,
                    _stateMachine.Player.GetWeaponTransform(),
                    _stateMachine.Player.transform);
                break;

            case Event.ARRIVED:
                // Arrived while charged / physical-skills — locomotion finished, go IDLE.
                // Debug.Log(
                    // $"[SS_CHARGE_SM] State.ARRIVED while PHYSICAL_SKILLS → IDLE " +
                    // $"(arrived overrides charged)");
                PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_IDLE);
                PlayerStatePacketServices.PlayerState.NotifyEvent(Event.ARRIVED);
                break;

        }
    }

    /// <summary>
    /// Targeted PHYS (READY_TO_ACT) and self PHYS (APPLY_SELF_SKILL) share this cast.
    /// Mix combo stretch in <see cref="MagicCastData"/> fits every SpAtk phase to hit time.
    /// </summary>
    void PlayMixCast(MagicSkillUseDto useSkill)
    {
        Entity entity = _stateMachine.Player;
        AnimationCombo combo = null;
        if (entity != null && entity.Identity != null &&
            SetupDurationHelper.TryGetPlayableCombo(useSkill, out combo))
        {
            SetupDurationHelper.SetupDurationIfHitTimeNot0(
                useSkill, entity.Identity.Id, entity, combo);
        }

        PhysicalSkillPresentation.TryPlay(entity, useSkill, _events);
        SkillVisualEffectDispatch.PlayIfKnown(entity, useSkill, combo);
    }
}
