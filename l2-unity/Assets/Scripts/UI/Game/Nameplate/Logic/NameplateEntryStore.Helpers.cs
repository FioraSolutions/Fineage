using System.Collections.Generic;
using UnityEngine;

public sealed partial class NameplateEntryStore
{
    public void Remove(int id, NameplatePixelSnap snap)
    {
        if (_entries.Remove(id))
        {
            _removeObjId = id;
            snap?.Clear(id);
            L2OverheadAnchor.Invalidate(id);
        }
    }

    public void Discover(
        Collider[] hits,
        Color defaultNameColor)
    {
        if (hits == null)
        {
            return;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            Collider c = hits[i];
            if (c == null)
            {
                continue;
            }

            Transform hitT = c.transform;
            if (hitT == null)
            {
                continue;
            }

            // click_area is a child on EntityClick; Entity lives on the root.
            Entity entity = hitT.GetComponentInParent<Entity>();
            if (entity == null || entity.Identity == null)
            {
                continue;
            }

            if (entity.Identity.Id == _removeObjId)
            {
                continue;
            }

            UpsertEntry(entity, defaultNameColor);
        }
    }
}
