using UnityEngine;

public partial class NewMagicSkillsState  : AbstractAttackEvents
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void HandleEvent(Event evt, object payload = null)
    {
        MagicSkillUseDto useSkill = GetPayload(payload);
        PlayerEntity entity = _stateMachine.Player;
        int objectId = entity.Identity.Id;

        switch (evt)
        {
            case Event.READY_TO_ACT:
            {
                Skillgrp readyGrp = useSkill.SkillGrp
                    ?? TablePacketServices.SkillGrps?.GetSkill(useSkill.SkillId, useSkill.SkillLvl);
                AnimationCombo readyCombo = TablePacketServices.SkillGrps != null
                    ? TablePacketServices.SkillGrps.GetAnimComboBySkillId(useSkill.SkillId, useSkill.SkillLvl)
                    : null;
                string animTag = readyGrp != null ? readyGrp.Animation : "-";
                string cycleDump = readyCombo != null
                    ? string.Join(",", readyCombo.GetAnimCycle() ?? System.Array.Empty<string>())
                    : "-";
                string animatorName = "-";
                try
                {
                    var ctrl = ControllerPacketServices.Animations?.GetRegisteredController(objectId);
                    animatorName = ctrl != null ? ctrl.GetAnimatorName() : "no-ctrl";
                }
                catch (System.Exception) { animatorName = "err"; }

                if (readyCombo == null || SkillExecutorPacketServices.Executor == null)
                {
                    Debug.LogWarning(
                        "[MagicSkills] READY_TO_ACT FAIL skill=" + useSkill.SkillId +
                        " lv=" + useSkill.SkillLvl +
                        " hit=" + useSkill.HitTime +
                        " grp=" + (readyGrp != null) +
                        " anim=[" + animTag + "]" +
                        " combo=" + (readyCombo != null) +
                        " executor=" + (SkillExecutorPacketServices.Executor != null) +
                        " animator=" + animatorName);
                    break;
                }

                string[] orderedReadyCycle = SetupDurationHelper.BuildOrderedCycleForOverrideTiming(readyCombo.GetAnimCycle());
                float[] durations = ControllerPacketServices.Animations.GetOverrideClipsDurations(objectId, orderedReadyCycle);
                string durDump = durations != null ? string.Join(",", durations) : "-";
                float shotEventTime = SetupDurationHelper.ResolveShotEventTime(objectId, orderedReadyCycle);
                float flightTimeMs = SetupDurationHelper.ResolveMagicFlightTimeMs(
                    entity, useSkill.SkillId, entity != null ? entity.Target : null);
                Debug.Log(
                    "[CastSync] READY_TO_ACT skill=" + useSkill.SkillId +
                    " lv=" + useSkill.SkillLvl +
                    " hitMs=" + useSkill.HitTime +
                    " flightMs=" + flightTimeMs.ToString("F0") +
                    " shotEvt=" + shotEventTime.ToString("F3") +
                    " anim=[" + animTag + "]" +
                    " cycle=" + cycleDump +
                    " ordered=" + string.Join(",", orderedReadyCycle ?? System.Array.Empty<string>()) +
                    " durations=" + durDump +
                    " animator=" + animatorName);

                entity.SetupTotalCastDuration(useSkill.HitTime, flightTimeMs, durations, shotEventTime, useSkill.TargetId, SetupDurationHelper.CastStyleOf(useSkill), SetupDurationHelper.MixComboOf(useSkill));

                _ = SkillExecutorPacketServices.Executor.ExecuteSkillOverride(readyGrp, entity, readyCombo, _events);
                break;
            }
            case Event.CANCEL:
                MagicSkillPresentation.EndCastChannel();
                break;
            case Event.APPLY_SELF_SKILL:
            {
                if (SetupDurationHelper.IsUsePotion(useSkill))
                {
                    SetupDurationHelper.FinishPotionUse(_stateMachine, entity, useSkill);
                    break;
                }

                Skillgrp selfGrp = useSkill.SkillGrp
                    ?? TablePacketServices.SkillGrps?.GetSkill(useSkill.SkillId, useSkill.SkillLvl);
                AnimationCombo selfCombo = TablePacketServices.SkillGrps != null
                    ? TablePacketServices.SkillGrps.GetAnimComboBySkillId(useSkill.SkillId, useSkill.SkillLvl)
                    : null;
                if (selfCombo == null || SkillExecutorPacketServices.Executor == null)
                {
                    Debug.LogWarning(
                        "[MagicSkills] APPLY_SELF missing combo/executor skill=" + useSkill.SkillId +
                        " lv=" + useSkill.SkillLvl);
                    break;
                }

                SetupDurationHelper.SetupDurationIfHitTimeNot0(useSkill, objectId, entity, selfCombo);
                _ = SkillExecutorPacketServices.Executor.ExecuteSkillOverride(selfGrp, entity, selfCombo, _events);
                break;
            }

        }
    }
}
