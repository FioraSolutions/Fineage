using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2
{
    void RefreshV2Context()
    {
        _context = CompositeEffectUtilities.RebuildPreservingHit(_context, _owner, _castData);
    }

    static bool IsTargetSideAttachment(EffectAttachmentPoint point)
    {
        return point == EffectAttachmentPoint.WorldHitPoint ||
               point == EffectAttachmentPoint.TargetRoot ||
               point == EffectAttachmentPoint.TargetLowerBody ||
               point == EffectAttachmentPoint.TargetCenter ||
               point == EffectAttachmentPoint.TargetOverHead ||
               point == EffectAttachmentPoint.TargetPosition;
    }

    void EnsureHitPointFromTarget()
    {
        if (_context == null || _context.HasHitPoint)
        {
            return;
        }

        // Never invent a hit on the caster — that is what put prominence_ta on the mage.
        Transform target = _context.TargetTransform;
        Entity targetEntity = _context.TargetEntity;
        if (target == null && targetEntity == null)
        {
            return;
        }

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
