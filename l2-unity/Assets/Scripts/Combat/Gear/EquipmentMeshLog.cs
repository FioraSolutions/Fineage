using System.Collections.Generic;
using UnityEngine;

/// <summary>One warning per item id when a worn piece has no mesh. Inventory icons stay as they are.</summary>
public static class EquipmentMeshLog
{
    static readonly HashSet<string> Seen = new HashSet<string>();

    public static void Once(string kind, int itemId)
    {
        if (itemId == 0 || string.IsNullOrEmpty(kind))
            return;

        string key = kind + ":" + itemId;
        lock (Seen)
        {
            if (!Seen.Add(key))
                return;
        }

        Debug.LogWarning("[Equip] missing " + kind + " mesh for item " + itemId + "; inventory icon stays.");
    }
}
