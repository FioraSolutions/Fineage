using UnityEngine;

public abstract partial class CompositePart : ISerializationCallbackReceiver
{
    public virtual void OnAnimationShoot(BaseEffect instance, EffectResolveContext context)
    {
    }

    public virtual bool TryResolveSpawn(
        IEffectAttachmentResolver resolver,
        EffectResolveContext context,
        out Transform followTransform,
        out Vector3 worldPosition)
    {
        if (requireNamedBone && !string.IsNullOrEmpty(boneName))
        {
            if (TryFindNamedBone(context, boneOnTarget, boneName, out followTransform, out worldPosition))
                return true;

            Debug.LogError(
                "[SkillVisual] missing bone=" + boneName +
                " effect=" + name +
                " onTarget=" + boneOnTarget);
            followTransform = null;
            worldPosition = Vector3.zero;
            return false;
        }

        if (!string.IsNullOrEmpty(boneName))
        {
            var bonePlacement = new BonePlacement { boneName = boneName };
            if (bonePlacement.TryResolve(resolver, context, out followTransform, out worldPosition))
            {
                return true;
            }
        }

        followTransform = null;
        worldPosition = Vector3.zero;
        if (resolver != null &&
            resolver.Resolve(spawnAttachmentPoint, context, out followTransform, out worldPosition))
        {
            return true;
        }

        return placement != null &&
               placement.TryResolve(resolver, context, out followTransform, out worldPosition);
    }

    static bool TryFindNamedBone(
        EffectResolveContext context,
        bool onTarget,
        string bone,
        out Transform followTransform,
        out Vector3 worldPosition)
    {
        followTransform = null;
        worldPosition = Vector3.zero;
        if (context == null || string.IsNullOrEmpty(bone))
            return false;

        Entity who = onTarget ? context.TargetEntity : context.CasterEntity;
        if (who == null || who.Gear == null)
            return false;

        Transform found = who.Gear.FindRecursiveBone(bone);
        if (found == null)
            return false;

        followTransform = found;
        worldPosition = found.position;
        return true;
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        if (placement is BonePlacement bone && !string.IsNullOrEmpty(bone.boneName))
        {
            boneName = bone.boneName;
        }

        if (spawnAttachmentPoint == EffectAttachmentPoint.CasterRoot && placement != null)
        {
            spawnAttachmentPoint = EffectPlacement.ToAttachment(placement);
        }
    }
}
