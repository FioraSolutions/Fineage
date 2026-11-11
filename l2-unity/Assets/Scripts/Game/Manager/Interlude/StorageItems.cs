using System.Collections.Generic;

public partial class StorageItems
{
    private static StorageItems instance;
    private object _sync = new object();
    private Dictionary <int , ItemInstance> items;
    private Dictionary<int, ItemInstance> equip_items;
    private List<Shortcut> shortCuts;
    private bool showWindow;




    public Dictionary<int , ItemInstance>  GetItems()
    {
        //lock (_sync)
       // {
            if (items != null)
            {
                return items;
            }
            return new Dictionary<int , ItemInstance>();
       // }

    }

    public Dictionary<int, ItemInstance> GetEquipItems()
    {
        //lock (_sync)
        // {
        if (equip_items != null)
        {
            return equip_items;
        }
        return new Dictionary<int, ItemInstance>();
        // }

    }


}
