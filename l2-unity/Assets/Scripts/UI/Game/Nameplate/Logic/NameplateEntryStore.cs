using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// World nameplate entry dictionary: discover, upsert, visibility cull, paint list.
/// </summary>
public sealed partial class NameplateEntryStore
{
    private readonly Dictionary<int, NameplateEntry> _entries;
    private readonly List<int> _removeIds;
    private readonly List<int> _entryKeys;
    private readonly NameplateBubbleResolver _resolver;

    private int _removeObjId;

    public NameplateEntryStore(NameplateBubbleResolver resolver, int capacity = 100)
    {
        _resolver = resolver;
        _entries = new Dictionary<int, NameplateEntry>(capacity);
        _removeIds = new List<int>(32);
        _entryKeys = new List<int>(capacity);
    }

    public int Count => _entries.Count;

    public void Clear()
    {
        _entries.Clear();
        L2OverheadAnchor.ClearAll();
    }

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
