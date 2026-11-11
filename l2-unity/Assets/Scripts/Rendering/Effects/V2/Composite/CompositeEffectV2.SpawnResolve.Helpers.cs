using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CompositeEffectV2
{
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
