using UnityEngine;

public partial class MovementData
{
    public float GetSpeed()
    {
        if (_entity.Stats == null)
            return 0f;
        bool npc = !IsPlayerPawn(_entity);
        return L2PlayerMoveGait.Speed(
            _moveTimer,
            _entity.Running,
            npc,
            _entity.Stats.UnitySpeedWalking,
            _entity.Stats.UnitySpeedRun);
    }

    public bool IsWalkGait()
    {
        return L2PlayerMoveGait.IsWalkStart(_moveTimer, _entity.Running, !IsPlayerPawn(_entity));
    }

    public float GetMoveTimer()
    {
        return _moveTimer;
    }

    bool IsAnimatorOffGait(string wantedGait)
    {
        UserEntity ue = _entity as UserEntity;
        if (ue == null)
            return false;
        NetworkAnimationController nac = ue.GetAnimatorController();
        Animator anm = nac != null ? nac.GetAnimator() : null;
        if (anm == null)
            return false;

        string state = wantedGait + "_" + ue.WeaponAnim;
        if (PlayerLocomotionCrossFade.IsAlreadyPlaying(anm, state))
            return false;

        AnimatorClipInfo[] clips = anm.GetCurrentAnimatorClipInfo(0);
        string clipName = clips != null && clips.Length > 0 && clips[0].clip != null
            ? clips[0].clip.name
            : null;

        // Stuck on a non-locomotion clip with no fade into the wanted gait.
        if (!string.IsNullOrEmpty(clipName))
        {
            bool isWantedLocomotion =
                clipName.IndexOf(".ao_" + wantedGait + "_", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("_" + wantedGait + "_", System.StringComparison.OrdinalIgnoreCase) >= 0;
            if (isWantedLocomotion)
                return false;
            if (clipName.IndexOf(".ao_wait_", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf(".ao_atkwait_", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf(".ao_Wait_", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("jatk", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("PicItem", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("pickup", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("MagicShot", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("CastLong", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("CastMid", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                clipName.IndexOf("CastEnd", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
        }

        return true;
    }

    void LookAlong(Transform t, Vector3 lookDir)
    {
        if (t == null || _pawnYaw == null)
            return;
        int id = _entity.Identity != null ? _entity.Identity.Id : 0;
        if (id != 0 && CombatPacketServices.Facing != null && CombatPacketServices.Facing.HasFollow(id))
        {
            // CombatFacingService.ApplyYaw owns this rotation. A stale drive would
            // overwrite it from StepCharInfo when this Update runs later.
            if (_yawDrive != null)
                _yawDrive.Stop();
            return;
        }
        if (lookDir.sqrMagnitude <= 0.0001f)
            return;
        if (_yawDrive == null)
            _yawDrive = _pawnYaw.EnsureDrive(t);
        _pawnYaw.RequestLook(_yawDrive, t, t.position + lookDir, L2PawnYaw.RotationRateYaw);
    }
}
