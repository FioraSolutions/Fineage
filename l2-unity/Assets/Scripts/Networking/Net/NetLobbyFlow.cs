using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

/// <summary>
/// Lobby transport, auth/roster sends, and inbound lobby packet handling.
/// Owned by <see cref="NetSession"/>; public entry points remain on the session.
/// </summary>
public sealed partial class NetLobbyFlow
{
    NetTransport _lobby;
    bool _helloLobby;
    /// <summary>True from successful TCP connect until roster UI or a surfaced login failure.</summary>
    bool _awaitingLoginUi;
    HandoffMsg _handoff;

    readonly Func<uint> _nextSeq;
    readonly Action<Action> _post;
    readonly Action _resetLobbySession;
    readonly Action<int> _setCharacterId;
    readonly Action _resetSessionObjectId;
    readonly Func<HandoffMsg, Task> _connectZoneForHandoff;
    readonly Action<AuthResultMsg> _authResult;
    readonly Action<CharacterRosterMsg> _rosterReceived;
    readonly Action<DeleteResultMsg> _deleteResultReceived;
    readonly Action<HandoffMsg> _handoffReceived;

    public NetLobbyFlow(
        Func<uint> nextSeq,
        Action<Action> post,
        Action resetLobbySession,
        Action<int> setCharacterId,
        Action resetSessionObjectId,
        Func<HandoffMsg, Task> connectZoneForHandoff,
        Action<AuthResultMsg> authResult,
        Action<CharacterRosterMsg> rosterReceived,
        Action<DeleteResultMsg> deleteResultReceived,
        Action<HandoffMsg> handoffReceived)
    {
        _nextSeq = nextSeq ?? throw new ArgumentNullException(nameof(nextSeq));
        _post = post ?? throw new ArgumentNullException(nameof(post));
        _resetLobbySession = resetLobbySession ?? throw new ArgumentNullException(nameof(resetLobbySession));
        _setCharacterId = setCharacterId ?? throw new ArgumentNullException(nameof(setCharacterId));
        _resetSessionObjectId = resetSessionObjectId ?? throw new ArgumentNullException(nameof(resetSessionObjectId));
        _connectZoneForHandoff = connectZoneForHandoff ?? throw new ArgumentNullException(nameof(connectZoneForHandoff));
        _authResult = authResult;
        _rosterReceived = rosterReceived;
        _deleteResultReceived = deleteResultReceived;
        _handoffReceived = handoffReceived;
    }

    public bool HasHandoff => _handoff != null;
    public HandoffMsg LastHandoff => _handoff;

    public void Dispose()
    {
        _lobby?.Dispose();
        _lobby = null;
    }


    void OnLobbyClosed(string reason)
    {
        Debug.Log("[Net] lobby " + reason);
        if (!_awaitingLoginUi)
            return;
        _awaitingLoginUi = false;
        ShowLoginFailure("Connection lost.");
    }





    /// <summary>Deletes a character by id (slot = -1) or by roster slot (id = 0).</summary>

    /// <summary>Clears a pending delete timer. Same identifying convention as <see cref="DeleteCharacter"/>.</summary>


    void OnLobbyPacket(PacketEnvelope envelope)
    {
        if (envelope.Opcode == NetOpcode.HelloAck)
            _helloLobby = true;
        _post(() => HandleLobby(envelope));
    }

    void HandleLobby(PacketEnvelope envelope)
    {
        switch (envelope.Opcode)
        {
            case NetOpcode.HelloAck:
                break;
            case NetOpcode.AuthResult:
            {
                var result = NetCodec.DecodeAuthResult(envelope.Payload);
                _authResult?.Invoke(result);
                if (result.Ok)
                {
                    _ = RequestRoster();
                    break;
                }

                _awaitingLoginUi = false;
                string authLine = string.IsNullOrWhiteSpace(result.Message)
                    ? (string.IsNullOrWhiteSpace(result.Code) ? "Login failed." : result.Code)
                    : result.Message;
                ShowLoginFailure(authLine);
                break;
            }
            case NetOpcode.CharacterRoster:
            {
                var roster = NetCodec.DecodeCharacterRoster(envelope.Payload);
                _awaitingLoginUi = false;
                ApplyRosterUi(roster);
                _rosterReceived?.Invoke(roster);
                break;
            }
            case NetOpcode.CreateResult:
            {
                var created = NetCodec.DecodeCreateResult(envelope.Payload);
                if (!created.Ok)
                {
                    Debug.LogWarning($"[Net] create failed {created.Code}: {created.Message}");
                    if (LoginPacketServices.CharCreate != null)
                        LoginPacketServices.CharCreate.SetlabelError(
                            LobbyCharacterText.ErrorMessage(created.Code, created.Message));
                    break;
                }

                CharacterSummaryNet summary = created.Character;
                int newId = summary != null ? summary.CharacterId : 0;
                Debug.Log("[Net] character created id=" + newId);
                if (LoginPacketServices.CharCreate != null)
                    LoginPacketServices.CharCreate.HideWindow();
                _ = RequestRoster();
                break;
            }
            case NetOpcode.DeleteResult:
            {
                var deleted = NetCodec.DecodeDeleteResult(envelope.Payload);
                if (!deleted.Ok)
                    Debug.LogWarning($"[Net] delete/restore failed {deleted.Code}: {deleted.Message}");
                _deleteResultReceived?.Invoke(deleted);
                break;
            }
            case NetOpcode.Handoff:
            {
                var handoff = NetCodec.DecodeHandoff(envelope.Payload);
                _handoff = handoff;
                _handoffReceived?.Invoke(handoff);
                _ = EnterZoneWithHandoff(handoff);
                break;
            }
            case NetOpcode.Reject:
            {
                var reject = NetCodec.DecodeReject(envelope.Payload);
                Debug.LogWarning($"[Net] lobby reject {reject.Code}: {reject.Message}");
                if (_awaitingLoginUi)
                {
                    _awaitingLoginUi = false;
                    string line = NetRejectLines.Resolve(reject.Code, reject.Message);
                    if (string.IsNullOrEmpty(line))
                        line = string.IsNullOrWhiteSpace(reject.Message) ? "Login failed." : reject.Message;
                    ShowLoginFailure(line);
                }
                break;
            }
        }
    }


    async Task SendHelloAsync(NetTransport transport, string clientName)
    {
        await transport.SendAsync(
            NetOpcode.Hello,
            NetCodec.EncodeHello(new HelloMsg
            {
                ClientName = clientName,
                FeatureFlags = ProtocolCapabilities.FeatureSession
                    | ProtocolCapabilities.FeatureLobby
                    | ProtocolCapabilities.FeatureWorldPresence
            }),
            _nextSeq());
    }

}
