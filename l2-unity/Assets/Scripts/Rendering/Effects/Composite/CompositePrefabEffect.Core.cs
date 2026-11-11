using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CompositePrefabEffect : TimedCompositeEffectBase
{
    public override void CollectReferencedPrefabs(List<UnityEngine.Object> sink)
    {
        base.CollectReferencedPrefabs(sink);
        if (_parts == null || sink == null)
            return;
        for (int i = 0; i < _parts.Length; i++)
        {
            CompositePrefabPart part = _parts[i];
            if (part == null || part.prefab == null)
                continue;
            sink.Add(part.prefab);
            if (part.prefab.gameObject != null)
                sink.Add(part.prefab.gameObject);
        }
    }

    private void WireRenderingPacketServices()
    {
        RenderingPacketServices.Bind(animations: _animations);
    }
}
