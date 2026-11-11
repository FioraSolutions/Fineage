using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class GameManager : MonoBehaviour, IGameManager
{

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


    public void OnWorldSceneLoaded() => _lobbyFlow.OnWorldSceneLoaded();

    public void OnLoginServerConnected() => _lobbyFlow.OnLoginServerConnected();

    public void OnLoginServerAuthAllowed() => _lobbyFlow.OnLoginServerAuthAllowed();

    public void OnLoginServerPlayOk() => _lobbyFlow.OnLoginServerPlayOk();

    public void OnConnectingToGameServer() => _lobbyFlow.OnConnectingToGameServer();

}
