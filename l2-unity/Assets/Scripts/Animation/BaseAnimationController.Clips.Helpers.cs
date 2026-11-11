using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseAnimationController
{
    public void SetAnimatorSpeed(float value)
    {
        if (!TryGetAnimator())
            return;
        _animator.speed = value;
        _castPlayable?.SetControllerSpeed(value);
    }

    public void SetEnabled(bool value)
    {
        enabled = value;
    }

    public string GetAnimatorName()
    {
       return _animator?.runtimeAnimatorController.name;
    }

    public void ReplaceAnimClip(string animName , string overrideAnimName)
    {
        string raceName = _animator?.runtimeAnimatorController.name;
        AnimationClip clip = SkillAnimationDatabase.GetOverrideClip(overrideAnimName , raceName);
        bool hasSlot = HasClip(animName);
        if (SkillAnimationRunner.Trace)
        {
            Debug.Log(
                "[SUMMON_ANIM] ReplaceAnimClip slot=" + animName +
                " override=" + overrideAnimName +
                " race=" + raceName +
                " clip=" + (clip != null ? clip.name : "null") +
                " hasSlot=" + hasSlot);
        }
        if (HasClip(animName))
        {
            _overrideController[animName] = clip;
            // Debug.Log("BaseAnimationController>ReplaceAnimClip clip orig   " + animName + " replace name " + clip.name);
        }
        else
        {
            // Debug.LogWarning("BaseAnimationController>ReplaceAnimClip not found replace anim  " + animName);
        }

    }

    public bool HasClip(string originalClipName)
    {
        // Проверяем, есть ли вообще такой "слот" для замены
        return _overrideController[originalClipName] != null;
    }

    public float GetEventTimeByName(AnimationClip clip, string eventName)
    {
        return AnimationDataCache.GetEventTimeByName(_animator, clip, eventName);
    }

    public override void OnAnimationShoot(string animationName)
    {
        if (!TryGetAnimator())
        {
            base.OnAnimationShoot(animationName);
            return;
        }

        int objectId = _animator.GetInteger(AnimatorUtils.OBJECT_ID);

        if (Time.frameCount == _lastDuplicateShootFrame &&
            string.Equals(_lastDuplicateShootAnim, animationName, StringComparison.Ordinal))
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            AnimationManager.LogAnimationShootFromAnimator(
                objectId,
                "SKIP_DEDUP_SAME_FRAME",
                animationName,
                BuildAnimatorShootDebugContext());
#endif
            return;
        }

        _lastDuplicateShootFrame = Time.frameCount;
        _lastDuplicateShootAnim = animationName;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        AnimationManager.LogAnimationShootFromAnimator(
            objectId,
            "DISPATCH",
            animationName,
            BuildAnimatorShootDebugContext());
#endif

        base.OnAnimationShoot(animationName);
    }

    private string BuildAnimatorShootDebugContext()
    {
        AnimatorStateInfo s0 = _animator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] clipInfos = _animator.GetCurrentAnimatorClipInfo(0);
        string clip0 = clipInfos.Length > 0 && clipInfos[0].clip != null ? clipInfos[0].clip.name : "(no clip info)";
        float clipLen = clipInfos.Length > 0 && clipInfos[0].clip != null ? clipInfos[0].clip.length : -1f;

        int layerCount = _animator.layerCount;
        string layer1 = "";
        if (layerCount > 1)
        {
            AnimatorStateInfo s1 = _animator.GetCurrentAnimatorStateInfo(1);
            AnimatorClipInfo[] c1 = _animator.GetCurrentAnimatorClipInfo(1);
            string n1 = c1.Length > 0 && c1[0].clip != null ? c1[0].clip.name : "?";
            layer1 = $" | L1 clip={n1} normT={s1.normalizedTime:F3}";
        }

        return
            $"controller='{GetAnimatorName()}' L0 clip={clip0} clipLen={clipLen:F3}s " +
            $"normT={s0.normalizedTime:F3} speed={_animator.speed:F3} shortNameHash={s0.shortNameHash}" +
            layer1;
    }
}
