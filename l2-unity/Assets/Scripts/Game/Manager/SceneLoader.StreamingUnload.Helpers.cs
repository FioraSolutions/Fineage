using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public partial class SceneLoader
{
    IEnumerator InstantiateMapTilePrefabsAsync(string tile)
    {
        if (_prefabTiles.ContainsKey(tile))
            yield break;

        using (FineagePerfProfiling.TileLoad.Auto())
        {
            yield return InstantiateMapTilePrefabsCoreAsync(tile);
        }
    }

    IEnumerator InstantiateMapTilePrefabsCoreAsync(string tile)
    {
        string terrainPath = "Data/Maps/" + tile + "/" + tile;
        ResourceRequest terrainReq = Resources.LoadAsync<GameObject>(terrainPath);
        yield return terrainReq;
        GameObject terrain = terrainReq.asset as GameObject;
        if (terrain == null)
        {
            _worldStreaming = false;
            _streamingBusy = false;
            throw MapTileLoadPolicy.Missing(tile);
        }

        var root = new GameObject(tile);
        Scene game = SceneManager.GetSceneByName(_gameScene);
        if (game.IsValid() && game.isLoaded)
            SceneManager.MoveGameObjectToScene(root, game);

        string[] parts =
        {
            tile,
            tile + "_DecoLayer",
            "StaticMeshes",
            "Brushes",
            tile + "_AmbientSounds"
        };
        string decoBytesPath = "Data/Maps/" + tile + "/" + tile + "_Deco";
        ResourceRequest decoReq = Resources.LoadAsync<TextAsset>(decoBytesPath);
        yield return decoReq;
        TextAsset decoArchive = decoReq.asset as TextAsset;
        bool skipDecoPrefab = decoArchive != null;
        int placed = 0;
        if (skipDecoPrefab)
        {
            var decoGo = new GameObject(tile + "_Deco");
            decoGo.transform.SetParent(root.transform, false);
            int decoLayer = LayerMask.NameToLayer("Deco");
            if (decoLayer < 0)
            {
                FineageLog.Error(
                    FineageLogCategory.World,
                    "[Interlude deco] " + tile + ": physics layer 'Deco' is missing");
                decoLayer = 0;
            }
            decoGo.layer = decoLayer;
            decoGo.AddComponent<L2DecoInstancedRenderer>().SetArchive(decoArchive);
            placed++;
        }
        for (int i = 0; i < parts.Length; i++)
        {
            if (skipDecoPrefab && parts[i] == tile + "_DecoLayer")
                continue;
            string path = "Data/Maps/" + tile + "/" + parts[i];
            ResourceRequest req = Resources.LoadAsync<GameObject>(path);
            yield return req;
            GameObject prefab = req.asset as GameObject;
            if (prefab == null)
                continue;
            if (!skipDecoPrefab && parts[i] == tile + "_DecoLayer")
            {
                FineageLog.Error(
                    FineageLogCategory.World,
                    "[Interlude deco] " + tile + ": no " + tile + "_Deco.bytes; loaded " + tile
                    + "_DecoLayer prefab, waiting for L2DC rebake");
            }
            GameObject go = UnityEngine.Object.Instantiate(prefab, root.transform);
            go.name = prefab.name;
            go.transform.SetPositionAndRotation(prefab.transform.position, prefab.transform.rotation);
            placed++;
        }

        _prefabTiles[tile] = root;
        FineageLog.Info(
            FineageLogCategory.World,
            "LoadSWMap prefab-tile '" + tile + "' placed=" + placed);
    }

    void DestroyPrefabTile(string tile)
    {
        if (!_prefabTiles.TryGetValue(tile, out GameObject root))
            return;
        _prefabTiles.Remove(tile);
        if (root != null)
            UnityEngine.Object.Destroy(root);
    }

    void DestroyAllPrefabTiles()
    {
        foreach (KeyValuePair<string, GameObject> pair in _prefabTiles)
        {
            if (pair.Value != null)
                UnityEngine.Object.Destroy(pair.Value);
        }
        _prefabTiles.Clear();
    }
}
