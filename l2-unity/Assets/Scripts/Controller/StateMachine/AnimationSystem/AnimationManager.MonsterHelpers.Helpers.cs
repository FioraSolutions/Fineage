using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public partial class AnimationManager
{
    public bool TryPlayExactAnimatorState(int objectId, string stateName, out float duration)
    {
        duration = 0f;
        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null || string.IsNullOrEmpty(stateName))
        {
            Debug.LogError(
                "[SocialAnim] TryPlayExactAnimatorState objectId=" + objectId +
                " state=" + (stateName ?? "(null)") + " missing animation controller");
            return false;
        }

        Animator animator = controller.GetAnimator();
        if (animator == null)
        {
            Debug.LogError(
                "[SocialAnim] TryPlayExactAnimatorState objectId=" + objectId +
                " state=" + stateName + " missing Animator");
            return false;
        }

        if (!animator.HasState(0, Animator.StringToHash(stateName)))
        {
            Debug.LogError(
                "[SocialAnim] TryPlayExactAnimatorState objectId=" + objectId +
                " state=" + stateName + " not in controller=" +
                (animator.runtimeAnimatorController != null
                    ? animator.runtimeAnimatorController.name
                    : "(null)"));
            return false;
        }

        duration = PlayExactAnimatorState(objectId, stateName);
        return true;
    }

    public float PlayExactAnimatorState(int objectId, string stateName, bool snapToEnd = false)
    {
        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null || string.IsNullOrEmpty(stateName))
            return 1.6f;

        AnimationEventForwarder.BindAnimator(controller);
        controller.SetEnabled(true);
        DesableLastPlayerAnimationElseTrue(objectId, controller);
        SetRecentName(objectId, stateName);

        bool isPickup = string.Equals(stateName, "pickup", System.StringComparison.OrdinalIgnoreCase);
        // Override clip is PicItem_*, not "pickup" — resolve by PicItem for length.
        float clipLength = isPickup
            ? GetClipLengthByName(objectId, "PicItem", AnimPlayback.PickupFallbackSeconds)
            : GetClipLengthByName(objectId, stateName, 1.6f);

        // PicItem: authored timing only (Interlude ~0.17–0.25s). Not bound to server clocks.
        AnimPlayback.PrepareGesture(controller);

        Animator animator = controller.GetAnimator();
        if (snapToEnd && animator != null)
        {
            animator.Play(stateName, 0, 1f);
            animator.Update(0f);
        }
        else
        {
            controller.CrossFadeInFixedTime(stateName, LocomotionCrossFadeSettings.FixedDuration);
        }

        if (isPickup)
            return AnimPlayback.PickupPlaySeconds(clipLength);
        return Mathf.Max(0.4f, clipLength);
    }

    public float PlayGesture(int objectId, string stateName)
    {
        return PlayExactAnimatorState(objectId, stateName, snapToEnd: false);
    }

    public float[] GetOverrideClipsDurations(int objectId, string[]cycle)
    {
        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null)
            return new float[cycle != null ? cycle.Length : 0];

        float[] durations = new float[cycle.Length];

        using (MagicSkillUseTrace.Measure("anim.clipLookup"))
        {
            for(int i=0; i< cycle.Length; i++)
            {
                AnimationClip clip = ResolveTimingClip(objectId, controller, cycle[i], out _);
                if(clip != null)
                {
                    durations[i] = clip.length;
                }
            }
        }

        return durations;
    }

    public float GetOverrideEventTimeByName(int objectId, string[] cycle, string eventName)
    {
        IAnimationController controller = GetRegisteredController(objectId);
        if (controller == null || cycle == null)
            return 0;

        // The pin is the last phase that actually has a shot notify (MagicShot, or the
        // last SpAtk of a Mix). An earlier swing's AttackShot must not steal that time.
        float found = 0f;
        for (int i = 0; i < cycle.Length; i++)
        {
            AnimationClip clip = ResolveTimingClip(objectId, controller, cycle[i], out string stateName);
            if (clip == null)
                continue;

            float time = controller.GetEventTimeByName(clip, eventName);
            if (time == 0f && IsShotEventName(eventName))
                time = controller.GetEventTimeByName(clip, "AttackShot");
            if (time == 0f && IsShotEventName(eventName))
                time = SkillAnimationDatabase.OfficialAttackShotSeconds(stateName, controller.GetAnimatorName(), clip.length);
            if (time != 0f)
                found = time;
        }

        return found;
    }

    AnimationClip ResolveTimingClip(
        int objectId,
        IAnimationController controller,
        string token,
        out string stateName)
    {
        stateName = token;
        if (controller == null || string.IsNullOrEmpty(token))
            return null;

        string race = controller.GetAnimatorName();
        Entity timingEntity = GetEntity(objectId);
        int timingSkill = timingEntity != null && timingEntity.GetMagicCastData() != null
            ? timingEntity.GetMagicCastData().SkillId
            : 0;
        SkillAnimationResolver.SetActiveSkill(timingSkill);
        bool spatk = token.IndexOf("SpAtk", StringComparison.OrdinalIgnoreCase) >= 0;
        if (!spatk)
            return SkillAnimationDatabase.GetOverrideClip(token, race);

        if (!PlayerSpAtkAnim.TryParse(token, out _, out _, out stateName))
        {
            string weapon = GetEquipAnimName(objectId);
            if (string.IsNullOrEmpty(weapon))
            {
                Debug.LogError(
                    "[CastSync] SpAtk clip duration missing weapon suffix state=" + token +
                    " objectId=" + objectId +
                    " race=" + race +
                    " — equipped weapon anim is empty, no default suffix");
                return null;
            }

            stateName = token.EndsWith("_", StringComparison.Ordinal) ? token + weapon : token + "_" + weapon;
        }

        AnimationClip clip = SkillAnimationDatabase.GetSpAtkCombatClip(stateName, race);
        if (clip == null)
        {
            Entity entity = GetEntity(objectId);
            int skillId = entity != null && entity.GetMagicCastData() != null
                ? entity.GetMagicCastData().SkillId
                : 0;
            SkillAnimationResolver.LogMissingCombatClip(skillId, race, stateName);
        }

        return clip;
    }

    static bool IsShotEventName(string eventName)
    {
        return string.Equals(eventName, "OnAnimationShoot", StringComparison.Ordinal)
            || string.Equals(eventName, "AttackShot", StringComparison.Ordinal)
            || string.Equals(eventName, "OnAnimationAttackShot", StringComparison.Ordinal);
    }
}
