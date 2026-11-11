using System;
using System.Collections.Generic;

public partial class RecipeItemMakeInfoDto : IWireDto
{
    private int _recipeId;
    private int _isDwarven; // 0 = dwarven, 1 = common
    private int _currentMp;
    private int _maxMp;
    private int _status;
    private RecipeData _data;
    private List<ItemInstance> _requiredItems;
    public int RecipeId => _recipeId;

    public RecipeData RecipeData => _data;

    public List<ItemInstance>  RequiredItems => _requiredItems;
    public int IsDwarvenRecipe => _isDwarven;
    public int CurrentMp => _currentMp;
    public int MaxMp => _maxMp;
    public int CraftStatus => _status;



}
