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
}
