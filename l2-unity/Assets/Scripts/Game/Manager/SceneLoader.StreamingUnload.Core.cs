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
}
