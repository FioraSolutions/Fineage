using System.Collections;
using UnityEngine;

public partial class GameManager : MonoBehaviour, IGameManager
{
    private IEnumerator AutoLoginFromArgs(string account, string password)
    {
        Debug.Log("[Fineage] auto-login account=" + account);
        if (_zoneTransport == null)
        {
            Debug.LogError("[GameManager] IZoneTransport missing — AppLifetimeScope did not register FineageZoneTransport, so auto-login cannot begin a session.");
            yield break;
        }

        _zoneTransport.BeginSession(account, password);
    }

    public static void DisconnectZoneSession()
    {
        if (_instance == null || _instance._zoneTransport == null)
        {
            Debug.LogError("[GameManager] IZoneTransport missing — AppLifetimeScope did not register FineageZoneTransport, so the session host was not destroyed.");
            return;
        }

        _instance._zoneTransport.ReleaseSessionHost();
    }
}
