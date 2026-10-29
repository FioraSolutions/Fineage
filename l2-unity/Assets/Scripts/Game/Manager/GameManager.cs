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

    private SceneLoader Scenes
    {
        get { return _sceneLoader != null ? _sceneLoader : SceneLoader.Instance; }
    }

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

        // Decide public RP vs explicit local backend once, before any UI can press Log In.
        FineageConnection.Initialize();

        if (FindFirstObjectByType<AppLifetimeScope>() == null)
            gameObject.AddComponent<AppLifetimeScope>();
    }

    [Inject]
    void WireLobbyFlow(LobbyFlowController lobbyFlow)
    {
        _lobbyFlow = lobbyFlow;
        _lobbyFlow.Attach(this);
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
        if (FineageLocalBackend.TryGetAutoLogin(out account, out password))
            StartCoroutine(AutoLoginLocalBackend(account, password));
    }

    private IEnumerator AutoLoginLocalBackend(string account, string password)
    {
        for (int i = 0; i < 30 && !App.HasContainer; i++)
            yield return null;

        Debug.Log("[FineageLocal] auto-login account=" + account);
        ZoneV2Client.Ensure().BeginSession(account, password);
    }

    #endregion

    #region Lobby flow (delegates to LobbyFlowController)

    public void LogIn() => _lobbyFlow.LogIn();

    public void LogOut() => _lobbyFlow.LogOut();

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

        L2GameUI ui = _gameUi != null ? _gameUi : IncomingPacketActions.Ui;
        if (ui != null)
            ui.StartLoading();
        L2LoginUI loginUi = _loginUi != null ? _loginUi : IncomingPacketActions.LoginUi;
        if (loginUi != null)
            loginUi.StartLoading();
    }

    #endregion
}
