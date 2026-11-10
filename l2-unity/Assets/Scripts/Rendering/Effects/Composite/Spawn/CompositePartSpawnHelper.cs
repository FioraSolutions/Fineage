using UnityEngine;

public static partial class CompositePartSpawnHelper
{
    public const float HitDirectionYawOffsetDegrees = -90f;

    public static bool TryResolveAttachment(
        CompositePrefabPart part,
        EffectResolveContext context,
        IEffectAttachmentResolver resolver,
        out Transform resolvedTransform,
        out Vector3 worldPosition)
    {
        resolvedTransform = null;
        worldPosition = Vector3.zero;
        if (part == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(part.attachmentBoneName) &&
            context != null &&
            context.CasterEntity != null &&
            context.CasterEntity.Gear != null)
        {
            Transform bone = context.CasterEntity.Gear.FindRecursiveBone(part.attachmentBoneName);
            if (bone != null)
            {
                resolvedTransform = bone;
                worldPosition = bone.position;
                return true;
            }
        }

        if (resolver != null &&
            resolver.Resolve(part.attachmentPoint, context, out resolvedTransform, out worldPosition))
        {
            return true;
        }

        // Debug.LogWarning($"CompositePrefabEffect: could not resolve point {part.attachmentPoint} for part {part.name}.");
        return false;
    }

    public static BaseEffect SpawnInstance(
        CompositePrefabPart part,
        Transform resolvedTransform,
        Vector3 worldPosition,
        Transform owner,
        EffectResolveContext context,
        bool useLegacyLifetimeHacks)
    {
        Vector3 adjustedOffset = GetAdjustedOffset(part, resolvedTransform, owner);
        Vector3 spawnPosition = CompositeEffectUtilities.ResolveSpawnPosition(
            resolvedTransform,
            worldPosition,
            adjustedOffset);
        Quaternion rotation = ResolveSpawnRotation(part, resolvedTransform, context);

        BaseEffect instance = Object.Instantiate(part.prefab, spawnPosition, rotation);
        instance.gameObject.SetActive(true);

        AttachIfFollow(part, resolvedTransform, instance.transform, adjustedOffset, worldPosition);
        ApplyScale(part, instance.transform);
        if (useLegacyLifetimeHacks)
        {
            ApplyShaderLifetimeOverride(part, instance.transform);
        }

        return instance;
    }

    public static Quaternion ResolveSpawnRotation(
        CompositePrefabPart part,
        Transform resolvedTransform,
        EffectResolveContext context)
    {
        if (context != null &&
            context.HasHitDirection &&
            context.HitDirection.sqrMagnitude > 0.0001f)
        {
            return Quaternion.LookRotation(context.HitDirection.normalized, Vector3.up) *
                   Quaternion.Euler(0f, HitDirectionYawOffsetDegrees, 0f);
        }

        return CompositeEffectUtilities.ResolveSpawnRotation(part != null && part.inheritRotation, resolvedTransform);
    }

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
