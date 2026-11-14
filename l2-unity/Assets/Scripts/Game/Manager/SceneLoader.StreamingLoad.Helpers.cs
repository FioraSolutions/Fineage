using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public partial class SceneLoader
{
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
        yield return ResnapActorsAfterTile();
        _streamingBusy = false;
    }

    /// <summary>
    /// Mesh colliders join the physics scene a frame after the tile. NPCs that
    /// spawned on the geodata node (Gludin hangs ~0.5 m over the stone) drop then.
    /// </summary>
    IEnumerator ResnapActorsAfterTile()
    {
        yield return null;
        yield return new WaitForFixedUpdate();
        World world = GameWorld;
        if (world != null)
            world.ResnapSpawnedActors();
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
}
