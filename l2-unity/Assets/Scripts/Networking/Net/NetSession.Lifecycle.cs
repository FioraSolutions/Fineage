using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession : MonoBehaviour
{
    public static NetSession Ensure()
    {
        if (Instance != null)
            return Instance;
        var go = new GameObject("NetSession");
        DontDestroyOnLoad(go);
        Instance = go.AddComponent<NetSession>();
        return Instance;
    }

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
