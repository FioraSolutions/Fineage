using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class GameManager : MonoBehaviour, IGameManager
{
    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
            return;
        }

        ConfigureBackend();

        if (FindFirstObjectByType<AppLifetimeScope>() == null)
            gameObject.AddComponent<AppLifetimeScope>();
        // AppLifetimeScope may already exist on this GO — still need FMOD listener before lobby.
        DiBootstrap.EnsureAppScope();
    }

    static void ConfigureBackend()
    {
        string netClient = System.Environment.GetEnvironmentVariable("FINEAGE_NET_CLIENT");
        bool pinnedToPublic = netClient == "0"
            || string.Equals(netClient, "false", System.StringComparison.OrdinalIgnoreCase);
        // Standalone second-window / local dual-client: FINEAGE_NET_CLIENT=1|true|local → 127.0.0.1:9106/9777.
        bool forceLocal = netClient == "1"
            || string.Equals(netClient, "true", System.StringComparison.OrdinalIgnoreCase)
            || string.Equals(netClient, "local", System.StringComparison.OrdinalIgnoreCase);

#if UNITY_EDITOR
        bool useLocal = forceLocal || !pinnedToPublic;
#else
        bool useLocal = forceLocal;
#endif

        if (useLocal)
        {
            FineageWireConfig.EnableLocalNetBackend();
            Debug.Log("[Fineage] backend=local login="
                + FineageWireConfig.LoginHost + ":" + FineageWireConfig.LoginPort
                + " zone=" + FineageWireConfig.ZoneHost + ":" + FineageWireConfig.ZonePort);
            return;
        }

#if UNITY_EDITOR
        FineageServerProfile.Refresh();
        if (!pinnedToPublic && FineageServerProfile.IsLocalActive)
        {
            FineageWireConfig.EnablePlaytestBackend(
                FineageServerProfile.LocalHost,
                FineageServerProfile.LocalLoginPort,
                FineageServerProfile.LocalHost,
                FineageServerProfile.LocalGamePort);
            Debug.LogWarning("[Fineage] backend=tunnel-profile login="
                + FineageWireConfig.LoginHost + ":" + FineageWireConfig.LoginPort
                + " zone=" + FineageWireConfig.ZoneHost + ":" + FineageWireConfig.ZonePort);
            return;
        }
#endif

        PlaytestEndpoint ep = PlaytestEndpointRuntime.Reload();
        string loginHost = FineageWireConfig.IsPublicLoginHost(ep.LoginHost)
            ? ep.LoginHost
            : PlaytestEndpoint.DefaultLoginHost;
        string gameHost = FineageWireConfig.IsPublicLoginHost(ep.GameHost)
            ? ep.GameHost
            : PlaytestEndpoint.DefaultGameHost;
        int loginPort = ep.LoginPort > 0 ? ep.LoginPort : PlaytestEndpoint.DefaultLoginPort;
        int gamePort = ep.GamePort > 0 ? ep.GamePort : PlaytestEndpoint.DefaultGamePort;

        FineageWireConfig.EnablePlaytestBackend(loginHost, loginPort, gameHost, gamePort);
        Debug.Log("[Fineage] backend=public-playtest login="
            + FineageWireConfig.LoginHost + ":" + FineageWireConfig.LoginPort
            + " zone=" + FineageWireConfig.ZoneHost + ":" + FineageWireConfig.ZonePort);
    }
}
