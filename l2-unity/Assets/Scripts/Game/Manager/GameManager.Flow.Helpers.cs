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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
        MusicPacketServices.Music?.Clear();
        Scenes.LoadMenu();
        yield break;
    }
}
