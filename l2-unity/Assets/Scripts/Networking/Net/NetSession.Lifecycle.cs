using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;
using System.Collections.Concurrent;

public sealed partial class NetSession : MonoBehaviour
{
    /// <summary>
    /// Composition-root owner for the single DontDestroyOnLoad session host.
    /// Call from App/Game LifetimeScope bind only — not from gameplay / UI.
    /// </summary>
    public static NetSession CreateForCompositionRoot()
    {
        if (Instance != null)
            return Instance;
        var go = new GameObject("NetSession");
        DontDestroyOnLoad(go);
        Instance = go.AddComponent<NetSession>();
        return Instance;
    }

    /// <summary>
    /// Legacy self-create path. Prefer <see cref="NetworkingPacketServices.Session"/> after
    /// AppLifetimeScope binds the composition-root host.
    /// </summary>
    [System.Obsolete("Use NetworkingPacketServices.Session (bound by AppLifetimeScope). Composition root may call CreateForCompositionRoot.")]
    public static NetSession Ensure()
    {
        Debug.LogWarning(
            "[Net] NetSession.Ensure() bypasses DI — migrate caller to NetworkingPacketServices.Session");
        return CreateForCompositionRoot();
    }

    /// <summary>Bound session host, or null if composition has not created one yet.</summary>
    public static NetSession TryGetBound() => NetworkingPacketServices.Session;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _commandSender = new NetCommandSender(() => _zone, NextSeq);
        _lobbyFlow = new NetLobbyFlow(
            NextSeq,
            Post,
            ResetLobbySessionState,
            characterId => CharacterId = characterId,
            () => SessionObjectId = 0,
            ConnectZoneForHandoffAsync,
            result => AuthResultReceived?.Invoke(result),
            roster => RosterReceived?.Invoke(roster),
            deleted => DeleteResultReceived?.Invoke(deleted),
            handoff => HandoffReceived?.Invoke(handoff));
        _zoneDispatch = new NetZoneDispatch(this, EnterWorldAsync);
    }

    void ResetLobbySessionState()
    {
        CharacterId = 0;
        SessionObjectId = 0;
        NetGearSession.Reset();
        NetShortCutReceiver.Reset();
        NetAutoShot.Reset();
    }

    void Update()
    {
        _mainThread.DrainFrame();

        // ItemList may arrive before the local player / inventory UI exist (same as the legacy bridge).
        NetGearSession.FlushPending();
        // Same for the hotbar: ShortCutInit lands during EnterWorld, before the skillbar.
        NetShortCutReceiver.FlushPending();
    }
}
