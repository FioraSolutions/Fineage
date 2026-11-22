using UnityEngine;
using System;

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

        // The template controller carries one behaviour per SpAtk family state (01, 02, 06).
        // Match on the numeric family of the sequence we were asked about, not just "02 or not",
        // or the buff gesture is handed the sword-swing behaviour and its (missing) hit window.
        string wantFamily = SpAtkFamily(spatkAnimName);
        for (int i = 0; i < behaviours.Length; i++)
        {
            PlayerStateSpAtk candidate = behaviours[i];
            if (candidate == null || string.IsNullOrEmpty(candidate.motionName))
            {
                continue;
            }

            if (string.Equals(SpAtkFamily(candidate.motionName), wantFamily, System.StringComparison.Ordinal))
            {
                behaviour = candidate;
                return true;
            }
        }

        behaviour = behaviours[0];
        return behaviour != null;
    }

    /// <summary>
    /// Normalised SpAtk family ("01", "02", "06") from an animator state (<c>SpAtk06_1HS</c>) or a
    /// source clip (<c>*_ao_spatk001_1hs_*</c>, <c>*_ao_SpAtk06_Hand_*</c>). The committed clips pad
    /// the number to three digits, the states do not, so parse the digit run and reformat.
    /// </summary>
    private static string SpAtkFamily(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        int at = name.IndexOf("SpAtk", System.StringComparison.OrdinalIgnoreCase);
        if (at < 0)
        {
            return null;
        }

        int start = at + 5;
        int end = start;
        while (end < name.Length && end - start < 3 && char.IsDigit(name[end]))
        {
            end++;
        }

        if (end == start)
        {
            return null;
        }

        int value;
        if (!int.TryParse(name.Substring(start, end - start), out value))
        {
            return null;
        }

        return value.ToString("00");
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

        // Self-buff gesture (SpAtk06_hand: Ultimate Evasion, Dash, Focus Chance, Escape Shackle).
        // The retail clip carries no hit notify, so there is no hit window to sync to. It must
        // still finish with the server cast, otherwise a 1 s Dash keeps gesturing for 1.5 s after
        // the buff is already applied. Fit the gesture to the same wall time the other SpAtk
        // families use: SwitchToIdle at the packet HitTime.
        if (IsAuthoredTimingGesture())
        {
            int gestureCycleMs = serverTimeMs > 0 ? serverTimeMs : 0;
            float gestureSeconds = (gestureCycleMs / 1000f) - HitTimeCompensationSeconds;
            if (gestureSeconds >= MinServerHitSeconds)
            {
                _serverHitTime = gestureSeconds;
                // Whole clip (to the SwitchToIdle mark) has to land on the server cast end.
                _linearPatkSpd = (SwitchToIdleNormalizedTime * clipLength) / gestureSeconds;
            }
            else
            {
                _linearPatkSpd = AnimPlayback.AuthoredRate;
                _serverHitTime = clipLength / Mathf.Max(0.01f, _linearPatkSpd);
            }
            Animations.SetPAtkSpeed(objectId, _linearPatkSpd);
        }
        else if (serverTimeMs > 0 && _eventHitTimeInClip > 0.01f)
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

    /// <summary>
    /// SpAtk06 is the hands-up buff gesture shared by every race (retail
    /// <c>AnimationCombo_Interlude</c> tag <c>X</c>), with no hit window to sync playback to.
    /// The behaviour's <see cref="motionName"/> is the source clip (<c>*_SpAtk06_Hand_&lt;Race&gt;</c>),
    /// which is what the template controller's <c>SpAtk06_hand</c> state holds.
    /// </summary>
    private bool IsAuthoredTimingGesture()
    {
        return IsSpAtk06Gesture(motionName);
    }

    internal static bool IsSpAtk06Gesture(string name)
    {
        return !string.IsNullOrEmpty(name) &&
               name.IndexOf("SpAtk06", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
