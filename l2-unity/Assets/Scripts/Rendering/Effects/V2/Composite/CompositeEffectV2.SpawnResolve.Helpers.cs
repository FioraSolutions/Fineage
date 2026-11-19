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

        // A composite made only of caster-side parts (Dash: mo_dash_ca + mo_dash_ta at CasterRoot)
        // has nothing to aim at. Inventing a hit point from the current target used to give
        // ResolveV2SpawnRotation a sideways vector and yawed the whole burst away from the actor.
        if (!NeedsTargetHitPoint())
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

        if (IsCasterSelfTarget(target, targetEntity))
        {
            // Self skills (skillgrp cast_range = -1, e.g. Dash 4) are pinned to the caster by
            // SkillVisualEffectDispatch. The anchor is then the mage itself, and a "direction to
            // target" computed from the weapon bone points sideways. ResolveV2SpawnRotation would
            // yaw the whole burst onto that vector, so the effect fired away from the actor instead
            // of moving with it. Keep the anchor for target-side parts, drop the fake direction.
            Transform selfAnchor = HitAnchorResolver.ResolveHitAnchor(targetEntity, target);
            if (selfAnchor != null)
            {
                _context.HasHitPoint = true;
                _context.HitPoint = selfAnchor.position;
            }

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

    /// <summary>
    /// True when the composite has at least one part that actually lands on the target side
    /// (impact / hit-point / projectile). Caster-side-only skills never need a hit anchor.
    /// </summary>
    bool NeedsTargetHitPoint()
    {
        if (_v2Parts == null)
        {
            return false;
        }

        for (int i = 0; i < _v2Parts.Length; i++)
        {
            CompositePart part = _v2Parts[i];
            if (part == null || !part.IsSpawnable)
            {
                continue;
            }

            if (part.WantsHitCollider || part.IsLaunchedProjectile ||
                IsTargetSideAttachment(part.spawnAttachmentPoint))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// True when the resolved target is the caster itself (self buff / Dash). Such casts must not
    /// fabricate a hit direction: the effect has to sit on and face with the actor.
    /// </summary>
    bool IsCasterSelfTarget(Transform target, Entity targetEntity)
    {
        if (_context == null)
        {
            return false;
        }

        if (_context.CasterEntity != null && targetEntity != null)
        {
            return ReferenceEquals(_context.CasterEntity, targetEntity);
        }

        if (_context.CasterUserId > 0 && _castData != null && _castData.TargetObjectId > 0)
        {
            return _context.CasterUserId == _castData.TargetObjectId;
        }

        if (target == null)
        {
            return false;
        }

        Transform casterRoot = _context.CasterEntity != null
            ? _context.CasterEntity.transform
            : _context.CasterTransform;
        return casterRoot != null && (target == casterRoot || target.IsChildOf(casterRoot));
    }
}
