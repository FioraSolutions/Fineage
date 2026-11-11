using System;
using System.Collections.Generic;

public partial class RecipeItemMakeInfoDto : IWireDto
{
    public void ReadFrom(PacketReader reader)
    {
        try
        {
            
            _recipeId = reader.ReadI();
            _isDwarven = reader.ReadI();
            _currentMp = reader.ReadI();
            _maxMp = reader.ReadI();
            _status = reader.ReadI();

            _data = TablePacketServices.Recipes.GetRecipeData(_recipeId);
            if(_data != null)
            {
                RecipeMaterials[] materials = _data.Materials;

                for(int i = 0; i < materials.Length; i++)
                {
                    RecipeMaterials material = materials[i];

                    if(material != null && material.MaterialsMCnt > 0)
                    {
                        var item = new ItemInstance(-1, material.MaterialsMId, ItemLocation.Void, i, material.MaterialsMCnt, ItemCategory.RequiredItem, false, ItemSlot.none, 0, -1);
                        item.SetItemRequiredType(true, material.MaterialsMCnt);
                        _requiredItems.Add(item);
                    }

                }
            }

        }
        catch (Exception)
        {
            // Debug.LogError($"[RecipeItemMakeInfo] Parse error: {ex.Message}");
            _recipeId = -1;
            _isDwarven = -1;
            _currentMp = 0;
            _maxMp = 0;
            _status = -1;
        }
    }

    public override string ToString()
    {
        return $"[RecipeItemMakeInfo] RecipeId: {_recipeId}, Type: {(IsDwarvenRecipe)}, " +
               $"MP: {_currentMp}/{_maxMp}, Status: {_status}";
    }
}
