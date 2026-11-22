#if (UNITY_EDITOR) 
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class L2TerrainGenerator
{
    public void GenerateDecoLayers(TerrainData terrainData, L2TerrainInfo terrainInfo) {
        // Flip vertically
        Texture2D[] flippedAlphaMaps = new Texture2D[terrainInfo.decoLayers.Count];
        for (int i = 0; i < terrainInfo.decoLayers.Count; i++) {
            if (terrainInfo.decoLayers[i].densityMap != null) {
                flippedAlphaMaps[i] = TextureUtils.FlipTextureVertically(terrainInfo.decoLayers[i].densityMap);
            }
        }

        DetailPrototype[] detailPrototypes = new DetailPrototype[terrainInfo.decoLayers.Count];
        for (int i = 0; i < terrainInfo.decoLayers.Count; i++) {
            detailPrototypes[i] = new DetailPrototype();
            detailPrototypes[i].prototype = terrainInfo.decoLayers[i].staticMesh;
            detailPrototypes[i].renderMode = DetailRenderMode.VertexLit;
            detailPrototypes[i].usePrototypeMesh = true;
            detailPrototypes[i].useInstancing = true;
            detailPrototypes[i].dryColor = Color.white;
            detailPrototypes[i].healthyColor = Color.white;
            detailPrototypes[i].minHeight = terrainInfo.decoLayers[i].minHeight * ueToUnityUnitScale;
            detailPrototypes[i].maxHeight = terrainInfo.decoLayers[i].maxHeight * ueToUnityUnitScale;
            detailPrototypes[i].minWidth = terrainInfo.decoLayers[i].minWidth * ueToUnityUnitScale;
            detailPrototypes[i].maxWidth = terrainInfo.decoLayers[i].maxWidth * ueToUnityUnitScale;
        }

        terrainData.detailPrototypes = detailPrototypes;

        float maxFadeUu = 0f;
        for (int i = 0; i < terrainInfo.decoLayers.Count; i++) {
            var deco = terrainInfo.decoLayers[i];
            if (deco.fadeMaxUu > maxFadeUu) maxFadeUu = deco.fadeMaxUu;
            Texture2D densityTexture = flippedAlphaMaps[i];
            if (densityTexture == null)
            {
                Debug.LogError("[Interlude deco] " + (deco.densityMapPath ?? ("layer " + i)) + ": density map not loaded, skip placement");
                continue;
            }
            if (!deco.showOnTerrain)
                continue;

            var detailHeight = densityTexture.height;
            var detailWidth = densityTexture.width;

            int[,] detailLayer = new int[detailHeight, detailWidth];

            densityTexture = TextureUtils.DuplicateReadable(densityTexture);
            Color32[] pixels = densityTexture.GetPixels32();

            for (int y = 0; y < detailHeight; y++) {
                for (int x = 0; x < detailWidth; x++) {
                    var px = pixels[y * detailWidth + x];
                    int packed = Mathf.Max(px.r, px.a);
                    detailLayer[x, y] = deco.CountForDensity(packed);
                }
            }

            terrainData.SetDetailLayer(0, 0, i, detailLayer);
        }

        // Per-layer FadeoutRadius is official UU (stored on L2DecoLayer). Unity terrain
        // detail prototypes share one global distance; the instancer/DecoToMesh bake uses fadeMaxUu.
        if (maxFadeUu > 0f)
            terrainData.SetDetailScatterMode(DetailScatterMode.InstanceCountMode);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void GenerateStaticMeshes(L2TerrainInfo terrainInfo) {
        // Multi-tile AutoBake left prior "StaticMeshes" roots in the open scene;
        // GameObject.Find then saved Prefab Variants pointing at the wrong tile.
        foreach (var leftover in UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (leftover != null && leftover.parent == null && leftover.name == "StaticMeshes")
                UnityEngine.Object.DestroyImmediate(leftover.gameObject);
        }

        GameObject staticMeshesGo = new GameObject("StaticMeshes");

        foreach (var staticMesh in terrainInfo.staticMeshes) {
            L2MapStaticMeshBuilder.BuildSingleStaticMesh(staticMesh, staticMeshesGo);
        }
    }

    public void StitchTerrainSeams(Dictionary<string, Terrain> mapTerrains) {
        string[] keys = new string[mapTerrains.Keys.Count];
        mapTerrains.Keys.CopyTo(keys, 0);

        for (int i = 0; i < keys.Length; ++i) {
            string mapID = keys[i];

            Terrain targetTerrain = mapTerrains[mapID];

            string[] zxMapID = mapID.Split('_');

            string zNeighbourMapID = (int.Parse(zxMapID[0]) - 1).ToString() + "_" + zxMapID[1].ToString();
            string xNeighbourMapID = zxMapID[0].ToString() + "_" + (int.Parse(zxMapID[1]) - 1).ToString();


            if (mapTerrains.ContainsKey(zNeighbourMapID)) {
                Terrain neighbourTerrain = mapTerrains[zNeighbourMapID];
                var res = neighbourTerrain.terrainData.heightmapResolution;
                float[,] neighbourHeights = neighbourTerrain.terrainData.GetHeights(0, res - 1, res, 1);
                float verticalDisplacement = neighbourTerrain.transform.position.y - targetTerrain.transform.position.y;
                AdjustHeightsWithVerticalOffset(neighbourHeights, verticalDisplacement, neighbourTerrain.terrainData.heightmapScale.y);
                targetTerrain.terrainData.SetHeights(0, 0, neighbourHeights);
            }

            if (mapTerrains.ContainsKey(xNeighbourMapID)) {
                Terrain neighbourTerrain = mapTerrains[xNeighbourMapID];
                var res = neighbourTerrain.terrainData.heightmapResolution;
                float[,] neighbourHeights = neighbourTerrain.terrainData.GetHeights(res - 1, 0, 1, res);

                float verticalDisplacement = neighbourTerrain.transform.position.y - targetTerrain.transform.position.y;
                AdjustHeightsWithVerticalOffset(neighbourHeights, verticalDisplacement, neighbourTerrain.terrainData.heightmapScale.y);
                targetTerrain.terrainData.SetHeights(0, 0, neighbourHeights);
            }
        }
    }

    private void AdjustHeightsWithVerticalOffset(float[,] neighbourHeights, float verticalDisplacement, float neighbourHeightmapScale) {
        float offsetRatio = verticalDisplacement / neighbourHeightmapScale;
        for (int i = 0; i < neighbourHeights.GetLength(0); i++) {
            for (int j = 0; j < neighbourHeights.GetLength(1); j++) {
                neighbourHeights[i, j] = neighbourHeights[i, j] + offsetRatio;
            }
        }
    }
}
#endif
