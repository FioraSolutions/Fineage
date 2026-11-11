using System.Collections.Generic;

public partial class StorageItems
{
    private StorageItems()
    { }

    public static StorageItems getInstance()
    {
        if (instance == null)
        {
            instance = new StorageItems();
        }

        return instance;
    }

    public void AddShortCuts(List<Shortcut> shortCuts)
    {
       // lock (_sync)
       // {
            if (this.shortCuts != null)
            {
                this.shortCuts.Clear();
            }

            this.shortCuts = shortCuts;
       // }
       
    }

    public void AddItems(Dictionary<int, ItemInstance> itemsParce)
    {
        //lock (_sync)
       // {
            items = null;
            items = itemsParce;
       // } 
    }
}
