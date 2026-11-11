using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CompositeEffectV2
{
    Quaternion ResolveV2SpawnRotation(CompositePart part, Transform resolvedTransform)
    {
        if (part != null && part.UsesWeaponBoneRotation && resolvedTransform != null)
        {
            return resolvedTransform.rotation;
        }

        // L2 actor +X is flight/cast; Unity LookRotation is Z-forward. Same -90
        // yaw as V1 CompositePartSpawnHelper — required for MeshRing/cs burst too,
        // not only ShotProjectilePart (otherwise the halo faces into/side of caster).
        if (_context != null &&
            _context.HasHitDirection &&
            _context.HitDirection.sqrMagnitude > 0.0001f)
        {
            return Quaternion.LookRotation(_context.HitDirection.normalized, Vector3.up) *
                   Quaternion.Euler(0f, HitDirectionYawOffsetDegrees, 0f);
        }

        if (resolvedTransform != null && (part == null || part.follow || part.inheritRotation))
        {
            Vector3 forward = resolvedTransform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude > 0.0001f)
            {
                return Quaternion.LookRotation(forward.normalized, Vector3.up) *
                       Quaternion.Euler(0f, HitDirectionYawOffsetDegrees, 0f);
            }
        }

        return CompositeEffectUtilities.ResolveSpawnRotation(part != null && part.inheritRotation, resolvedTransform);
    }

    void RefreshV2Context()
    {
        _context = CompositeEffectUtilities.RebuildPreservingHit(_context, _owner, _castData);
    }

    void EnsureHitPointFromTarget()
    {
        if (_context == null || _context.HasHitPoint)
        {
            return;
        }

        Transform target = _context.TargetTransform != null
            ? _context.TargetTransform
            : _context.CasterTransform;
        Entity targetEntity = _context.TargetEntity != null
            ? _context.TargetEntity
            : _context.CasterEntity;
        if (targetEntity == null && target != null)
        {
            targetEntity = target.GetComponentInParent<Entity>();
        }

        Transform anchor = HitAnchorResolver.ResolveHitAnchor(targetEntity, target);
        if (anchor == null)
        {
            return;
        }

        _context.HasHitPoint = true;
        _context.HitPoint = anchor.position;
        if (!_context.HasHitDirection)
        {
            Vector3 fromCaster = _owner != null
                ? (anchor.position - _owner.position)
                : Vector3.zero;
            fromCaster.y = 0f;
            if (fromCaster.sqrMagnitude > 0.0001f)
            {
                _context.HasHitDirection = true;
                _context.HitDirection = fromCaster.normalized;
            }
        }
    }
}
