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
}
