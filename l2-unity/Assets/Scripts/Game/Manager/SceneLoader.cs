using System;

// fineage-touch 1794037730.455447
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public partial class SceneLoader : MonoBehaviour
{
    [SerializeField] private string _menuScene = "Menu";
    [SerializeField] private string _lobbyScene = "l2_lobby";
    [SerializeField] private string _gameScene = "Game";
    [SerializeField] private float _tilePreloadDistance = 80f;

    private readonly HashSet<string> _loadedTiles = new HashSet<string>();
    private readonly Dictionary<string, GameObject> _prefabTiles = new Dictionary<string, GameObject>();
    private bool _worldStreaming;
    private bool _streamingBusy;

    GameManager _gameManager;

    private GameManager Manager
    {
        get
        {
            if (_gameManager != null)
                return _gameManager;
            return GamePacketServices.Manager ?? WorldPacketServices.Manager;
        }
    }

    private World GameWorld
    {
        get
        {
            if (App.GameContainer != null)
            {
                try
                {
                    return App.GameContainer.Resolve<World>();
                }
                catch
                {
                }
            }
            return WorldApplyPacketServices.GameWorld ?? GamePacketServices.GameWorld;
        }
    }

    public string GameScene { get { return _gameScene; } }

    public static SceneLoader _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static SceneLoader Instance { get { return _instance; } }

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

    public void LoadSWMap()
    {
        IZoneTransport transport = ZoneCommands.Transport;
        if (transport == null || transport.PlayerInfo.Identity == null)
        {
            Debug.LogError("LoadSWMap: no player xyz (PlayerInfo.Identity null) — loading UI will stick");
            return;
        }

        Vector3 l2Pos = transport.PlayerInfo.Identity.GetL2jPos();
        StartCoroutine(LoadSWMapRoutine(l2Pos));
    }

    private IEnumerator LoadSWMapRoutine(Vector3 l2Pos)
    {
        // Textbook: drop lobby island first. No Menu/l2_lobby overlap with world tile load.
        yield return UnloadLobbyScenesRoutine();

        yield return LoadSceneAdditiveIfNeeded(_gameScene);
        SetSceneRootsActive(_gameScene, true, true);
        EnsureGameplayCamera();

        Scene game = SceneManager.GetSceneByName(_gameScene);
        if (game.IsValid() && game.isLoaded)
        {
            SceneManager.SetActiveScene(game);
        }

        Manager.OnWorldLoading();
        SetGameHudVisible(true);

        string tile = VectorUtils.GetSwMapName(l2Pos);
        FineageLog.Info(FineageLogCategory.World, "LoadSWMap tile='" + tile + "' xyz=(" + l2Pos.x + ", " + l2Pos.y + ", " + l2Pos.z + ")");

        Geodata geodata = WorldPacketServices.Geodata;
        if (geodata != null)
        {
            geodata.LoadMaps(new List<string> { tile });
        }

        yield return LoadSceneAdditiveIfNeeded(tile);
        _loadedTiles.Add(tile);

        FinishLoadSWMap();
        EnsureGameplayCamera();
        WorldClock.EnsurePersistent();
        _worldStreaming = true;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        StartCoroutine(WorldEnterDiagDelayed());
#endif
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private IEnumerator WorldEnterDiagDelayed()
    {
        yield return null;
        yield return null;
        SafeWorldSnapshot("LoadSWMap +2 frames");
        yield return new WaitForSecondsRealtime(1f);
        SafeWorldSnapshot("LoadSWMap +1s");
        try
        {
            LobbyCleanGate.Report report = LobbyCleanGate.Evaluate();
            FineageLog.Info(
                FineageLogCategory.World,
                (report.Ok ? "LobbyClean PASS " : "LobbyClean FAIL ") + report.Detail);
        }
        catch (System.Exception ex)
        {
            FineageLog.Exception(FineageLogCategory.World, "LobbyCleanGate", ex);
        }
    }

    static void SafeWorldSnapshot(string phase)
    {
        try
        {
            WorldEnterDiag.Snapshot(phase);
        }
        catch (System.Exception ex)
        {
            FineageLog.Exception(FineageLogCategory.World, "WorldEnterDiag." + phase, ex);
        }
    }
#endif

    private void FinishLoadSWMap()
    {
        if (GameWorld != null)
        {
            Manager.OnWorldSceneLoaded();
        }
    }

}
