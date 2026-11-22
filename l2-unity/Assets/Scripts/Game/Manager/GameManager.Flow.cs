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

    public void WireLoginUi(IUiLoading loginUi)
    {
        if (loginUi != null)
            _loginUi = loginUi;
    }

    public void WireGameUi(IUiLoading gameUi)
    {
        if (gameUi != null)
            _gameUi = gameUi;
    }

    public void StartLoading()
    {
        if (_loadingCamera != null)
            _loadingCamera.enabled = true;

        if (_gameUi != null)
            _gameUi.StartLoading();
        if (_loginUi != null)
            _loginUi.StartLoading();
        if (_gameUi == null && _loginUi == null)
            Debug.LogError("[GameManager] IUiLoading missing — WireGameUi and WireLoginUi were not called. GameLifetimeScope should register L2GameUI; LoginLifetimeScope should register L2LoginUI.");
    }
}
