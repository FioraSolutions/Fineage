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
                // Every physical skill that has art must play it, not just Power Strike (3):
                // Deadly Blow / Lethal Blow animated the swing and stayed invisible.
                SkillVisualEffectDispatch.PlayIfKnown(
                    _stateMachine.Player, useSkill, animComboAct);
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
                    // Physical self skills (Ultimate Evasion X, Dash X, Focus Chance X, Escape Shackle X)
                    // are tags that map to SpAtk06 → the shared SpAtk06_hand buff gesture on the
                    // template controller (each race override points it at its own
                    // *_ao_SpAtk06_Hand_* clip). The override runner would map that single clip to
                    // the "CastEnd" trigger, which plays the magic cast pose and never fires the
                    // SpAtk SMB — the hero stands frozen. StartRun plays the combat CrossFade
                    // instead, which resolves SpAtk06_<weapon> to SpAtk06_hand.
                    _ = SkillExecutorPacketServices.Executor.ExecuteSkill(
                        entity, castCombo, _events);
                    SkillVisualEffectDispatch.PlayIfKnown(entity, useSkill, castCombo);
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
