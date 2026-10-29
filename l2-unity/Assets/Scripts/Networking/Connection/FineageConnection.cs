using UnityEngine;

/// <summary>
/// Startup wiring for the connection path. Picks the backend exactly once:
///   * Local Fineage-Backend only when explicitly requested (see <see cref="FineageLocalBackend.IsRequested"/>);
///   * otherwise the public RP, with optional host/port overrides from <see cref="PlaytestEndpointRuntime"/>.
/// Editor Play, Editor "Fineage/Play RP" and shipped players all take the same public path.
/// </summary>
public static class FineageConnection
{
    static bool _initialized;

    public static FineageEndpoints Endpoints
    {
        get { return FineageWireConfig.Endpoints; }
    }

    public static void Initialize()
    {
        if (_initialized)
            return;
        _initialized = true;

        FineageEndpoints endpoints;
        if (!FineageLocalBackend.TryGetEndpoints(out endpoints))
            endpoints = FineageEndpoints.Public(PlaytestEndpointRuntime.Current);

        FineageWireConfig.Use(endpoints);
        Debug.Log("[Fineage] connection " + endpoints);
    }
}
