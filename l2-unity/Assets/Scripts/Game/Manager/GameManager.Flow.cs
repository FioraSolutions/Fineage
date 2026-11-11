using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class GameManager : MonoBehaviour, IGameManager
{
    private IEnumerator AutoLoginFromArgs(string account, string password)
    {
        for (int i = 0; i < 30 && !App.HasContainer; i++)
            yield return null;

        // Net owns the login, so an auto-login from launch args always has somewhere to go.
        Debug.Log("[Fineage] auto-login account=" + account);
        IZoneTransport transport = null;
        if (App.HasContainer)
        {
            try
            {
                transport = App.Resolve<IZoneTransport>();
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[Fineage] IZoneTransport resolve failed: " + ex.Message);
            }
        }

        if (transport != null)
            transport.BeginSession(account, password);
        else
            Debug.LogError("[Fineage] auto-login: no IZoneTransport resolved — cannot begin a session "
                + "(Fineage Net transport missing).");
    }

    public static void DisconnectZoneSession()
    {
        NetSession net = NetworkingPacketServices.Session;
        if (net != null)
            UnityEngine.Object.Destroy(net.gameObject);
    }

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
