using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public partial class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            // Stay on Game.prefab — that root already has DontDestroyOnLoad.
            // Do not unparent into a second DDOL object (that leaked LoadingCamera).
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }

    public void LoadMenu()
    {
        _worldStreaming = false;
        _streamingBusy = false;
        _loadedTiles.Clear();
        DestroyAllPrefabTiles();
        Manager.OnStartingGame();
        if (SceneManager.GetActiveScene().name != _menuScene)
        {
            SwitchScene(_menuScene, (AsyncOperation o) =>
            {
                StartCoroutine(LoadLobbyAndCacheGame());
            });
            return;
        }

        StartCoroutine(LoadLobbyAndCacheGame());
    }

    private IEnumerator LoadLobbyAndCacheGame()
    {
        Manager.StartLoading();
        yield return LoadSceneAdditiveIfNeeded(_lobbyScene);
        // Lobby meshes exist only after additive load — hide magenta/error mats here.
        yield return null;
        LobbyBrokenMaterialHider.HideBrokenRenderers("after_l2_lobby");
        L2ZoneTerrainDraw.Apply(LobbyMapInterludeContract.ZoneCatalogMap);
        L2Ue2MaterialRuntime.Ensure();
        EnsureLobbyOfficials();
        Manager.OnGameLaunched();
        yield return PreloadGameScene();
    }

    /// World.Start adds the official-effect playbacks in the game world. The
    /// additive lobby has no World, so its 33 Emitter actors (the vista's
    /// BeamEmitter light shafts and the fog03 mist) never spawned at login.
    /// Host the playback on a lobby-scene root: unloading l2_lobby tears it
    /// down with the scene, so the world never sees a second instance.
    void EnsureLobbyOfficials()
    {
        Scene lobby = SceneManager.GetSceneByName(_lobbyScene);
        if (!lobby.IsValid() || !lobby.isLoaded)
            return;
        GameObject host = null;
        foreach (GameObject root in lobby.GetRootGameObjects())
        {
            if (root.name == "LobbyOfficials")
            {
                host = root;
                break;
            }
        }
        if (host == null)
        {
            host = new GameObject("LobbyOfficials");
            SceneManager.MoveGameObjectToScene(host, lobby);
        }
        if (host.GetComponent<OfficialWorldEmitterPlayback>() == null)
            host.AddComponent<OfficialWorldEmitterPlayback>();
    }

    private IEnumerator PreloadGameScene()
    {
        yield return LoadSceneAdditiveIfNeeded(_gameScene);
        // Hide world (incl. Main Camera) immediately so login SwitchCamera cannot
        // treat the gameplay camera as Camera.main during WaitGameUiBuilt.
        SetSceneRootsActive(_gameScene, false, true);
        yield return WaitGameUiBuilt();
        SetGameHudVisible(false);
        // Keep Menu active while lobby Instantiates run — parentless clones land in
        // the active scene; if Game were active they would survive Menu unload.
        Scene menu = SceneManager.GetSceneByName(_menuScene);
        if (menu.IsValid() && menu.isLoaded)
            SceneManager.SetActiveScene(menu);
    }
}
