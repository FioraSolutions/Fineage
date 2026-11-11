using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

public partial class SkillExecutor : MonoBehaviour
{
    public async Task ExecuteSkill(Entity entity , AnimationCombo animationCombo , AnimationEventsBase actions)
    {
        if (entity == null || animationCombo == null) return;
        int objectId = entity.Identity.Id;


        // _emitter.SetupActions(actions);

        string[] cycle = animationCombo.GetAnimCycle();
        await _animRunner.StartRun(cycle, objectId, Animations, () => OnAllAnimationFinish(actions));
    }

    public async Task ExecuteSkillOverride(Skillgrp skill, Entity entity, AnimationCombo animationCombo, AnimationEventsBase actions)
    {
        if (entity == null || animationCombo == null) return;
        int objectId = entity.Identity.Id;

        EffectManager effects = Effects;
        if (effects != null && skill != null)
        {
            MagicCastData cast = entity.GetMagicCastData();
            Debug.Log(
                "[ShotOrb] ExecuteOverride PlayEffect skill=" + skill.Id +
                " entity=" + entity.name +
                " cast=" + (cast != null) +
                " shoot=" + (cast != null ? cast.serverTimeToShoot.ToString("0.###") : "-") +
                " targetId=" + (cast != null ? cast.TargetObjectId.ToString() : "-"));
            effects.PlayEffect(skill.Id, entity.transform, cast);
        }
        else
        {
            Debug.LogWarning(
                "[ShotOrb] ExecuteOverride SKIP PlayEffect effects=" + (effects != null) +
                " skill=" + (skill != null ? skill.Id.ToString() : "null"));
        }

        string[] cycle = animationCombo.GetAnimCycle();
        SkillAnimationRunner.Trace = skill != null && skill.Id == 2046;
        if (SkillAnimationRunner.Trace)
        {
            Debug.Log(
                "[SUMMON_ANIM] ExecuteOverride objectId=" + objectId +
                " anim=" + skill.Animation +
                " isMagic=" + skill.IsMagic +
                " hitGrp=" + skill.HitTime +
                " cycle=" + (cycle == null ? "null" : string.Join(",", cycle)) +
                " runner=" + (_animRunner != null) +
                " animations=" + (Animations != null));
        }
        await _animRunner.StartRunOverride(cycle, objectId, Animations, () => OnAllAnimationFinish(actions));
    }

    private void OnAllAnimationFinish(AnimationEventsBase actions)
    {
        OnAllAnimationFinished?.Invoke(actions);
    }
}
