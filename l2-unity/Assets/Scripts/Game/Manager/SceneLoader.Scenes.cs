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

    /// <summary>
    /// Game.unity ships an inactive <c>Prefabs</c> catalog (faces/hair/npc meshes parked
    /// near the school). It must stay off — enabling all Game roots would show them in-world.
    /// </summary>
    private static bool IsSceneCatalogRoot(GameObject root)
    {
        return root != null && root.name == "Prefabs";
    }

    private void SetSceneRootsActive(string sceneName, bool active, bool keepGameUi = false)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid() || !scene.isLoaded)
        {
            return;
        }

        GameObject[] roots = scene.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
        {
            if (roots[i] == null)
            {
                continue;
            }

            // Catalog stays inactive whether we are showing or hiding the world.
            if (IsSceneCatalogRoot(roots[i]))
            {
                if (roots[i].activeSelf)
                    roots[i].SetActive(false);
                continue;
            }

            if (keepGameUi && IsGameUiRoot(roots[i]))
            {
                continue;
            }

            roots[i].SetActive(active);
        }
    }

    /// <summary>
    /// Destroy lobby presentation, then unload Menu + l2_lobby and wait until gone.
    /// </summary>
    private IEnumerator UnloadLobbyScenesRoutine()
    {
        CharacterSelector selector = LoginPacketServices.Characters;
        if (selector != null)
            selector.ClearLobbyVisuals();
        CharacterCreator creator = LoginPacketServices.Creator;
        if (creator != null)
            creator.DestroyAllPawns();

        // Hide immediately so nothing renders during async unload.
        SetSceneRootsActive(_lobbyScene, false);
        SetSceneRootsActive(_menuScene, false);

        yield return UnloadSceneCompleted(_lobbyScene);
        yield return UnloadSceneCompleted(_menuScene);

        if (selector != null)
            selector.ForgetLobbyVisuals();
        LobbyPresentation.ForgetRoot();

        FineageLog.Info(
            FineageLogCategory.World,
            "Lobby unloaded. MenuLoaded="
            + SceneManager.GetSceneByName(_menuScene).isLoaded
            + " LobbyLoaded="
            + SceneManager.GetSceneByName(_lobbyScene).isLoaded);
    }

    private IEnumerator UnloadSceneCompleted(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid() || !scene.isLoaded)
            yield break;

        AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);
        if (op == null)
            yield break;
        while (!op.isDone)
            yield return null;
    }

    /// <summary>Enable gameplay follow-cam; it must own Camera.main after lobby is gone.</summary>
    static void EnsureGameplayCamera()
    {
        CameraController controller = CameraPacketServices.Controller;
        if (controller == null)
            controller = UnityEngine.Object.FindFirstObjectByType<CameraController>(FindObjectsInactive.Include);
        if (controller == null)
        {
            FineageLog.Warn(FineageLogCategory.World, "EnsureGameplayCamera: CameraController missing");
            return;
        }

        controller.enabled = true;
        Camera gameplay = controller.GetComponent<Camera>();
        if (gameplay != null)
        {
            gameplay.enabled = true;
            if (!gameplay.CompareTag("MainCamera"))
                gameplay.tag = "MainCamera";
        }

        // Strip accidental MainCamera tags — Menu cams die with Menu unload (no DDOL).
        Camera[] cameras = UnityEngine.Object.FindObjectsByType<Camera>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
        for (int i = 0; i < cameras.Length; i++)
        {
            Camera other = cameras[i];
            if (other == null || other == gameplay || !other.CompareTag("MainCamera"))
                continue;
            other.tag = "Untagged";
            other.enabled = false;
            FineageLog.Warn(
                FineageLogCategory.World,
                "EnsureGameplayCamera demoted extra MainCamera=" + other.name
                + " scene=" + other.gameObject.scene.name);
        }

        NameplatesManager nameplates = NameplatePacketServices.Nameplates;
        if (nameplates != null && gameplay != null)
            nameplates.Camera = gameplay;

        // FMOD StudioEventEmitter distance checks require a StudioListener in the scene.
        if (gameplay != null && gameplay.GetComponent<FMODUnity.StudioListener>() == null)
            gameplay.gameObject.AddComponent<FMODUnity.StudioListener>();

        FineageLog.Info(
            FineageLogCategory.World,
            "EnsureGameplayCamera ok name=" + controller.name
            + " camEn=" + (gameplay != null && gameplay.enabled)
            + " Camera.main=" + (Camera.main != null ? Camera.main.name : "null"));
    }

    public void SwitchScene(string sceneName, Action<AsyncOperation> p)
    {
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            // Debug.Log("Switching to scene " + sceneName);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.completed += p;
        }
        else
        {
            // Debug.Log("Skipping scene switch " + sceneName);
        }
    }

    private void UnloadScene(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).IsValid())
            return;
        SceneManager.UnloadSceneAsync(sceneName);
    }
}
