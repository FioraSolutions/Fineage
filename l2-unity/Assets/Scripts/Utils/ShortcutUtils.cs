using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class ShortcutUtils 
{
    public static object DetectedType(Shortcut data)
    {

        if (data != null)
        {
            switch (data.Type)
            {
                case Shortcut.TYPE_SKILL:
                    return new SkillInstance(data.Id, data.Level, false, false);
                case Shortcut.TYPE_ACTION:
                    return TablePacketServices.ActionNames.GetAction((ActionType)data.Id);
                case Shortcut.TYPE_ITEM:
                    PlayerInventory inventory = InventoryPacketServices.Inventory;
                    if (inventory == null)
                    {
                        Debug.LogError("[Inventory] shortcut item " + data.Id + " has no inventory");
                        return null;
                    }
                    ItemInstance item;
                    string error;
                    if (!inventory.TryResolveShortcut(data.Id, data.Level, out item, out error))
                    {
                        if (!string.IsNullOrEmpty(error))
                            Debug.LogError("[Inventory] " + error);
                        return null;
                    }
                    return item;
                default:
                    Debug.LogError("[Inventory] shortcut type " + data.Type + " is not supported");
                    return null;
            }
        }

        return null;
    }
}
