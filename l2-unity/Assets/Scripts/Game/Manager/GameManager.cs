using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class GameManager : MonoBehaviour, IGameManager {

    #region Configuration ── protocol version, inspector toggles

    private int _protocolVersion = 746;
    [SerializeField] private GameState _gameState = GameState.LOGIN_SCREEN;
    private bool _gameReady = false;
    [SerializeField] private Camera _loadingCamera;

    #endregion

    #region Dependencies

    [Inject] TableInitializer _tableInitializer;
    [Inject] SceneLoader _sceneLoader;

    LobbyFlowController _lobbyFlow;
    L2GameUI _gameUi;
    L2LoginUI _loginUi;

    #endregion

    #region Lifecycle

    private SceneLoader Scenes => LobbyFlowPacketServices.Scenes;

    public bool IsSwitchingServer { get; set; } = false;
    public bool WorldSpawnReady { get; internal set; }

    public GameState GameState {
        get { return _gameState; }
        set {
            _gameState = value;
        }
    }
    public bool GameReady { get { return _gameReady; } set { _gameReady = value; } }
    public int ProtocolVersion { get { return _protocolVersion; } }

    private static GameManager _instance;
    [System.Obsolete("Use [Inject] instead")]
    public static GameManager Instance { get { return _instance; } }

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
            return;
        }

        ConfigureBackend();

        if (FindFirstObjectByType<AppLifetimeScope>() == null)
            gameObject.AddComponent<AppLifetimeScope>();
        // AppLifetimeScope may already exist on this GO — still need FMOD listener before lobby.
        DiBootstrap.EnsureAppScope();
    }

    /// <summary>
    /// Pick the backend the client dials. Net is the only wire, so this chooses hosts, not
    /// a protocol.
    ///
    /// <para>
    /// Precedence: an explicit <c>FINEAGE_NET_CLIENT=0</c> still wins, because that is how a
    /// developer pins a public-playtest build while testing locally. Otherwise the Editor
    /// defaults to loopback (the Dock has no environment) unless Fineage/Play Local selects
    /// the tunnel profile.
    /// </para>
    /// </summary>
    static void ConfigureBackend()
    {
        string netClient = System.Environment.GetEnvironmentVariable("FINEAGE_NET_CLIENT");
        bool pinnedToPublic = netClient == "0"
            || string.Equals(netClient, "false", System.StringComparison.OrdinalIgnoreCase);
        // Standalone second-window / local dual-client: FINEAGE_NET_CLIENT=1|true|local → 127.0.0.1:9106/9777.
        bool forceLocal = netClient == "1"
            || string.Equals(netClient, "true", System.StringComparison.OrdinalIgnoreCase)
            || string.Equals(netClient, "local", System.StringComparison.OrdinalIgnoreCase);

#if UNITY_EDITOR
        bool useLocal = forceLocal || !pinnedToPublic;
#else
        bool useLocal = forceLocal;
#endif

        if (useLocal)
        {
            FineageWireConfig.EnableLocalNetBackend();
            Debug.Log("[Fineage] backend=local login="
                + FineageWireConfig.LoginHost + ":" + FineageWireConfig.LoginPort
                + " zone=" + FineageWireConfig.ZoneHost + ":" + FineageWireConfig.ZonePort);
            return;
        }

#if UNITY_EDITOR
        FineageServerProfile.Refresh();
        if (!pinnedToPublic && FineageServerProfile.IsLocalActive)
        {
            FineageWireConfig.EnablePlaytestBackend(
                FineageServerProfile.LocalHost,
                FineageServerProfile.LocalLoginPort,
                FineageServerProfile.LocalHost,
                FineageServerProfile.LocalGamePort);
            Debug.LogWarning("[Fineage] backend=tunnel-profile login="
                + FineageWireConfig.LoginHost + ":" + FineageWireConfig.LoginPort
                + " zone=" + FineageWireConfig.ZoneHost + ":" + FineageWireConfig.ZonePort);
            return;
        }
#endif

        PlaytestEndpoint ep = PlaytestEndpointRuntime.Reload();
        string loginHost = FineageWireConfig.IsPublicLoginHost(ep.LoginHost)
            ? ep.LoginHost
            : PlaytestEndpoint.DefaultLoginHost;
        string gameHost = FineageWireConfig.IsPublicLoginHost(ep.GameHost)
            ? ep.GameHost
            : PlaytestEndpoint.DefaultGameHost;
        int loginPort = ep.LoginPort > 0 ? ep.LoginPort : PlaytestEndpoint.DefaultLoginPort;
        int gamePort = ep.GamePort > 0 ? ep.GamePort : PlaytestEndpoint.DefaultGamePort;

        FineageWireConfig.EnablePlaytestBackend(loginHost, loginPort, gameHost, gamePort);
        Debug.Log("[Fineage] backend=public-playtest login="
            + FineageWireConfig.LoginHost + ":" + FineageWireConfig.LoginPort
            + " zone=" + FineageWireConfig.ZoneHost + ":" + FineageWireConfig.ZonePort);
    }

    [Inject]
    void WireLobbyFlow(LobbyFlowController lobbyFlow)
    {
        _lobbyFlow = lobbyFlow;
        _lobbyFlow.Attach(this);
        LobbyFlowPacketServices.Bind(scenes: _sceneLoader);
    }

    private void Start() {
        if (_tableInitializer == null)
        {
            Debug.LogError("[GameManager] TableInitializer missing — AppLifetimeScope failed to build.");
            return;
        }
        _tableInitializer.LoadTables();
        Scenes.LoadMenu();
        string account;
        string password;
        if (FineageLaunchArgs.TryGetAutoLogin(out account, out password))
            StartCoroutine(AutoLoginFromArgs(account, password));
    }

    private IEnumerator AutoLoginFromArgs(string account, string password)
    {
        for (int i = 0; i < 30 && !App.HasContainer; i++)
            yield return null;

        // Net owns the login, so an auto-login from launch args always has somewhere to go.
        Debug.Log("[Fineage] auto-login account=" + account);
        IZoneTransport transport = null;
        if (App.HasContainer)
        {
            try
            {
                transport = App.Resolve<IZoneTransport>();
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[Fineage] IZoneTransport resolve failed: " + ex.Message);
            }
        }

        if (transport != null)
            transport.BeginSession(account, password);
        else
            Debug.LogError("[Fineage] auto-login: no IZoneTransport resolved — cannot begin a session "
                + "(Fineage Net transport missing).");
    }

    #endregion

    /// <summary>
    /// Tear down the live Net session. Same teardown as
    /// <see cref="FineageZoneTransport.Disconnect"/> (destroy the <see cref="NetSession"/>
    /// GameObject) so QuitGame and RestartRoutine never leave a dangling socket.
    /// </summary>
    public static void DisconnectZoneSession()
    {
        NetSession net = NetworkingPacketServices.Session;
        if (net != null)
            UnityEngine.Object.Destroy(net.gameObject);
    }

    #region Lobby flow (delegates to LobbyFlowController)

    public void LogIn() => _lobbyFlow.LogIn();

    public void LogOut() => _lobbyFlow.LogOut();

    /// <summary>Interlude System Menu → Restart: leave world, back to character select.</summary>
    public void RestartToCharacterSelect()
    {
        StartCoroutine(RestartRoutine());
    }

    /// <summary>Interlude System Menu → Exit: close the client.</summary>
    public void QuitGame()
    {
        Debug.Log("[Fineage] exit game");
        DisconnectZoneSession();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator RestartRoutine()
    {
        // Back to the login screen only — no auto-submit (auto re-enter broke textures).
        // Remember-login checkbox prefills the fields; user presses Login.
        Debug.Log("[Fineage] restart → login screen");
        DisconnectZoneSession();
        IsSwitchingServer = false;
        WorldSpawnReady = false;
        GameState = GameState.LOGIN_SCREEN;
        MusicPacketServices.Music?.Clear();
        Scenes.LoadMenu();
        yield break;
    }

    public void OnWorldSceneLoaded() => _lobbyFlow.OnWorldSceneLoaded();

    public void OnLoginServerConnected() => _lobbyFlow.OnLoginServerConnected();

    public void OnLoginServerAuthAllowed() => _lobbyFlow.OnLoginServerAuthAllowed();

    public void OnLoginServerPlayOk() => _lobbyFlow.OnLoginServerPlayOk();

    public void OnConnectingToGameServer() => _lobbyFlow.OnConnectingToGameServer();

    public void OnReceivedServerList(byte lastServer, List<ServerData> serverData, Dictionary<int, int> charsOnServers) =>
        _lobbyFlow.OnReceivedServerList(lastServer, serverData, charsOnServers);

    public void OnAuthAllowed() => _lobbyFlow.OnAuthAllowed();

    public void OnCharacterSelect() => _lobbyFlow.OnCharacterSelect();

    public void OnCreateUser(List<PlayerTemplates> playerTemplates) => _lobbyFlow.OnCreateUser(playerTemplates);

    public void OnCreateUserFail(string text) => _lobbyFlow.OnCreateUserFail(text);

    public void OnWorldLoading() => _lobbyFlow.OnWorldLoading();

    public void OnRelogin() => _lobbyFlow.OnRelogin();

    public void OnDisconnect() => _lobbyFlow.OnDisconnect();

    public void OnGameserverSelected() => _lobbyFlow.OnGameserverSelected();

    public void OnStartingGame() => _lobbyFlow.OnStartingGame();

    public void OnGameLaunched() => _lobbyFlow.OnGameLaunched();

    public void WireLoginUi(L2LoginUI loginUi)
    {
        if (loginUi != null)
            _loginUi = loginUi;
    }

    public void WireGameUi(L2GameUI gameUi)
    {
        if (gameUi != null)
            _gameUi = gameUi;
    }

    #endregion

    #region Loading UI helpers

    public void StartLoading()
    {
        if (_loadingCamera != null)
        {
            _loadingCamera.enabled = true;
        }

        L2GameUI ui = _gameUi != null ? _gameUi : LobbyUiPacketServices.GameUi;
        if (ui != null)
            ui.StartLoading();
        L2LoginUI loginUi = _loginUi != null ? _loginUi : LobbyUiPacketServices.LoginUi;
        if (loginUi != null)
            loginUi.StartLoading();
    }

    #endregion
}
