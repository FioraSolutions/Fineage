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

                AnimationCombo animComboAct = TablePacketServices.SkillGrps.GetAnimComboBySkillId(useSkill.SkillId, useSkill.SkillLvl);
                //not use bow atk
                //RotateFaceToMonster(_stateMachine.Player);
                _ = SkillExecutorPacketServices.Executor.ExecuteSkill(_stateMachine.Player, animComboAct, _events);
                // Only skills with a ready EffectDatabase entry (e.g. Power Strike = 3).
                if (useSkill.SkillId == 3)
                {
                    EffectManager effects = ControllerPacketServices.Effects;
                    if (effects != null)
                    {
                        effects.PlayEffectSyncedToSkillAnimation(
                            useSkill.SkillId,
                            _stateMachine.Player,
                            useSkill.HitTime,
                            animComboAct);
                    }
                }
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
                    Entity entity = _stateMachine.Player;
                    if (useSkill.SkillId == 2046)
                    {
                        string[] cycle = castCombo.GetAnimCycle();
                        Debug.Log(
                            "[SUMMON_ANIM] APPLY_SELF branch=combo skill=2046" +
                            " anim=" + useSkill.SkillGrp.Animation +
                            " hitMs=" + useSkill.HitTime +
                            " cycle=" + (cycle == null ? "null" : string.Join(",", cycle)) +
                            " state=" + _stateMachine.State);
                    }
                    SetupDurationHelper.SetupDurationIfHitTimeNot0(
                        useSkill, entity.Identity.Id, entity, castCombo);
                    _ = SkillExecutorPacketServices.Executor.ExecuteSkillOverride(
                        useSkill.SkillGrp, entity, castCombo, _events);
                }
                else if (skillgrp != null && SkillExecutorPacketServices.Executor != null)
                {
                    AnimationCombo combo = new AnimationCombo("-1", new string[1] { skillgrp.GetAnimOperationType3() }, "");
                    _ = SkillExecutorPacketServices.Executor.ExecuteSkill(_stateMachine.Player, combo, _events);
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
}
