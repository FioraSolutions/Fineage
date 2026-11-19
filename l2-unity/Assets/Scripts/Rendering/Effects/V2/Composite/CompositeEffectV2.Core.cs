using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
    public override void CollectReferencedPrefabs(List<UnityEngine.Object> sink)
    {
        base.CollectReferencedPrefabs(sink);
        if (_v2Parts == null || sink == null)
            return;
        for (int i = 0; i < _v2Parts.Length; i++)
        {
            CompositePart part = _v2Parts[i];
            if (part == null || part.prefab == null)
                continue;
            sink.Add(part.prefab);
            if (part.prefab.gameObject != null)
                sink.Add(part.prefab.gameObject);
        }
    }

    bool TraceCrasherWind()
    {
        return _owner != null &&
               _owner.name.IndexOf("Crasher", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    // Trick / Switch / Confusion share m_u018_a_v2: one line per part so a Player.log shows where the
    // target-side art (m_u018_b, at_fatal_strike_ta) really lands — on the mob or on the caster.
    bool TraceTargetArt()
    {
        return name != null && name.IndexOf("m_u018_a_v2", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    void LogTargetArt(string stage, CompositePart part, Transform followTransform, Vector3 worldPosition, BaseEffect instance)
    {
        Transform caster = _context != null ? _context.CasterTransform : null;
        Transform target = _context != null ? _context.TargetTransform : null;
        Vector3 goPos = instance != null ? instance.transform.position : Vector3.zero;
        Debug.Log(
            "[TrickFx] " + stage +
            " part=" + (part != null ? part.name : "null") +
            " attach=" + (part != null ? part.spawnAttachmentPoint.ToString() : "-") +
            " timing=" + (part != null ? part.spawnTiming.ToString() : "-") +
            " castTarget=" + (_castData != null ? _castData.TargetObjectId : 0) +
            " caster=" + (caster != null ? caster.name : "null") +
            " target=" + (target != null ? target.name : "null") +
            " follow=" + (followTransform != null ? followTransform.name : "null") +
            " parent=" + (instance != null && instance.transform.parent != null ? instance.transform.parent.name : "null") +
            " pos=" + worldPosition.ToString("F2") +
            " go=" + goPos.ToString("F2") +
            " dCaster=" + (caster != null ? Vector3.Distance(goPos, caster.position).ToString("F2") : "-") +
            " dTarget=" + (target != null ? Vector3.Distance(goPos, target.position).ToString("F2") : "-"));
    }
}
