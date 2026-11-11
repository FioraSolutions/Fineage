using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public partial class AnimationManager
{
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
