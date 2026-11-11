using System.Collections.Generic;
using UnityEngine;

public sealed partial class NameplateEntryStore
{
    private readonly Dictionary<int, NameplateEntry> _entries;
    private readonly List<int> _removeIds;
    private readonly List<int> _entryKeys;
    private readonly NameplateBubbleResolver _resolver;

    private int _removeObjId;


    public int Count => _entries.Count;




}
