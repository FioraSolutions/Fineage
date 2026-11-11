using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class GameManager : MonoBehaviour, IGameManager
{
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
}
