using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class RecipeTable
{
    public RecipeData GetRecipeData(int recipeId)
    {
        if (_recipesData.ContainsKey(recipeId))
        {
            return _recipesData[recipeId];
        }
        return null;
    }

    public void Initialize()
    {
        try
        {
            string dataPathE = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Recipe-c_interlude.txt");
            ReadRecipeInterlude(dataPathE);
        }
        catch (Exception)
        {
            // Debug.LogError($"Failed to initialize QuestNameTable: {ex.Message}");
        }
    }

    public void ReadRecipeInterlude(string dataPath)
    {

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            int index = 0;
            while ((line = reader.ReadLine()) != null)
            {
                if (index != 0)
                {
                    string[] ids = line.Split('\t');
                    int id = Int32.Parse(ids[_indexid_mk]);
                    RecipeData recipeData = CreateModel(ids, id);
                    if (!_recipesData.ContainsKey(recipeData.IDMk))
                    {
                        _recipesData.Add(recipeData.IDMk , recipeData);
                    }
                }
                index++;
            }

        }

    }
}
