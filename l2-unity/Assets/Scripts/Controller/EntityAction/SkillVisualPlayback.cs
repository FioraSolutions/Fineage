using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns the prefabs <see cref="SkillVisualCatalog"/> resolved for one cast,
/// with that row's phase, attach, bone, offset, delay, and flying time.
/// </summary>
public static class SkillVisualPlayback
{
    public static void Play(Entity entity, SkillVisualResolution resolution, MagicCastData cast, int targetId)
    {
        if (entity == null || resolution == null)
            return;

        if (resolution.Status == SkillVisualStatus.None)
            return;

        for (int i = 0; i < resolution.MissingAssets.Count; i++)
        {
            Debug.LogError(
                "[SkillVisual] skill=" + resolution.SkillId +
                " level=" + resolution.Level +
                " missing asset=" + resolution.MissingAssets[i]);
        }

        EffectManager effects = ControllerPacketServices.Effects;
        if (effects == null)
        {
            Debug.LogError("[SkillVisual] skill=" + resolution.SkillId + " EffectManager is missing");
            return;
        }

        var parts = new List<CompositePart>(resolution.Actions.Count);
        Entity target = ResolveTarget(targetId);
        for (int i = 0; i < resolution.Actions.Count; i++)
        {
            SkillVisualAction action = resolution.Actions[i];
            if (!action.AssetPresent)
                continue;

            BaseEffect prefab;
            string resourcePath;
            if (!SkillVisualRuntime.TryLoadPrefab(action.AssetKey, out prefab, out resourcePath))
            {
                Debug.LogError(
                    "[SkillVisual] skill=" + resolution.SkillId +
                    " level=" + resolution.Level +
                    " missing asset=" + action.AssetKey +
                    " path=" + (resourcePath ?? string.Empty));
                continue;
            }

            parts.Add(BuildPart(action, prefab, resolution.FlyingTime, entity, target));
        }

        if (parts.Count == 0)
            return;

        var go = new GameObject("skill-visual-" + resolution.SkillId);
        go.transform.SetParent(entity.transform, false);
        CompositeEffectV2 composite = go.AddComponent<CompositeEffectV2>();
        composite.AssignResolvedParts(parts.ToArray());

        EffectParticle settings = ScriptableObject.CreateInstance<EffectParticle>();
        settings.defaultLifeTime = 3f;
        settings.hideTime = 0.5f;
        effects.PlayResolvedVisual(composite, settings, cast, entity.transform);
    }

    static CompositePart BuildPart(
        SkillVisualAction action,
        BaseEffect prefab,
        float flyingTime,
        Entity caster,
        Entity target)
    {
        CompositePart part;
        if (action.Kind == SkillVisualPartKind.Projectile)
        {
            float speed = ProjectileSpeed(flyingTime, caster, target);
            part = new ShotProjectilePart
            {
                launchMode = ProjectileLaunchMode.OnAnimationShoot,
                showBeforeAnimationShoot = false,
                impactType = ProjectileImpactType.EffectOnly,
                speed = speed
            };
        }
        else if (action.Kind == SkillVisualPartKind.Impact)
        {
            part = new ImpactPart();
        }
        else
        {
            part = new StationaryPart();
        }

        part.name = action.AssetKey;
        part.prefab = prefab;
        part.scale = 1f;
        part.spawnTiming = action.Timing;
        part.spawnDelaySeconds = action.Delay > 0f ? action.Delay : 0f;
        part.spawnAttachmentPoint = action.Attachment;
        part.follow = action.Follow;
        part.boneName = action.NamedBone ? action.Bone : null;
        part.requireNamedBone = action.NamedBone;
        part.boneOnTarget = action.OnTarget && action.NamedBone;
        part.spawnOffset = VectorUtils.ConvertPosToUnity(new Vector3(action.OffsetX, action.OffsetY, action.OffsetZ));
        part.spawnOffsetLocal = action.RelativeToCylinder;
        return part;
    }

    static float ProjectileSpeed(float flyingTime, Entity caster, Entity target)
    {
        if (flyingTime <= 0f || caster == null || target == null)
            return 0.01f;

        Vector3 start = caster.GetPositionRightHand();
        float distance = Vector3.Distance(start, target.transform.position);
        if (distance <= 0.01f)
            return 0.01f;
        return distance / flyingTime;
    }

    static Entity ResolveTarget(int targetId)
    {
        if (targetId <= 0 || ControllerPacketServices.GameWorld == null)
            return null;
        return ControllerPacketServices.GameWorld.GetEntityNoLockSync(targetId);
    }
}
