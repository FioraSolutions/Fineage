using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public partial class SceneLoader
{
    void InstantiateMapTilePrefabs(string tile)
    {
        if (_prefabTiles.ContainsKey(tile))
            return;

        GameObject terrain = Resources.Load<GameObject>("Data/Maps/" + tile + "/" + tile);
        if (terrain == null)
        {
            FineageLog.Warn(
                FineageLogCategory.World,
                "LoadSWMap: no scene and no map prefab for tile '" + tile + "'");
            return;
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
            GameObject prefab = Resources.Load<GameObject>("Data/Maps/" + tile + "/" + parts[i]);
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
