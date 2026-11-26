using System.Collections.Generic;

public partial class InventoryTab
{
    public void UpdateItemList(List<ItemInstance> removeAndAdd, List<ItemInstance> modified, ChangeInventoryData changeInventoryData)
    {
        RebindFromModel();
    }

    public void ModifiedRemove(ItemInstance item)
    {
        RebindFromModel();
    }

    public void AddItem(ItemInstance item)
    {
        RebindFromModel();
    }

    void RebindFromModel()
    {
        InventoryWindow window = InventoryPacketServices.Window;
        if (window == null)
        {
            UnityEngine.Debug.LogError("[Inventory] tab update has no window");
            return;
        }
        window.RebindFromModel();
    }
}
