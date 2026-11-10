using System.Collections.Generic;
using UnityEngine;
using VContainer;

public sealed partial class LobbyFlowController
{
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
        MusicManager music = MusicPacketServices.Music;
        if (music != null)
        {
            music.Clear();
        }

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
            MusicPacketServices.Music?.Clear();
            Scenes.LoadMenu();
        }
        else if (_game.GameState > GameState.LOGIN_SCREEN && !ZoneCommands.IsConnected)
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
        if (_game.GameState != GameState.IN_GAME)
            return;

        L2LoginUI loginUi = LoginUi;
        if (loginUi == null)
            return;
        loginUi.StopLoading();
        loginUi.SetLoading(true);
    }
}
