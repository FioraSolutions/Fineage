using System;
using System.Collections.Generic;

public partial class RecipeItemMakeInfoDto : IWireDto
{
    public RecipeItemMakeInfoDto()
    {
        _requiredItems = new List<ItemInstance>();
    }
}
