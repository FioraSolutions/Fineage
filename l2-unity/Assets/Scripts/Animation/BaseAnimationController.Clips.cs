using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseAnimationController
{
    public void SetAnimationProperty(int animId, float value)
    {
        SetAnimationProperty(animId, value, false);
    }

    // Update animator variable based on Animation Id
    public void SetAnimationProperty(int animId, float value, bool forceReset)
    {
        if (!TryGetAnimator())
            return;
        //Debug.Log("animId " + animId + "/" + _animator.parameters.Length);
        if (animId >= 0 && animId < _animator.parameters.Length)
        {
            if (_resetStateOnReceive || forceReset)
            {
                ClearAnimParams();
            }

            AnimatorControllerParameter anim = _animator.parameters[animId];

            switch (anim.type)
            {
                case AnimatorControllerParameterType.Float:
                    DriveFloat(anim.name, value);
                    break;
                case AnimatorControllerParameterType.Int:
                    DriveInt(anim.name, (int)value);
                    break;
                case AnimatorControllerParameterType.Bool:
                    SetBool(anim.name, value == 1f);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    DriveTrigger(anim.name);
                    break;
            }
        }
    }

    // Return an animator variable based on its ID
    public float GetAnimationProperty(int animId)
    {
        if (!TryGetAnimator())
            return 0f;
        if (animId >= 0 && animId < _animator.parameters.Length)
        {
            AnimatorControllerParameter anim = _animator.parameters[animId];

            switch (anim.type)
            {
                case AnimatorControllerParameterType.Float:
                    return _animator.GetFloat(anim.name);
                case AnimatorControllerParameterType.Int:
                    return (int)_animator.GetFloat(anim.name);
                case AnimatorControllerParameterType.Bool:
                    return _animator.GetBool(anim.name) == true ? 1f : 0;
            }
        }

        return 0f;
    }

    public void SetFloat(string name, float value)
    {
        if (!TryGetAnimator())
            return;
        DriveFloat(name, value);
    }

    public float GetFloat(string name)
    {
        if (!TryGetAnimator())
            return 0f;
        return  _animator.GetFloat(_animator.name);
    }

    public void SetInt(string name, int value)
    {
        if (!TryGetAnimator())
            return;
        DriveInt(name, value);
    }

    public int GetInt(string name)
    {
        if (!TryGetAnimator())
            return 0;
        return _animator.GetInteger(_animator.name);
    }

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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
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
#endif
}
