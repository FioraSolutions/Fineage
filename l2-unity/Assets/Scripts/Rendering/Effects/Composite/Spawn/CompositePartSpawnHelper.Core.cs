using UnityEngine;

public static partial class CompositePartSpawnHelper
{
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
}
