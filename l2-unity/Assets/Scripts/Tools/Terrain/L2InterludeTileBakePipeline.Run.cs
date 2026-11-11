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

    public static L2InterludeTileBakeResult RunSingleTile(string tileId, L2InterludeBakeStep steps)
    {
        L2InterludeTileBakeResult result = new L2InterludeTileBakeResult { tileId = tileId };

        try
        {
            string t3dAbs = L2InterludeMapPaths.T3dAbsolutePath(tileId);
            bool hasT3d = File.Exists(t3dAbs);
            string t3dAsset = L2InterludeMapPaths.T3dAssetPath(tileId);

            if ((steps & (L2InterludeBakeStep.BuildBrushes |
                          L2InterludeBakeStep.ImportAmbientSounds |
                          L2InterludeBakeStep.BuildAmbientSounds |
                          L2InterludeBakeStep.BuildCameras |
                          L2InterludeBakeStep.GenerateStaticMeshScene |
                          L2InterludeBakeStep.GenerateTerrain)) != 0 && !hasT3d)
            {
                result.log.Add("Skip: missing Meta/" + tileId + ".t3d (export map from client with UModel first).");
            }

            if ((steps & L2InterludeBakeStep.ImportBrushTextures) != 0)
            {
                string brushesJson = Path.GetFullPath(Path.Combine(Application.dataPath, "..", L2InterludeMapPaths.BrushesJsonAssetPath(tileId)));
                if (File.Exists(brushesJson))
                {
                    L2JSONBrushImporter.ImportBrushTexturesFromJson(brushesJson, false);
                    result.log.Add("Import brush textures from Brushes.json");
                }
                else
                {
                    result.log.Add("Skip brush textures: Brushes.json missing");
                }
            }

            if ((steps & L2InterludeBakeStep.ImportStaticMeshes) != 0)
            {
                if (hasT3d)
                {
                    L2T3DStaticMeshImporter.ImportFromT3d(t3dAbs, false);
                    result.log.Add("Import static mesh FBX/textures from t3d list");
                }
                else
                {
                    string listAsset = L2InterludeMapPaths.StaticMeshListAssetPath(tileId);
                    string listAbs = Path.GetFullPath(Path.Combine(Application.dataPath, "..", listAsset));
                    if (File.Exists(listAbs))
                    {
                        L2T3DStaticMeshImporter.ImportFromListFile(listAbs, false);
                        result.log.Add("Import static meshes from list " + Path.GetFileName(listAsset));
                    }
                    else
                    {
                        result.log.Add("Skip static mesh import: no t3d or actor list");
                    }
                }
            }

            if ((steps & L2InterludeBakeStep.GenerateMaterials) != 0)
            {
                L2MaterialBuilder.RunSetupMaterials();
                result.log.Add("Shnok/2 materials");
            }

            if ((steps & L2InterludeBakeStep.GenerateStaticMeshScene) != 0 && hasT3d)
            {
                L2MapStaticMeshBuilder.MeshLocalYawFix = 90f; // +90 confirmed correct by visual check (bridge/ring)
                try { L2TerrainGeneratorTool.GenerateStaticMeshesForMap(tileId); }
                finally { L2MapStaticMeshBuilder.MeshLocalYawFix = 0f; }
                SaveScenePrefab(tileId, "StaticMeshes", "StaticMeshes.prefab");
                result.log.Add("Shnok/3 static mesh scene");
            }

            if ((steps & L2InterludeBakeStep.GenerateTerrain) != 0 && hasT3d)
            {
                Terrain terrain = L2TerrainGeneratorTool.GenerateTerrainForMap(tileId);
                if (terrain != null)
                {
                    SaveTerrainPrefab(tileId, terrain.gameObject);
                    result.log.Add("Shnok/4 terrain + UnityTerrain prefab");
                }
                else
                {
                    result.log.Add("Terrain generation returned null");
                }
            }

            if ((steps & L2InterludeBakeStep.BuildBrushes) != 0 && hasT3d)
            {
                GameObject brushes = L2BrushBuilder.BuildBrushesFromT3d(t3dAbs);
                if (brushes != null)
                {
                    SaveScenePrefab(tileId, brushes, "Brushes.prefab");
                    UnityEngine.Object.DestroyImmediate(brushes);
                    result.log.Add("Shnok/5 brushes");
                }
            }

            if ((steps & L2InterludeBakeStep.ImportAmbientSounds) != 0 && hasT3d)
            {
                L2JSONAmbientSoundImporter.ImportFromT3d(t3dAbs);
                result.log.Add("Shnok/9 ambient sound file copy");
            }

            if ((steps & L2InterludeBakeStep.BuildAmbientSounds) != 0 && hasT3d)
            {
                GameObject sounds = L2AmbientSoundBuilder.BuildAmbientSoundsFromT3d(t3dAbs);
                if (sounds != null)
                {
                    SaveScenePrefab(tileId, sounds, tileId + "_AmbientSounds.prefab");
                    UnityEngine.Object.DestroyImmediate(sounds);
                    result.log.Add("Shnok/9.1 ambient sounds prefab");
                }
            }

            if ((steps & L2InterludeBakeStep.ConvertTerrainToMesh) != 0)
            {
                if (TerrainConverter.ConvertTerrainToMeshForMap(tileId))
                {
                    result.log.Add("Shnok/6 terrain mesh");
                }
                else
                {
                    result.log.Add("Skip Shnok/6: terrain prefab missing");
                }
            }

            if ((steps & L2InterludeBakeStep.GenerateDecoLayerMesh) != 0)
            {
                if (TerrainConverter.GenerateDecoLayerMeshForMap(tileId))
                {
                    result.log.Add("Shnok/7 deco layer");
                }
                else
                {
                    result.log.Add("Skip Shnok/7: terrain prefab or t3d missing");
                }
            }

            if ((steps & L2InterludeBakeStep.BuildCameras) != 0 && hasT3d)
            {
                GameObject cameras = L2CameraBuilder.BuildCamerasFromT3d(t3dAbs);
                if (cameras != null)
                {
                    SaveScenePrefab(tileId, cameras, "Cameras.prefab");
                    UnityEngine.Object.DestroyImmediate(cameras);
                    result.log.Add("Shnok/8 cameras");
                }
            }

            result.success = true;
        }
        catch (Exception ex)
        {
            result.success = false;
            result.log.Add("ERROR: " + ex.Message);
            Debug.LogException(ex);
        }

        return result;
    }
}
