using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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
}
