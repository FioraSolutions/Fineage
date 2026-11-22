using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class GameManager : MonoBehaviour, IGameManager
{
    [Inject]
    void WireLobbyFlow(ILobbySession lobbyFlow)
    {
        _lobbyFlow = lobbyFlow;
        _lobbyFlow.Attach(this);
    }

    bool LoadMenuScene()
    {
        if (_sceneLoader == null)
        {
            Debug.LogError("[GameManager] ISceneLoader missing — AppLifetimeScope.RegisterSceneLoader did not register SceneLoader.");
            return false;
        }
        _sceneLoader.LoadMenu();
        return true;
    }

    private void Start() {
        if (_tableInitializer == null)
        {
            Debug.LogError("[GameManager] TableInitializer missing — AppLifetimeScope failed to build.");
            return;
        }
        _tableInitializer.LoadTables();
        if (!LoadMenuScene())
            return;
        string account;
        string password;
        if (FineageLaunchArgs.TryGetAutoLogin(out account, out password))
            StartCoroutine(AutoLoginFromArgs(account, password));
    }
}
