using UnityEngine;

public partial class PlayerStateSpAtk : StateMachineBehaviour
{
    private static bool TryResolveBehaviour(Animator animator, string spatkAnimName, out PlayerStateSpAtk behaviour)
    {
        behaviour = null;
        PlayerStateSpAtk[] behaviours = animator.GetBehaviours<PlayerStateSpAtk>();
        if (behaviours == null || behaviours.Length == 0)
        {
            return false;
        }

        if (behaviours.Length == 1)
        {
            behaviour = behaviours[0];
            return true;
        }

        bool want02 = !string.IsNullOrEmpty(spatkAnimName) &&
                      spatkAnimName.IndexOf("SpAtk02", System.StringComparison.OrdinalIgnoreCase) >= 0;
        for (int i = 0; i < behaviours.Length; i++)
        {
            PlayerStateSpAtk candidate = behaviours[i];
            if (candidate == null || string.IsNullOrEmpty(candidate.motionName))
            {
                continue;
            }

            string motion = candidate.motionName;
            bool is02 = motion.IndexOf("spatk002", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        motion.IndexOf("spatk02", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        motion.IndexOf("SpAtk02", System.StringComparison.OrdinalIgnoreCase) >= 0;
            if (is02 == want02)
            {
                behaviour = candidate;
                return true;
            }
        }

        behaviour = behaviours[0];
        return behaviour != null;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _isSwitchIdle = false;
        _hitExecuted = false;
        _startTime = Time.time;

        float startHit = AnimationDataCache.GetEventTimeByName(animator, motionName, eventStartHitName);
        float endHit = AnimationDataCache.GetEventTimeByName(animator, motionName, eventEndHitName);
        _eventHitTimeInClip = (startHit + endHit) / 2f;

        int objectId = AnimatorUtils.GetObjectId(animator);
        int serverTimeMs = animator.GetInteger("sptimeatk");
        float clipLength = AnimationDataCache.GetOverrideLength(animator, motionName);
        if (clipLength <= 0.01f)
            clipLength = Mathf.Max(stateInfo.length, 0.01f);

        if (serverTimeMs > 0 && _eventHitTimeInClip > 0.01f)
        {
            _serverHitTime = (serverTimeMs / 1000f) - HitTimeCompensationSeconds;
            if (_serverHitTime < MinServerHitSeconds)
                _serverHitTime = MinServerHitSeconds;
            _linearPatkSpd = _eventHitTimeInClip / _serverHitTime;
            Animations.SetPAtkSpeed(objectId, _linearPatkSpd);
        }
        else
        {
            World world = WorldApplyPacketServices.GameWorld;
            Entity entity = world != null ? world.GetEntityNoLockSync(objectId) : null;
            _linearPatkSpd = Animations.ApplyLinearMeleePAtkSpeed(
                objectId, motionName, clipLength);
            float cycleMs = AttackTimingHelper.ResolveAttackCycleMs(entity, motionName);
            _serverHitTime = Mathf.Max(MinServerHitSeconds, cycleMs / 2000f);
        }

        // The cast playable graph owns this Animator. Update from OnStateEnter
        // evaluates the graph and enters this state again.
        if (!_applyingEnterPose)
        {
            _applyingEnterPose = true;
            animator.Update(0f);
            _applyingEnterPose = false;
        }
    }
}
