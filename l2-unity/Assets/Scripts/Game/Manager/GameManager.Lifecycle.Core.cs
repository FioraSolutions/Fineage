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

    /// <summary>
    /// Dials whichever backend <see cref="FineageServerProfile"/> resolves: the shared
    /// remote playtest by default, the local Fineage-Backend when the run asks for it
    /// (<c>FINEAGE_BACKEND=local</c>, or Fineage/Play Local in the Editor).
    /// </summary>
    static void ConfigureBackend()
    {
        FineageServerProfile.Refresh();

        if (FineageServerProfile.IsLocalActive)
        {
            FineageWireConfig.EnableLocalNetBackend();
            Debug.Log("[Fineage] backend=local login="
                + FineageWireConfig.LoginHost + ":" + FineageWireConfig.LoginPort
                + " zone=" + FineageWireConfig.ZoneHost + ":" + FineageWireConfig.ZonePort);
            return;
        }

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
        Debug.Log("[Fineage] backend=remote-playtest login="
            + FineageWireConfig.LoginHost + ":" + FineageWireConfig.LoginPort
            + " zone=" + FineageWireConfig.ZoneHost + ":" + FineageWireConfig.ZonePort);
    }
}
