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
}
