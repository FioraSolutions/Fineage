using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public partial class SceneLoader
{
    private static bool CanLoadTile(string tile)
    {
        return MapTileLoadPolicy.ShouldAttempt(
            tile,
            Application.CanStreamedLevelBeLoaded(tile),
            TileStreamingManifest.Contains(tile));
    }

    private IEnumerator LoadSceneAdditiveIfNeeded(string sceneName)
    {
        _lastTilePresent = false;

        // Map tiles have one source: Resources/Data/Maps/{id}. There is no XX_YY scene.
        if (MapTileLoadPolicy.IsMapTileName(sceneName))
        {
            if (!_prefabTiles.ContainsKey(sceneName))
                yield return InstantiateMapTilePrefabsAsync(sceneName);
            _lastTilePresent = _prefabTiles.ContainsKey(sceneName);
            yield break;
        }

        Scene existing = SceneManager.GetSceneByName(sceneName);
        if (existing.IsValid() && existing.isLoaded)
        {
            SetSceneRootsActive(sceneName, true);
            _lastTilePresent = true;
            yield break;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
            yield break;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if (asyncLoad == null)
        {
            _worldStreaming = false;
            _streamingBusy = false;
            throw new MapTileMissingException(
                sceneName,
                "Assets/Resources/Scenes/" + sceneName + ".unity",
                MapTileLoadPolicy.Catalog);
        }

        while (!asyncLoad.isDone)
            yield return null;
        SetSceneRootsActive(sceneName, true);
        _lastTilePresent = true;
    }
}
