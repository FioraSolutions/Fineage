using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns the prefabs <see cref="SkillVisualCatalog"/> resolved for one cast,
/// with that row's phase, attach, bone, offset, delay, scale, rotation, and flying time.
/// Placement comes only from the skill-effect-placement table.
/// </summary>
public static class SkillVisualPlayback
{
    // Relative-to-cylinder offsets with a component larger than this are stored
    // in Unreal units even when bRelativeToCylinder is set. The packed Engine.dll
    // Notify() formula was not dumped; those rows convert as UU and are logged.
    const float RelativeCylinderUnitMax = 5f;

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

            parts.Add(BuildPart(action, prefab, resolution, entity, target));
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
        SkillVisualResolution resolution,
        Entity caster,
        Entity target)
    {
        CompositePart part;
        if (action.Kind == SkillVisualPartKind.Projectile)
        {
            float speed = ProjectileSpeed(action, resolution.FlyingTime, caster, target);
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

        Entity attachEntity = action.OnTarget ? target : caster;
        part.name = action.AssetKey;
        part.prefab = prefab;
        part.scale = action.DrawScale > 0f ? action.DrawScale : 1f;
        part.spawnTiming = action.Timing;
        part.spawnDelaySeconds = action.Delay > 0f ? action.Delay : 0f;
        part.spawnAttachmentPoint = action.Attachment;
        part.follow = action.Follow;
        part.boneName = action.NamedBone ? action.Bone : null;
        part.requireNamedBone = action.NamedBone;
        part.boneOnTarget = action.OnTarget && action.NamedBone;
        part.spawnOffset = ResolveOffset(action, attachEntity, resolution.SkillId);
        part.spawnOffsetLocal = action.RelativeToCylinder;
        part.inheritRotation = action.UseCharacterRotation;
        part.spawnEuler = VectorUtils.ConvertRotToUnity(
            new Vector3(action.RotationPitch, action.RotationYaw, action.RotationRoll));
        return part;
    }

    static Vector3 ResolveOffset(SkillVisualAction action, Entity attachEntity, int skillId)
    {
        var ue = new Vector3(action.OffsetX, action.OffsetY, action.OffsetZ);
        if (ue == Vector3.zero)
            return Vector3.zero;

        if (!action.RelativeToCylinder)
            return VectorUtils.ConvertPosToUnity(ue);

        float maxAbs = Mathf.Max(Mathf.Abs(ue.x), Mathf.Abs(ue.y), Mathf.Abs(ue.z));
        if (maxAbs > RelativeCylinderUnitMax)
        {
            Debug.LogError(
                "[SkillVisual] skill=" + skillId +
                " phase=" + (action.Phase ?? string.Empty) +
                " field=offset relative-to-cylinder value (" +
                ue.x + "," + ue.y + "," + ue.z +
                ") is larger than " + RelativeCylinderUnitMax +
                "; converting as UU because the packed Notify() formula is not available");
            return VectorUtils.ConvertPosToUnity(ue);
        }

        CharacterController controller = FindController(attachEntity);
        if (controller == null)
        {
            Debug.LogError(
                "[SkillVisual] skill=" + skillId +
                " phase=" + (action.Phase ?? string.Empty) +
                " field=b_relative_to_cylinder CharacterController is missing");
            return new Vector3(ue.y, ue.z, ue.x);
        }

        // Notify() is packed in Engine.dll and was not dumped. Offsets with
        // max |component| <= 5 are treated as cylinder units from the attach
        // point (CasterCenter is the capsule center): Unity X = UE Y * radius,
        // Unity Y = UE Z * half-height, Unity Z = UE X * radius, so (0,0,-1)
        // lands at the feet. This is an inferred convention, not a dumped formula.
        float radius = controller.radius;
        float halfHeight = controller.height * 0.5f;
        return new Vector3(ue.y * radius, ue.z * halfHeight, ue.x * radius);
    }

    static CharacterController FindController(Entity entity)
    {
        if (entity == null)
            return null;
        CharacterController controller = entity.GetComponent<CharacterController>();
        if (controller != null)
            return controller;
        return entity.GetComponentInChildren<CharacterController>(true);
    }

    static float ProjectileSpeed(SkillVisualAction action, float flyingTime, Entity caster, Entity target)
    {
        if (flyingTime > 0f && caster != null && target != null)
        {
            Vector3 start = caster.GetPositionRightHand();
            float distance = Vector3.Distance(start, target.transform.position);
            if (distance > 0.01f)
                return distance / flyingTime;
        }

        if (action.Speed > 0f)
            return VectorUtils.ConvertPosToUnity(new Vector3(action.Speed, 0f, 0f)).magnitude;

        return 0.01f;
    }

    static Entity ResolveTarget(int targetId)
    {
        if (targetId <= 0 || ControllerPacketServices.GameWorld == null)
            return null;
        return ControllerPacketServices.GameWorld.GetEntityNoLockSync(targetId);
    }
}
