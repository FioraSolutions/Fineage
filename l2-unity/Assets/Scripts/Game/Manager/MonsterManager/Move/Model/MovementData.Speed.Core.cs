using UnityEngine;

public partial class MovementData
{
    public void AdvanceMoveTimer()
    {
        if (IsPlayerPawn(_entity))
            _moveTimer += Time.deltaTime;
    }

    public void SyncUserGait()
    {
        if (!IsPlayerPawn(_entity))
            return;
        bool walking = IsWalkGait();
        string anim = walking ? "walk" : "run";

        // Heal leftover CullCompletely from older spawns (freezes mid-run when
        // briefly off-camera). New spawns already use CullUpdateTransforms.
        UserEntity userForCull = _entity as UserEntity;
        if (userForCull != null)
        {
            NetworkAnimationController nacCull = userForCull.GetAnimatorController();
            Animator anmCull = nacCull != null ? nacCull.GetAnimator() : null;
            if (anmCull != null && anmCull.cullingMode == AnimatorCullingMode.CullCompletely)
                anmCull.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        }

        // [GAIT] diagnostics — per-entity throttle so concurrent CharInfo don't
        // starve each other of sync lines (global throttle hid MMagic freezes).
        int entityId = _entity.Identity != null ? _entity.Identity.Id : 0;
        float lastLogAt;
        _syncPrevLogAt.TryGetValue(entityId, out lastLogAt);
        if (_entity is UserEntity && GaitLog.Enabled &&
            Time.time - lastLogAt >= 0.25f)
        {
            _syncPrevLogAt[entityId] = Time.time;
            UserEntity ue = _entity as UserEntity;
            string clip = "?";
            string stateWant = anim + "_" + ue.WeaponAnim;
            float animSpeed = 0f;
            float runSpeed = 0f;
            float walkSpeed = 0f;
            float normTime = 0f;
            float normDelta = 0f;
            bool inTrans = false;
            bool already = false;
            string cull = "?";
            NetworkAnimationController nac = ue.GetAnimatorController();
            Animator anm = nac != null ? nac.GetAnimator() : null;
            if (anm != null)
            {
                AnimatorClipInfo[] clips = anm.GetCurrentAnimatorClipInfo(0);
                clip = clips != null && clips.Length > 0 && clips[0].clip != null
                    ? clips[0].clip.name : "<none>";
                animSpeed = anm.speed;
                runSpeed = AnimatorBoolSet.HasFloat(anm, "run_speed") ? anm.GetFloat("run_speed") : 0f;
                walkSpeed = AnimatorBoolSet.HasFloat(anm, "walk_speed") ? anm.GetFloat("walk_speed") : 0f;
                AnimatorStateInfo st = anm.GetCurrentAnimatorStateInfo(0);
                normTime = st.normalizedTime;
                inTrans = anm.IsInTransition(0);
                already = PlayerLocomotionCrossFade.IsAlreadyPlaying(anm, stateWant);
                cull = anm.cullingMode.ToString();
                if (_syncPrevNorm.TryGetValue(entityId, out float prev))
                    normDelta = normTime - prev;
                _syncPrevNorm[entityId] = normTime;
            }
            GaitLog.Log(
                _entity, "sync",
                "anim=" + anim +
                " cached=" + (_gaitAnim ?? "none") +
                " want=" + stateWant +
                " clip=" + clip +
                " already=" + already +
                " inTrans=" + inTrans +
                " cull=" + cull +
                " animSpeed=" + animSpeed.ToString("F2") +
                " runSpeed=" + runSpeed.ToString("F3") +
                " walkSpeed=" + walkSpeed.ToString("F3") +
                " normT=" + normTime.ToString("F3") +
                " normD=" + normDelta.ToString("F3") +
                " running=" + _entity.Running +
                " moveTimer=" + _moveTimer.ToString("F3") +
                " inCombat=" + _entity.InCombat +
                " attackVis=" + ue.IsAttackVisualPlaying());
            MagicGaitTrace.SyncTick(_entity,
                "anim=" + anim +
                " cached=" + (_gaitAnim ?? "none") +
                " moveTimer=" + _moveTimer.ToString("F3") +
                " offGait=" + IsAnimatorOffGait(anim));
        }

        if (_entity is UserEntity && _gaitAnim != anim && GaitLog.Enabled)
        {
            GaitLog.Log(
                _entity, "transition",
                "from=" + (_gaitAnim ?? "none") +
                " to=" + anim +
                " running=" + _entity.Running +
                " identityRunning=" + (_entity.Identity != null ? _entity.Identity.IsRunning : false) +
                " moveTimer=" + _moveTimer.ToString("F3"));
            MagicGaitTrace.Event(_entity, "gait-transition",
                "from=" + (_gaitAnim ?? "none") + " to=" + anim);
        }

        if (_gaitAnim == anim)
        {
            // Reconcile: something (PlayCombatWait/PlayStandWait/Idle.Enter/
            // Pickup/presentation) may have swapped the animator to a non-locomotion
            // state (atkwait/wait/jatk/pickup) while _gaitAnim still caches run/walk.
            // In that case the pawn keeps translating (MoveToPoint still steps it)
            // but the animator is frozen in a stance. Detect the actual state and replay.
            if (IsAnimatorOffGait(anim))
            {
                // Cooldown so a wait→run CrossFade cannot be ForceReplay'd every
                // frame (that restarts the fade and freezes MMagic mid-stride).
                if (Time.time - _lastForceGaitAt >= 0.35f)
                {
                    _lastForceGaitAt = Time.time;
                    _gaitAnim = anim;
                    MagicGaitTrace.Event(_entity, "FORCE-GAIT", "to=" + anim + " reason=OffGait");
                    EntityActionVisual.StartMove(_entity, walking, force: true);
                    if (GaitLog.Enabled)
                        GaitLog.Log(_entity, "force-gait", "to=" + anim);
                }
                else
                {
                    MagicGaitTrace.Decision(_entity, "force-gait-COOLDOWN",
                        "to=" + anim + " left=" + (0.35f - (Time.time - _lastForceGaitAt)).ToString("F2"));
                }
            }
            return;
        }
        _gaitAnim = anim;
        MagicGaitTrace.Decision(_entity, "StartMove", "walking=" + walking + " force=false");
        EntityActionVisual.StartMove(_entity, walking);
    }

    public void ReplayUserGait()
    {
        MagicGaitTrace.Decision(_entity, "ReplayUserGait", "cached→null then Sync");
        _gaitAnim = null;
        // Safe, non-forcing replay used on frequent retargets (move-point packets).
        // SyncUserGait will re-play only if the animator is not already in the gait
        // state, so this does not restart the run clip every packet.
        SyncUserGait();
    }

    public void ForceReplayUserGait()
    {
        bool walking = IsWalkGait();
        _gaitAnim = walking ? "walk" : "run";
        MagicGaitTrace.Event(_entity, "ForceReplayUserGait", "walking=" + walking);
        EntityActionVisual.StartMove(_entity, walking, force: true);
    }
}
