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
        Scene existing = SceneManager.GetSceneByName(sceneName);
        bool sceneLoaded = existing.IsValid() && existing.isLoaded;

        // A Build Settings scene is not the map. Town scenes such as 19_21 are stubs;
        // the geometry lives in Resources/Data/Maps. Load the scene when it exists,
        // then still place the prefab. Skipping the prefab because the scene loaded
        // is what left Gludio dark and empty (#1843) and left prefab-only tiles as
        // sky (#1842) when the manifest allow-list did not name them.
        if (!sceneLoaded && Application.CanStreamedLevelBeLoaded(sceneName))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (asyncLoad == null)
            {
                FineageLog.Error(
                    FineageLogCategory.World,
                    "LoadSWMap: LoadSceneAsync returned null for '" + sceneName + "'");
            }
            else
            {
                while (!asyncLoad.isDone)
                    yield return null;
                sceneLoaded = true;
            }
        }

        if (sceneLoaded)
            SetSceneRootsActive(sceneName, true);

        if (MapTileLoadPolicy.IsMapTileName(sceneName) && !_prefabTiles.ContainsKey(sceneName))
            yield return InstantiateMapTilePrefabsAsync(sceneName);

        _lastTilePresent = sceneLoaded || _prefabTiles.ContainsKey(sceneName);
    }
}
