using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class RecipeTable
{
    private static RecipeTable _instance;
    private static Dictionary<int, RecipeData> _recipesData;

    private int _indexName = 0;
    private int _indexid_mk = 1;
    private int _indexRecipeId = 2;
    private int _indexlevel = 3;
    private int _indexItemId = 4;
    private int _indexCount = 5;
    private int _indexMpCost = 6;
    private int _indexSuccesRate = 7;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static RecipeTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RecipeTable();
                _recipesData = new Dictionary<int, RecipeData>();
            }

            return _instance;
        }
    }











    // Helper method to safely set materials


}
