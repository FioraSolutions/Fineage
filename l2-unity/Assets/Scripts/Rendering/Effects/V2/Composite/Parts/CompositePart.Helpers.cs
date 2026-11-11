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
