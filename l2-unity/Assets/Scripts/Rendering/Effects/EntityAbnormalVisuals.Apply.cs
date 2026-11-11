using System.Collections.Generic;
using UnityEngine;

public sealed partial class EntityAbnormalVisuals : MonoBehaviour
{
    void Awake()
    {
        _entity = GetComponent<Entity>() ?? GetComponentInParent<Entity>();
    }

    void LateUpdate()
    {
        if (_entity == null || _entity.IsDead())
        {
            ClearAll();
            return;
        }

        AbnormalAveFlags flags = AbnormalVisualCatalog.ResolveFlags(_entity.TargetEffects);
        AbnormalPoseKind pose = AbnormalVisualCatalog.ResolvePose(flags);

        if (pose != _pose)
            ApplyPose(pose);
        if (flags != _flags)
            SyncFx(flags);

        _pose = pose;
        _flags = flags;
    }

    void ApplyPose(AbnormalPoseKind pose)
    {
        ClearPetrify();

        switch (pose)
        {
            case AbnormalPoseKind.Idle:
                // PlayStandWait → atkwait in combat so NPC weapons stay in the combat idle pose.
                var actions = EntityActionPacketServices.Actions;
                if (actions != null)
                    actions.Set(_entity, EntityActionKind.Idle, null);
                break;
            case AbnormalPoseKind.Petrify:
                ApplyPetrify();
                break;
            case AbnormalPoseKind.None:
            default:
                break;
        }
    }

    void ApplyPetrify()
    {
        if (_petrified)
            return;

        EntityActionVisual.CancelMove(_entity);

        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
        {
            _petrifyAnimator = animator;
            _speedBeforePetrify = animator.speed > 0.0001f ? animator.speed : 1f;
            // FLESH_STONE: Interlude freezes the skeleton on the current frame (Anchor / Medusa).
            animator.speed = 0f;
        }

        TintStone(true);
        _petrified = true;
    }

    void ClearPetrify()
    {
        if (!_petrified)
            return;
        if (_petrifyAnimator != null)
            _petrifyAnimator.speed = _speedBeforePetrify > 0.0001f ? _speedBeforePetrify : 1f;
        _petrifyAnimator = null;
        _speedBeforePetrify = 1f;
        TintStone(false);
        _petrified = false;
    }
}
