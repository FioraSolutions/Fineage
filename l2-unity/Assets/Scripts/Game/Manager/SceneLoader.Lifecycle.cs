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
        Manager.OnGameLaunched();
        yield return PreloadGameScene();
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
