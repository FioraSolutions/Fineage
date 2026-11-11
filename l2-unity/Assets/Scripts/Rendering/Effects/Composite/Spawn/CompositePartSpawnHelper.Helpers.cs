using UnityEngine;

public static partial class CompositePartSpawnHelper
{
    public static Transform ResolveSetupOwner(Transform resolvedTransform, Transform owner, Transform instanceTransform)
    {
        return resolvedTransform != null ? resolvedTransform : (owner != null ? owner : instanceTransform);
    }

    public static Vector3 GetAdjustedOffset(CompositePrefabPart part, Transform resolvedTransform, Transform owner)
    {
        if (part == null || !part.normalizeOffsetByOwnerHeight)
        {
            return part != null ? part.positionOffset : Vector3.zero;
        }

        float height = ResolveAttachmentHeight(resolvedTransform, owner);
        float reference = Mathf.Max(0.01f, part.referenceHeight);
        float multiplier = Mathf.Max(0.01f, height / reference);
        return part.positionOffset * multiplier;
    }

    public static void ApplyShaderTargetPosition(
        CompositePrefabPart part,
        Transform instanceTransform,
        EffectResolveContext context,
        IEffectAttachmentResolver resolver)
    {
        if (part == null || instanceTransform == null || !part.passShaderTargetPosition || resolver == null)
        {
            return;
        }

        if (!resolver.Resolve(part.shaderTargetAttachmentPoint, context, out Transform targetTransform, out Vector3 targetWorldPosition))
        {
            return;
        }

        Vector3 adjustedTargetWorldPosition = CompositeEffectUtilities.ResolveSpawnPosition(
            targetTransform,
            targetWorldPosition,
            part.shaderTargetPositionOffset);

        EffectPart[] effectParts = instanceTransform.GetComponentsInChildren<EffectPart>(true);
        for (int i = 0; i < effectParts.Length; i++)
        {
            if (effectParts[i] != null)
            {
                effectParts[i].SetShaderTargetWorldPosOverride(true, adjustedTargetWorldPosition, targetTransform);
            }
        }
    }

    public static void ApplyShaderLifetimeOverride(CompositePrefabPart part, Transform instanceTransform)
    {
        if (part == null || !part.disableShaderLifetime || instanceTransform == null)
        {
            return;
        }

        EffectShaderLifetimeHelper.Apply(instanceTransform, false, 0.5f, 0.5f);
    }
}
