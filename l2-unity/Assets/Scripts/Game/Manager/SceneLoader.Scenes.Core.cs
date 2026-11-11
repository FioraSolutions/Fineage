using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public partial class SceneLoader
{
    private IEnumerator WaitGameUiBuilt()
    {
        float timeout = Time.realtimeSinceStartup + 8f;
        while (Time.realtimeSinceStartup < timeout)
        {
            L2GameUI ui = GetGameUi();
            if (ui != null && ui.AreWindowsReady())
            {
                // Debug.Log("Game UI windows ready.");
                yield break;
            }

            yield return null;
        }

        // Debug.LogWarning("Game UI windows were not ready before timeout.");
    }

    private void SetGameHudVisible(bool visible)
    {
        L2GameUI ui = GetGameUi();
        if (ui != null)
        {
            ui.SetHudVisible(visible);
        }
    }

    private static L2GameUI GetGameUi()
    {
        if (SystemUIPacketServices.Ui != null)
        {
            return SystemUIPacketServices.Ui;
        }

        return FindFirstObjectByType<L2GameUI>(FindObjectsInactive.Include);
    }

    private static bool IsGameUiRoot(GameObject root)
    {
        return root != null && root.GetComponentInChildren<L2GameUI>(true) != null;
    }

    private static bool IsSceneCatalogRoot(GameObject root)
    {
        return root != null && root.name == "Prefabs";
    }
}
