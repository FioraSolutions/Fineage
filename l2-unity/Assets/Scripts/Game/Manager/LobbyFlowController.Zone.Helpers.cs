using System.Collections.Generic;
using UnityEngine;
using VContainer;

public sealed partial class LobbyFlowController
{
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

        // Invalidate in-world client state before UI/scene transition so deferred
        // entity waits and spawn gates cannot apply after the socket is gone.
        _game.WorldSpawnReady = false;
        World world = GameWorld;
        if (world != null)
            world.CancelPendingEntityWaits();

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
