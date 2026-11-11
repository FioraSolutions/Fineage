using System;
using System.Collections.Generic;
using UnityEngine;

public partial class AnimationManager
{
    public void PlayMonsterAnimation(int objectId, string animationName)
    {
        PlayMonsterAnimation(objectId, animationName, null);
    }

    public void PlayMonsterAnimation(int objectId, string animationName, float? fixedDuration)
    {
        IAnimationController controller = GetMonsterController(objectId);
        if (controller == null)
        {
            // Debug.LogWarning(
                // $"[ANIM] PlayMonster MISS id={objectId} request='{animationName}' reason=no_controller");
            return;
        }

        if (MonsterAnim.TryResolve(animationName, out string stateName, out MonsterAnimState family))
        {
            SetMonsterRecentName(objectId, stateName);
            float duration = fixedDuration ?? LocomotionCrossFadeSettings.ResolveDuration(stateName, controller);
            bool already = PlayerLocomotionCrossFade.IsAlreadyPlaying(controller, stateName);
            bool played = MonsterCrossFade.TryPlay(controller, stateName, family, duration);

            return;
        }

        SetMonsterRecentName(objectId, animationName);
        float rawDuration = fixedDuration ?? LocomotionCrossFadeSettings.ResolveDuration(animationName, controller);
        // Debug.Log(
            // $"[ANIM] PlayMonster RAW id={objectId} request='{animationName}' duration={rawDuration:F3}s");
        MonsterCrossFade.TryPlayRaw(controller, animationName, rawDuration);
    }

    public void StopMonsterCurrentAnimation(int objectId, string animationName)
    {
        if (GetMonsterController(objectId) is { } controller)
        {
            controller.SetBool(animationName, false);
        }
        else
        {
            // Debug.LogWarning($"AnimationManager->StopMonsterCurrentAnimation: Не критическая ошибка - animator not found for monster {objectId}. Animation: {animationName}");
        }
    }

    public void SetSpTimeAtk(int objectId, int timeAtk)
    {
        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null)
            return;

        controller.SetInt(SP_TIME_ATK, timeAtk);
    }

    public void ResetPlayerAnimatorSpeed(int objectId, float speed = 1f)
    {
        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null)
        {
            // Debug.LogWarning($"[AnimSpeed] reset skipped objectId={objectId} reason=no_controller");
            return;
        }

        controller.SetAnimatorSpeed(speed);
        // Debug.Log($"[AnimSpeed] reset objectId={objectId} speed={speed:F3}");
    }

    public void SetPAtkSpeed(int objectId, float patkspd)
    {
        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null)
            return;

        controller.SetPAtkSpeed(patkspd);
    }

    public float ApplyLinearMeleePAtkSpeed(int objectId, string animName, float clipLengthSec)
    {
        IAnimationController controller = GetRegisteredController(objectId);
        Entity entity = GetEntity(objectId);
        if (controller == null)
            return 1f;

        float clipSec = clipLengthSec;
        if (clipSec <= 0f)
            clipSec = ResolveAttackClipLength(controller.GetAnimator(), animName);

        float cycleMs = AttackTimingHelper.ResolveAttackCycleMs(entity, animName);
        float linear = AttackTimingHelper.ComputeLinearPAtkSpeed(clipSec, cycleMs);
        controller.SetPAtkSpeed(linear);

        // Debug.Log(
            // $"[PATKSPD] ApplyLinear objectId={objectId} entity={entity?.name} anim={animName} " +
            // $"pAtkSpd={AttackTimingHelper.ResolvePAtkSpd(entity):F1} cycleMs={cycleMs:F1} " +
            // $"clipSec={clipSec:F3} patkspd={linear:F3}");
        return linear;
    }
}
