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
        if (!TileStreamingManifest.Contains(tile))
        {
            FineageLog.Error(
                FineageLogCategory.World,
                "LoadSWMap: tile '" + tile + "' not in TileStreamingManifest and no additive scene.");
            yield break;
        }

        string terrainPath = "Data/Maps/" + tile + "/" + tile;
        ResourceRequest terrainReq = Resources.LoadAsync<GameObject>(terrainPath);
        yield return terrainReq;
        GameObject terrain = terrainReq.asset as GameObject;
        if (terrain == null)
        {
            FineageLog.Warn(
                FineageLogCategory.World,
                "LoadSWMap: manifest lists tile '" + tile + "' but Resources missing '" + terrainPath + "'");
            yield break;
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
        int placed = 0;
        for (int i = 0; i < parts.Length; i++)
        {
            string path = "Data/Maps/" + tile + "/" + parts[i];
            ResourceRequest req = Resources.LoadAsync<GameObject>(path);
            yield return req;
            GameObject prefab = req.asset as GameObject;
            if (prefab == null)
                continue;
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
