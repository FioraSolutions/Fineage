using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
