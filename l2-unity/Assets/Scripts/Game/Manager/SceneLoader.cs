using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string _menuScene = "Menu";
    [SerializeField] private string _lobbyScene = "l2_lobby";
    [SerializeField] private string _gameScene = "Game";
    [SerializeField] private float _tilePreloadDistance = 80f;

    private readonly HashSet<string> _loadedTiles = new HashSet<string>();
    private bool _worldStreaming;
    private bool _streamingBusy;

    GameManager _gameManager;
    [Inject] GameClient _gameClient;

    private GameManager Manager
    {
        get
        {
            if (_gameManager != null)
                return _gameManager;
            if (GameManager.Instance != null)
                return GameManager.Instance;
            return IncomingPacketActions.Manager;
        }
    }

    private GameClient Game
    {
        get { return _gameClient != null ? _gameClient : IncomingPacketActions.Game; }
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
            return IncomingPacketActions.GameWorld;
        }
    }

    public string GameScene { get { return _gameScene; } }

    public static SceneLoader _instance;
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
        GameClient game = Game;
        if (game == null || game.PlayerInfo.Identity == null)
        {
            // Debug.LogError("LoadSWMap: no player xyz");
            return;
        }

        Vector3 l2Pos = game.PlayerInfo.Identity.GetL2jPos();
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
        FineageLog.Info(FineageLog.Category.World, "LoadSWMap tile='" + tile + "' xyz=(" + l2Pos.x + ", " + l2Pos.y + ", " + l2Pos.z + ")");

        if (Geodata.Instance != null)
        {
            Geodata.Instance.LoadMaps(new List<string> { tile });
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
                FineageLog.Category.World,
                (report.Ok ? "LobbyClean PASS " : "LobbyClean FAIL ") + report.Detail);
        }
        catch (System.Exception ex)
        {
            FineageLog.Exception(FineageLog.Category.World, "LobbyCleanGate", ex);
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
            FineageLog.Exception(FineageLog.Category.World, "WorldEnterDiag." + phase, ex);
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

    private void Update()
    {
        if (!_worldStreaming || _streamingBusy || PlayerController.Instance == null)
        {
            return;
        }

        Vector3 l2Pos = VectorUtils.ConvertPosUnityToL2j(PlayerController.Instance.transform.position);
        List<string> wanted = ResolveSeamlessTiles(l2Pos);
        if (wanted.Count == 0)
        {
            return;
        }

        for (int i = 0; i < wanted.Count; i++)
        {
            string tile = wanted[i];
            if (_loadedTiles.Contains(tile) || !Application.CanStreamedLevelBeLoaded(tile))
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

        float mapSize = Geodata.Instance != null ? Geodata.Instance.MapSize : 624.1524f;
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
        if (Geodata.Instance != null)
        {
            Geodata.Instance.LoadMaps(new List<string> { tile });
        }

        yield return LoadSceneAdditiveIfNeeded(tile);
        _loadedTiles.Add(tile);
        _streamingBusy = false;
    }

    private IEnumerator UnloadSWMap(string tile)
    {
        _streamingBusy = true;
        // Debug.Log("UnloadSWMap tile='" + tile + "'");
        SetSceneRootsActive(tile, false);
        UnloadScene(tile);
        _loadedTiles.Remove(tile);
        _streamingBusy = false;
        yield break;
    }

    private IEnumerator LoadSceneAdditiveIfNeeded(string sceneName)
    {
        Scene existing = SceneManager.GetSceneByName(sceneName);
        if (existing.IsValid() && existing.isLoaded)
        {
            // Debug.Log("Skipping scene load " + sceneName);
            yield break;
        }

        // Debug.Log("Loading scene " + sceneName);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if (asyncLoad == null)
        {
            // Debug.LogError("Failed to load scene " + sceneName);
            yield break;
        }

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

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
        if (L2GameUI.Instance != null)
        {
            return L2GameUI.Instance;
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
        CharacterSelector selector = CharacterSelector.Instance;
        if (selector != null)
            selector.ClearLobbyVisuals();
        CharacterCreator creator = CharacterCreator.Instance;
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
            FineageLog.Category.World,
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
        CameraController controller = CameraController.Instance;
        if (controller == null)
            controller = UnityEngine.Object.FindFirstObjectByType<CameraController>(FindObjectsInactive.Include);
        if (controller == null)
        {
            FineageLog.Warn(FineageLog.Category.World, "EnsureGameplayCamera: CameraController missing");
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
                FineageLog.Category.World,
                "EnsureGameplayCamera demoted extra MainCamera=" + other.name
                + " scene=" + other.gameObject.scene.name);
        }

        if (NameplatesManager.Instance != null && gameplay != null)
            NameplatesManager.Instance.Camera = gameplay;

        FineageLog.Info(
            FineageLog.Category.World,
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
