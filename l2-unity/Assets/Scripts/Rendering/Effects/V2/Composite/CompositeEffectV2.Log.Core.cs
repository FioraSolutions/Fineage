using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
    void LogHomeSpawnPlay()
    {
        Transform target = _context != null ? _context.TargetTransform : null;
        Transform caster = _context != null ? _context.CasterTransform : _owner;
        int partCount = _v2Parts != null ? _v2Parts.Length : 0;
        // Debug.Log(
            // $"[HOME_SPAWN] PlayV2 composite='{name}' v2Parts={partCount} " +
            // $"legacy={ShouldUseLegacyPartPipeline} followSelfParent='{(transform.parent != null ? transform.parent.name : "null")}' " +
            // $"compositePos={transform.position} caster='{(caster != null ? caster.name : "null")}' " +
            // $"casterPos={(caster != null ? caster.position.ToString("F3") : "-")} " +
            // $"target='{(target != null ? target.name : "null")}' " +
            // $"targetPos={(target != null ? target.position.ToString("F3") : "-")}");
        if (_v2Parts == null)
        {
            return;
        }

        for (int i = 0; i < _v2Parts.Length; i++)
        {
            CompositePart p = _v2Parts[i];
            if (p == null)
            {
                // Debug.Log($"[HOME_SPAWN]   part[{i}]=null");
                continue;
            }

            // Debug.Log(
                // $"[HOME_SPAWN]   part[{i}] name='{p.name}' type={p.GetType().Name} " +
                // $"placement={(p.placement != null ? p.placement.GetType().Name : "null")} " +
                // $"spawnPoint={p.spawnAttachmentPoint} " +
                // $"timing={p.spawnTiming} follow={p.follow} prefab='{(p.prefab != null ? p.prefab.name : "null")}'");
        }
    }
}
