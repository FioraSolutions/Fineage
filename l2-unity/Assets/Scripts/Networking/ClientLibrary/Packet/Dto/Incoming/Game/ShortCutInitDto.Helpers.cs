using System.Collections.Generic;

public partial class ShortCutInitDto : IWireDto
{
    private int ParcePages(int slot)
    {
        if(slot >= 11)
        {
            return 1;
        }else if(slot > 12 & slot <= 22)
        {
            return 2;
        }
        else if(slot > 22 & slot <= 33)
        {
            return 3;
        }
        return 1;
    }

    private int ConvertWorldSlot(int world_slot , int page)
    {
        if (page == 1)
        {
            return world_slot;
        }
        else
        {
            int all_slot = page * 11;
            return all_slot - world_slot;
        }
    }
}
