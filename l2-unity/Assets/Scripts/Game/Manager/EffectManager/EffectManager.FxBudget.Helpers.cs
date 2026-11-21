using UnityEngine;

public partial class EffectManager
{
    static int ResolveCasterEntityId(Transform target)
    {
        if (target == null)
            return 0;

        Entity entity = target.GetComponentInParent<Entity>();
        return entity != null && entity.Identity != null ? entity.Identity.Id : 0;
    }

    bool TryGateSkillFx(Transform target, MagicCastData castData)
    {
        int casterId = ResolveCasterEntityId(target);
        int targetObjectId = castData != null ? castData.TargetObjectId : 0;
        bool highPriority = FxBudgetPriority.IsHighPriorityCaster(casterId, targetObjectId);
        return FxBudgetPolicy.ShouldSpawnSkillFx(casterId, highPriority);
    }

    static void TrackFxBudget(GameObject fxRoot, Transform casterRoot, MagicCastData castData)
    {
        int casterId = ResolveCasterEntityId(casterRoot);
        int targetObjectId = castData != null ? castData.TargetObjectId : 0;
        bool highPriority = FxBudgetPriority.IsHighPriorityCaster(casterId, targetObjectId);
        FxBudgetEffectRelease.AttachIfNeeded(fxRoot, casterId, highPriority);
    }
}