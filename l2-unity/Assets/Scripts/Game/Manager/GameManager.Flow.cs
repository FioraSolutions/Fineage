using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class GameManager : MonoBehaviour, IGameManager
{
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

    public void StartLoading()
    {
        if (_loadingCamera != null)
            _loadingCamera.enabled = true;

        L2GameUI ui = _gameUi != null ? _gameUi : LobbyUiPacketServices.GameUi;
        if (ui != null)
            ui.StartLoading();
        L2LoginUI loginUi = _loginUi != null ? _loginUi : LobbyUiPacketServices.LoginUi;
        if (loginUi != null)
            loginUi.StartLoading();
    }
}
