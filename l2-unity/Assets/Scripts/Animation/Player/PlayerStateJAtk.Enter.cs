using UnityEngine;

public partial class PlayerStateJAtk
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(layerIndex);
        if (clipInfos == null || clipInfos.Length == 0)
        {
            clipInfos = animator.GetCurrentAnimatorClipInfo(layerIndex);
        }

        _isSwitchIdle = false;
        _exitViaSwitchToIdle = false;
        _dieTargetLatched = false;
        _lastSwitchDenyReason = "";
        _isSwordHitLogged = false;
        _milestoneMask = 0;
        _lastAnimNormalized = 0f;

        int objectId = animator.GetInteger(AnimatorUtils.OBJECT_ID);
        _rateFromSwingClip = false;
        float timeAnimation = 1f;
        if (clipInfos != null && clipInfos.Length > 0 && clipInfos[0].clip != null &&
            PlayerBasicAttackAnim.IsSwingClipName(clipInfos[0].clip.name))
        {
            timeAnimation = clipInfos[0].clip.length;
            _rateFromSwingClip = true;
        }
        else if (PlayerBasicAttackAnim.TryGetSwingClipLength(
            animator, layerIndex, out float swingLen, out _))
        {
            timeAnimation = swingLen;
            _rateFromSwingClip = true;
        }

        _clipLengthSec = timeAnimation;
        _fullCycleMs = GetFullAttackCycleMs(objectId, parameterName);
        _startTime = Time.time;
        _endTime = TimeUtils.ConvertMsToSec(_fullCycleMs);

        // Linear rate: play whole clip over full server attack cycle (not timeAtk/2).
        // After a chase, current clip is still run_* — do not use that length for patkspd.
        if (_rateFromSwingClip)
        {
            _linearPatkSpd = Animations.ApplyLinearMeleePAtkSpeed(
                objectId, parameterName, timeAnimation);
        }

        if (!AnimatorUtils.IsLocalPlayerAnimator(animator))
            return;

        TrySubscribeSwordCollider();

        int entityId = ResolvePlayerEntityId();
        if (entityId > 0 && SwordCollisionPacketServices.Collision != null)
        {
            _attackEpoch = SwordCollisionPacketServices.Collision.GetAttackEpoch(entityId);
            SwordCollisionPacketServices.Collision.UpdateAttackProgress(entityId, 0f);
        }
        else
        {
            _attackEpoch = 0;
        }

        float hitFraction = DomainPacketServices.LocalPlayer != null
            ? AttackTimingHelper.ResolveHitFractionByWeapon(DomainPacketServices.LocalPlayer)
            : 0.5f;
        float hitAtMs = _fullCycleMs * hitFraction;

        float attackShotSec = -1f;
        float onAnimHitSec = -1f;
        string clipName = "null";
        if (clipInfos != null && clipInfos.Length > 0 && clipInfos[0].clip != null)
        {
            AnimationClip clip = clipInfos[0].clip;
            clipName = clip.name;
            attackShotSec = AnimationDataCache.GetEventTimeByName(animator, clip, "AttackShot");
            onAnimHitSec = AnimationDataCache.GetEventTimeByName(animator, clip, "OnAnimationHit");
        }

        float clipLenMs = timeAnimation * 1000f;
        float attackShotNorm = timeAnimation > 0f && attackShotSec >= 0f ? attackShotSec / timeAnimation : -1f;
        float attackShotWallMs = attackShotNorm >= 0f ? _fullCycleMs * attackShotNorm : -1f;
        float onAnimHitNorm = timeAnimation > 0f && onAnimHitSec >= 0f ? onAnimHitSec / timeAnimation : -1f;

        float pAtkSpd = DomainPacketServices.LocalPlayer != null && DomainPacketServices.LocalPlayer.Stats != null
            ? DomainPacketServices.LocalPlayer.Stats.BasePAtkSpeed
            : 0f;

        // Client wall duration of the swing = server timeAtk (clip is rate-scaled to fit).
        // If clipLenMs >> serverTimeAtkMs, anim feels long only if rate is wrong; compare these.
        // Debug.Log(
            // $"[ATK_TIMING_CMP] Enter anim={parameterName} clip={clipName} " +
            // $"pAtkSpd={pAtkSpd:F1} serverTimeAtkMs={_fullCycleMs:F1} (500000/pAtkSpd) " +
            // $"serverHitMs={hitAtMs:F1} (timeAtk*{hitFraction:F2}) " +
            // $"clipLenMs={clipLenMs:F1} clipLenSec={timeAnimation:F3} " +
            // $"patkspd={_linearPatkSpd:F3} (=clipMs/serverMs) " +
            // $"clientWallCycleMs={_fullCycleMs:F1} " +
            // $"deltaClipVsServerMs={clipLenMs - _fullCycleMs:F1} " +
            // $"AttackShotSec={attackShotSec:F3} AttackShotNorm={attackShotNorm:F3} AttackShotWallMs~={attackShotWallMs:F1} " +
            // $"OnAnimationHitSec={onAnimHitSec:F3} OnAnimationHitNorm={onAnimHitNorm:F3} " +
            // $"epoch={_attackEpoch}");

        // Debug.Log(
            // $"[ATK_HIT_CHAIN] 1b.JAtkEnter frame={Time.frameCount} t={Time.time:F3} anim={parameterName} " +
            // $"fullCycleMs={_fullCycleMs:F1} hitFraction={hitFraction:F3} hitAtMs~={hitAtMs:F1} " +
            // $"clipLenSec={timeAnimation:F3} AttackShotNorm={attackShotNorm:F3} " +
            // $"patkspd={_linearPatkSpd:F3} player={DomainPacketServices.LocalPlayer?.RandomName}");
        // Debug.Log(
            // $"{JATK_TIMING_LOG} Enter LINEAR anim={parameterName} serverTimeAtkMs={_fullCycleMs:F1} " +
            // $"clipLenMs={clipLenMs:F1} patkspd={_linearPatkSpd:F3} AttackShotWallMs~={attackShotWallMs:F1} " +
            // $"player={DomainPacketServices.LocalPlayer?.RandomName}");
        // UnityChainLog.OnJAtkEnter(DomainPacketServices.LocalPlayer, parameterName, _fullCycleMs, hitAtMs, pAtkSpd, _linearPatkSpd, timeAnimation);

        if (IsDual(parameterName))
        {
            // Debug.Log(
                // $"{JATK_EARLY_LOG} Enter DUAL anim={parameterName} clip={clipName} " +
                // $"clipLenSec={timeAnimation:F3} serverCycleSec={_endTime:F3} " +
                // $"patkspd={_linearPatkSpd:F3} " +
                // $"note=wall_clock_ends_at_serverCycle_even_if_dual_clip_has_2_hits");
        }

        StopAnimationTrigger(animator, parameterName);
    }
}
