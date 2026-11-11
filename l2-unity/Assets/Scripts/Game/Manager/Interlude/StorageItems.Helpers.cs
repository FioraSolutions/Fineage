using System.Collections.Generic;

public partial class StorageItems
{
    public void AddEquipItems(Dictionary<int, ItemInstance> itemsParce)
    {
        //lock (_sync)
        // {
        equip_items = null;
        equip_items = itemsParce;
        // } 
    }

    public void AddShow(bool show)
    {
        showWindow = show;
    }

    public List<Shortcut> GetShortCuts()
    {
        lock (_sync)
        {
            if (shortCuts != null)
            {
                return shortCuts;
            }
            return new List<Shortcut>();
        }

    }

    public bool GetShowWindow()
    {
        return showWindow;
    }
}
