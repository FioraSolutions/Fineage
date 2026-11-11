#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static partial class L2InterludeTileBakePipeline
{
    public static List<L2InterludeTileBakeResult> RunTiles(
        IEnumerable<string> tileIds,
        L2InterludeBakeStep steps,
        bool runGlobalMaterialsOnce = true)
    {
        List<L2InterludeTileBakeResult> results = new List<L2InterludeTileBakeResult>();

        if (runGlobalMaterialsOnce && (steps & L2InterludeBakeStep.GenerateMaterials) != 0)
        {
            L2MaterialBuilder.RunSetupMaterials();
        }

        foreach (string tileId in tileIds)
        {
            L2InterludeBakeStep tileSteps = steps;
            if (runGlobalMaterialsOnce)
            {
                tileSteps &= ~L2InterludeBakeStep.GenerateMaterials;
            }

            results.Add(RunSingleTile(tileId, tileSteps));
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return results;
    }
}
#endif
