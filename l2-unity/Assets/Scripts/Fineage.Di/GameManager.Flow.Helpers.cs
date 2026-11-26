using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class GameManager : MonoBehaviour, IGameManager
{
    public void RestartToCharacterSelect()
    {
        StartCoroutine(RestartRoutine());
    }

    public void QuitGame()
    {
        Debug.Log("[Fineage] exit game");
        DisconnectZoneSession();
        PlayModeExit.Stop();
    }

    IEnumerator RestartRoutine()
    {
        // Back to the login screen only — no auto-submit (auto re-enter broke textures).
        // Remember-login checkbox prefills the fields; user presses Login.
        Debug.Log("[Fineage] restart → login screen");
        DisconnectZoneSession();
        IsSwitchingServer = false;
        WorldSpawnReady = false;
        GameState = GameState.LOGIN_SCREEN;
        MusicPlayback.TryClear();
        LoadMenuScene();
        yield break;
    }
}
