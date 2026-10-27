using System.Collections.Generic;
using UnityEngine;
using VContainer;

/// <summary>
/// Lobby and login flow callbacks. Extracted from <see cref="GameManager"/> (#141).
/// </summary>
public sealed class LobbyFlowController
{
    private GameManager _game;

    [Inject] GameClient _gameClient;
    [Inject] LoginClient _loginClient;
    [Inject] IZoneTransport _zoneTransport;
    [Inject] SceneLoader _sceneLoader;

    public void Attach(GameManager game)
    {
        _game = game;
    }

    private SceneLoader Scenes
    {
        get { return _sceneLoader != null ? _sceneLoader : SceneLoader.Instance; }
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
        GameObject.Destroy(IncomingPacketActions.LoginUi.gameObject);
        PlayerInfoInterlude playerInfo = _gameClient.PlayerInfo;
        DomainPlayerSpawned spawned = InterludeDomainMap.PlayerSpawned(playerInfo);
        if (IncomingPacketActions.Domain != null && spawned != null)
            IncomingPacketActions.Domain.ApplyPlayerSpawned(spawned);
        else
            IncomingPacketActions.GameWorld.SpawnPlayer(playerInfo.Identity, playerInfo.Status, playerInfo.Stats, playerInfo.Appearance);
        IncomingPacketActions.Ui.StopLoading();
        PlayerStateMachine.Instance.enabled = true;
        _game.WorldSpawnReady = true;
        if (_zoneTransport == null || _zoneTransport.AllowsInterludeGameSocketAuth)
            FineageLog.Info(FineageLog.Category.GameNet, "TX EnterWorld");
        _gameClient.Send(new EnterWorldCommand());
        if (IncomingPacketActions.Domain != null)
            IncomingPacketActions.Domain.FlushKnownlist();
        else
            IncomingPacketActions.ApplyWorld(apply => apply.FlushKnownlist());
        _gameClient.EndLoadWorld();

        PlayerStateMachine.Instance.ChangeState(PlayerState.IDLE);
        PlayerStateMachine.Instance.NotifyEvent(Event.ENTER_WORLD);
    }

    public void OnLoginServerConnected()
    {
        _game.GameState = GameState.LOGIN_CONNECTED;
    }

    public void OnLoginServerAuthAllowed()
    {
        _game.GameState = GameState.READING_LICENSE;

        IncomingPacketActions.LoginUi.ShowLicenseWindow();
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

        IncomingPacketActions.LoginUi.ShowServerSelectWindow();

        IncomingPacketActions.ServerSelect.UpdateServerList(lastServer, serverData, charsOnServers);
    }

    public void OnAuthAllowed()
    {
        _game.GameState = GameState.CHAR_SELECT;
        IncomingPacketActions.LoginCamera.SwitchCamera("CharSelect");

        IncomingPacketActions.LoginUi.ShowCharSelectWindow();
    }

    public void OnCharacterSelect()
    {
        _game.GameState = GameState.IN_GAME;
        _game.WorldSpawnReady = false;

        IncomingPacketActions.LoginUi.StartLoading();
        Scenes.LoadSWMap();
    }

    public void OnCreateUser(List<PlayerTemplates> playerTemplates)
    {
        _game.GameState = GameState.CHAR_CREATION;

        IncomingPacketActions.LoginCamera.SwitchCamera("Login");

        IncomingPacketActions.LoginUi.SetCharTemplations(playerTemplates);
        IncomingPacketActions.LoginUi.ShowCharCreationWindow();
    }

    public void OnCreateUserFail(string text)
    {
        IncomingPacketActions.LoginUi.ShowCharCreationError(text);
    }

    public void OnWorldLoading()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.Clear();
        }

        if (IncomingPacketActions.Ui != null)
        {
            IncomingPacketActions.Ui.StartLoading();
        }
        else if (IncomingPacketActions.LoginUi != null)
        {
            IncomingPacketActions.LoginUi.StartLoading();
        }
    }

    public void OnRelogin()
    {
        _game.IsSwitchingServer = false;
        _game.WorldSpawnReady = false;
        _game.GameState = GameState.LOGIN_SCREEN;

        IncomingPacketActions.LoginCamera.SwitchCamera("Login");

        IncomingPacketActions.LoginUi.ShowLoginWindow();
    }

    public void OnDisconnect()
    {
        if (_game.IsSwitchingServer)
            return;

        if (_game.GameState > GameState.CHAR_CREATION)
        {
            MusicManager.Instance.Clear();
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
            if (IncomingPacketActions.LoginUi != null)
            {
                IncomingPacketActions.LoginUi.StopLoading();
                IncomingPacketActions.LoginUi.SetLoading(true);
            }

            IncomingPacketActions.Creator.SpawnAllPawns();
        }
    }
}
