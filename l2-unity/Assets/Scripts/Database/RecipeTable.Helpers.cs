using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class RecipeTable
{
    private RecipeData CreateModel(string[] ids, int idMk)
    {
        RecipeData recipe = new RecipeData();

        string aliase = DatUtils.CleanupStringOldData(ids[_indexName]);
        recipe.Alias = aliase;
        recipe.IDMk = idMk;
        recipe.RecipeId = Int32.Parse(ids[_indexRecipeId]); ;
        recipe.Level = Int32.Parse(ids[_indexlevel]);
        recipe.ItemId = Int32.Parse(ids[_indexItemId]);
        recipe.Count = Int32.Parse(ids[_indexCount]);
        recipe.MpCost = Int32.Parse(ids[_indexMpCost]);
        recipe.SuccessRate = Int32.Parse(ids[_indexSuccesRate]);

        // Set materials with bounds checking
        SetMaterialSafely(0, recipe, ids);
        SetMaterialSafely(1, recipe, ids);
        SetMaterialSafely(2, recipe, ids);
        SetMaterialSafely(3, recipe, ids);
        SetMaterialSafely(4, recipe, ids);
        SetMaterialSafely(5, recipe, ids);
        SetMaterialSafely(6, recipe, ids);
        SetMaterialSafely(7, recipe, ids);
        SetMaterialSafely(8, recipe, ids);


        return recipe;
    }

    void SetMaterialSafely(int materialIndex, RecipeData recipe, string[] ids)
    {
        int idIndex = 9 + (materialIndex * 2);
        int countIndex = idIndex + 1;

        if (idIndex + 1 < ids.Length)
        {
            int materialId = SafeIntParse(ids, idIndex);
            int materialCount = SafeIntParse(ids, countIndex);
            recipe.SetMaterials(materialId, materialCount, materialIndex);
        }
    }

    private int SafeIntParse(string[] array, int index)
    {
        if (index < 0 || index >= array.Length)
        {
            // Debug.LogWarning($"Index {index} is out of bounds for array of length {array.Length}");
            return 0;
        }

        if (int.TryParse(array[index], out int result))
        {
            return result;
        }
        else
        {
            // Debug.LogWarning($"Failed to parse value '{array[index]}' to integer at index {index}");
            return 0;
        }


    }
}
