using System.Collections.Generic;
using UnityEngine;

public sealed partial class NameplateEntryStore
{
    public NameplateEntryStore(NameplateBubbleResolver resolver, int capacity = 100)
    {
        _resolver = resolver;
        _entries = new Dictionary<int, NameplateEntry>(capacity);
        _removeIds = new List<int>(32);
        _entryKeys = new List<int>(capacity);
    }

    public void Clear()
    {
        _entries.Clear();
        L2OverheadAnchor.ClearAll();
    }
}
