using System.Collections.Generic;
using UnityEngine;
using VContainer;

/// <summary>
/// Lobby and login flow callbacks. Extracted from <see cref="GameManager"/> (#141).
/// </summary>
public sealed class LobbyFlowController
{
    private GameManager _game;
    private LoginRuntime _loginRuntime;
    private World _world;
    private IWorldDomainSink _domain;
    private WorldPacketApply _worldApply;
    private PacketApplyQueue _applyQueue;
    private L2GameUI _gameUi;

    [Inject] GameClient _gameClient;
    [Inject] LoginClient _loginClient;
    [Inject] IZoneTransport _zoneTransport;
    [Inject] SceneLoader _sceneLoader;
    [Inject] MusicManager _music;

    public void Attach(GameManager game)
    {
        _game = game;
        WireLobbyFlowPacketServices();
    }

    void WireLobbyFlowPacketServices()
    {
        LobbyFlowPacketServices.Bind(_sceneLoader, playerState: null, music: _music);
    }

    public void WireLogin(LoginRuntime loginRuntime)
    {
        _loginRuntime = loginRuntime;
    }

    public void WireGame(
        World world,
        IWorldDomainSink domain,
        WorldPacketApply worldApply,
        PacketApplyQueue applyQueue,
        L2GameUI gameUi)
    {
        _world = world;
        _domain = domain;
        _worldApply = worldApply;
        _applyQueue = applyQueue;
        _gameUi = gameUi;
    }

    private SceneLoader Scenes => LobbyFlowPacketServices.Scenes;

    private PlayerStateMachine StateMachine => LobbyFlowPacketServices.PlayerState;

    private MusicManager Music => LobbyFlowPacketServices.Music;

    private L2LoginUI LoginUi
    {
        get
        {
            if (_loginRuntime != null && _loginRuntime.Ui != null)
                return _loginRuntime.Ui;
            return IncomingPacketActions.LoginUi;
        }
    }

    private L2GameUI GameUi
    {
        get
        {
            if (_gameUi != null)
                return _gameUi;
            return IncomingPacketActions.Ui;
        }
    }

    private ServerSelectWindow ServerSelect
    {
        get
        {
            if (_loginRuntime != null && _loginRuntime.Servers != null)
                return _loginRuntime.Servers;
            return IncomingPacketActions.ServerSelect;
        }
    }

    private LoginCameraManager LoginCamera
    {
        get
        {
            if (_loginRuntime != null && _loginRuntime.Camera != null)
                return _loginRuntime.Camera;
            return IncomingPacketActions.LoginCamera;
        }
    }

    private CharacterCreator Creator
    {
        get
        {
            if (_loginRuntime != null && _loginRuntime.Creator != null)
                return _loginRuntime.Creator;
            return IncomingPacketActions.Creator;
        }
    }

    private World GameWorld
    {
        get
        {
            if (_world != null)
                return _world;
            return IncomingPacketActions.GameWorld;
        }
    }

    private IWorldDomainSink Domain
    {
        get
        {
            if (_domain != null)
                return _domain;
            return IncomingPacketActions.Domain;
        }
    }

    private WorldPacketApply WorldApply
    {
        get
        {
            if (_worldApply != null)
                return _worldApply;
            return IncomingPacketActions.WorldApply;
        }
    }

    public void LogIn()
    {
    }

    public void LogOut()
    {
        _loginClient.Disconnect();
    }

    public void OnWorldSceneLoaded()
    {
        L2LoginUI loginUi = LoginUi;
        if (loginUi != null)
            GameObject.Destroy(loginUi.gameObject);
        PlayerInfoInterlude playerInfo = _gameClient.PlayerInfo;
        DomainPlayerSpawned spawned = InterludeDomainMap.PlayerSpawned(playerInfo);
        IWorldDomainSink domain = Domain;
        if (domain != null && spawned != null)
            domain.ApplyPlayerSpawned(spawned);
        else
            GameWorld.SpawnPlayer(playerInfo.Identity, playerInfo.Status, playerInfo.Stats, playerInfo.Appearance);
        L2GameUI ui = GameUi;
        if (ui != null)
            ui.StopLoading();
        PlayerStateMachine stateMachine = StateMachine;
        if (stateMachine != null)
            stateMachine.enabled = true;
        _game.WorldSpawnReady = true;
        if (_zoneTransport == null || _zoneTransport.AllowsInterludeGameSocketAuth)
            FineageLog.Info(FineageLog.Category.GameNet, "TX EnterWorld");
        _gameClient.Send(new EnterWorldCommand());
        if (domain != null)
            domain.FlushKnownlist();
        else
        {
            WorldPacketApply apply = WorldApply;
            if (apply != null)
                apply.FlushKnownlist();
            else
                IncomingPacketActions.ApplyWorld(a => a.FlushKnownlist());
        }
        _gameClient.EndLoadWorld();

        if (stateMachine != null)
        {
            stateMachine.ChangeState(PlayerState.IDLE);
            stateMachine.NotifyEvent(Event.ENTER_WORLD);
        }
    }

    public void OnLoginServerConnected()
    {
        _game.GameState = GameState.LOGIN_CONNECTED;
    }

    public void OnLoginServerAuthAllowed()
    {
        _game.GameState = GameState.READING_LICENSE;

        LoginUi.ShowLicenseWindow();
    }

    public void OnLoginServerPlayOk()
    {
        _game.GameState = GameState.READY_TO_CONNECT;
    }

    public void OnConnectingToGameServer()
    {
        _game.GameState = GameState.CONNECTING_TO_GAMESERVER;
    }

    public void OnReceivedServerList(byte lastServer, List<ServerData> serverData, Dictionary<int, int> charsOnServers)
    {
        _game.GameState = GameState.SERVER_LIST;

        LoginUi.ShowServerSelectWindow();

        ServerSelect.UpdateServerList(lastServer, serverData, charsOnServers);
    }

    public void OnAuthAllowed()
    {
        _game.GameState = GameState.CHAR_SELECT;
        LoginCamera.SwitchCamera("CharSelect");

        LoginUi.ShowCharSelectWindow();
    }

    public void OnCharacterSelect()
    {
        _game.GameState = GameState.IN_GAME;
        _game.WorldSpawnReady = false;

        LoginUi.StartLoading();
        Scenes.LoadSWMap();
    }

    public void OnCreateUser(List<PlayerTemplates> playerTemplates)
    {
        _game.GameState = GameState.CHAR_CREATION;

        LoginCamera.SwitchCamera("Login");

        LoginUi.SetCharTemplations(playerTemplates);
        LoginUi.ShowCharCreationWindow();
    }

    public void OnCreateUserFail(string text)
    {
        LoginUi.ShowCharCreationError(text);
    }

    public void OnWorldLoading()
    {
        MusicManager music = Music;
        if (music != null)
            music.Clear();

        L2GameUI ui = GameUi;
        if (ui != null)
        {
            ui.StartLoading();
        }
        else
        {
            L2LoginUI loginUi = LoginUi;
            if (loginUi != null)
                loginUi.StartLoading();
        }
    }

    public void OnRelogin()
    {
        _game.IsSwitchingServer = false;
        _game.WorldSpawnReady = false;
        _game.GameState = GameState.LOGIN_SCREEN;

        LoginCamera.SwitchCamera("Login");

        LoginUi.ShowLoginWindow();
    }

    public void OnDisconnect()
    {
        if (_game.IsSwitchingServer)
            return;

        if (_game.GameState > GameState.CHAR_CREATION)
        {
            MusicManager music = Music;
            if (music != null)
                music.Clear();
            Scenes.LoadMenu();
        }
        else if (_game.GameState > GameState.LOGIN_SCREEN && !_gameClient.IsConnected && !_loginClient.IsConnected)
        {
            OnRelogin();
        }
    }

    public void OnGameserverSelected()
    {
    }

    public void OnStartingGame()
    {
    }

    public void OnGameLaunched()
    {
        if (GameState.IN_GAME == _game.GameState)
        {
            L2LoginUI loginUi = LoginUi;
            if (loginUi != null)
            {
                loginUi.StopLoading();
                loginUi.SetLoading(true);
            }

            Creator.SpawnAllPawns();
        }
    }
}
