using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemName
{
    [SerializeField] int _id;
    [SerializeField] string _name;
    [SerializeField] string _description;
    [SerializeField] string _defaultAction;
    [SerializeField] bool _tradeable;
    [SerializeField] bool _destructible;
    [SerializeField] bool _droppable;
    [SerializeField] bool _sellable;

    // Runtime set data — not Unity-serialized (ItemSets↔ItemName cycle blew depth limit 10).
    [System.NonSerialized] ItemSets[] _setsThings;
    [System.NonSerialized] readonly List<ItemName> _setNameScratch = new List<ItemName>();

    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public string Description { get => _description; set => _description = value; }
    public string DefaultAction { get => _defaultAction; set => _defaultAction = value; }
    public bool Tradeable { get => _tradeable; set => _tradeable = value; }
    public bool Destructible { get => _destructible; set => _destructible = value; }
    public bool Droppable { get => _droppable; set => _droppable = value; }
    public bool Sellable { get => _sellable; set => _sellable = value; }

    public ItemName[] GetSetsName()
    {
        _setNameScratch.Clear();
        if (_setsThings == null)
            return null;

        foreach (ItemSets setItem in _setsThings)
        {
            if (setItem == null)
                continue;
            int[] array = setItem.GetArrayId();
            if (array == null || array.Length == 0)
                continue;
            for (int i = 0; i < array.Length; i++)
                _setNameScratch.Add(ItemNameTable.Instance.GetItemName(array[i]));
        }

        return _setNameScratch.ToArray();
    }

    public ItemSets[] GetSetsEffect() => _setsThings;

    public void SetSets(ItemSets[] sets)
    {
        if (sets != null)
            _setsThings = sets;
    }
}

public class ItemSets
{
    readonly int[] _id;
    readonly string _description;

    public ItemSets(int[] id, string description)
    {
        _id = id;
        _description = description;
    }

    public int[] GetArrayId() => _id;
    public string GetDescription() => _description;
}
