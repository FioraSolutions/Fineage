using System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public partial class SceneLoader
{
    private void Update()
    {
        PlayerController player = ControllerPacketServices.Player ?? WorldApplyPacketServices.Player;
        if (!_worldStreaming || _streamingBusy || player == null)
        {
            return;
        }

        Vector3 l2Pos = VectorUtils.ConvertPosUnityToL2j(player.transform.position);
        List<string> wanted = ResolveSeamlessTiles(l2Pos);
        if (wanted.Count == 0)
        {
            return;
        }

        for (int i = 0; i < wanted.Count; i++)
        {
            string tile = wanted[i];
            if (_loadedTiles.Contains(tile) || !CanLoadTile(tile))
            {
                continue;
            }

            StartCoroutine(EventLoadSWMap(tile));
            return;
        }

        foreach (string loaded in _loadedTiles)
        {
            if (wanted.Contains(loaded))
            {
                continue;
            }

            StartCoroutine(UnloadSWMap(loaded));
            return;
        }
    }

    private List<string> ResolveSeamlessTiles(Vector3 l2Pos)
    {
        int tileX;
        int tileY;
        VectorUtils.GetSwMapTile(l2Pos, out tileX, out tileY);

        List<string> wanted = new List<string>();
        AddTile(wanted, tileX, tileY);

        Geodata geodata = WorldPacketServices.Geodata;
        float mapSize = geodata != null ? geodata.MapSize : 624.1524f;
        float edge = mapSize > 0f ? _tilePreloadDistance / mapSize : 0.12f;
        float fx = l2Pos.x / VectorUtils.SwTileSizeUu - Mathf.Floor(l2Pos.x / VectorUtils.SwTileSizeUu);
        float fy = l2Pos.y / VectorUtils.SwTileSizeUu - Mathf.Floor(l2Pos.y / VectorUtils.SwTileSizeUu);

        int dx = 0;
        int dy = 0;
        if (fx < edge)
        {
            dx = -1;
        }
        else if (fx > 1f - edge)
        {
            dx = 1;
        }

        if (fy < edge)
        {
            dy = -1;
        }
        else if (fy > 1f - edge)
        {
            dy = 1;
        }

        if (dx != 0)
        {
            AddTile(wanted, tileX + dx, tileY);
        }

        if (dy != 0)
        {
            AddTile(wanted, tileX, tileY + dy);
        }

        if (dx != 0 && dy != 0)
        {
            AddTile(wanted, tileX + dx, tileY + dy);
        }

        return wanted;
    }

    private static void AddTile(List<string> tiles, int tileX, int tileY)
    {
        string name = VectorUtils.FormatSwMapName(tileX, tileY);
        if (!tiles.Contains(name))
        {
            tiles.Add(name);
        }
    }

    private IEnumerator EventLoadSWMap(string tile)
    {
        _streamingBusy = true;
        // Debug.Log("EventLoadSWMap tile='" + tile + "'");
        Geodata geodata = WorldPacketServices.Geodata;
        if (geodata != null)
        {
            geodata.LoadMaps(new List<string> { tile });
        }

        yield return LoadSceneAdditiveIfNeeded(tile);
        _loadedTiles.Add(tile);
        _streamingBusy = false;
    }

    private IEnumerator UnloadSWMap(string tile)
    {
        _streamingBusy = true;
        // Debug.Log("UnloadSWMap tile='" + tile + "'");
        if (_prefabTiles.ContainsKey(tile))
        {
            DestroyPrefabTile(tile);
        }
        else
        {
            SetSceneRootsActive(tile, false);
            UnloadScene(tile);
        }

        _loadedTiles.Remove(tile);
        _streamingBusy = false;
        yield break;
    }

    private static bool CanLoadTile(string tile)
    {
        if (Application.CanStreamedLevelBeLoaded(tile))
            return true;
        // Baked Maps/{tile}/{tile}.prefab is enough when the Unity scene was never generated.
        return Resources.Load<GameObject>("Data/Maps/" + tile + "/" + tile) != null;
    }

    private IEnumerator LoadSceneAdditiveIfNeeded(string sceneName)
    {
        Scene existing = SceneManager.GetSceneByName(sceneName);
        if (existing.IsValid() && existing.isLoaded)
        {
            // Debug.Log("Skipping scene load " + sceneName);
            yield break;
        }

        if (_prefabTiles.ContainsKey(sceneName))
            yield break;

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (asyncLoad == null)
                yield break;

            while (!asyncLoad.isDone)
                yield return null;
            yield break;
        }

        // Orc Immortal Plateau (18_14), DE (20_18), Dwarf (23_12) ship as map prefabs but
        // often lack Build Settings scenes — without this fallback the world is a white void.
        if (IsMapTileName(sceneName))
            InstantiateMapTilePrefabs(sceneName);
    }

    static bool IsMapTileName(string name)
    {
        if (string.IsNullOrEmpty(name) || name.Length != 5 || name[2] != '_')
            return false;
        return char.IsDigit(name[0]) && char.IsDigit(name[1])
            && char.IsDigit(name[3]) && char.IsDigit(name[4]);
    }

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
