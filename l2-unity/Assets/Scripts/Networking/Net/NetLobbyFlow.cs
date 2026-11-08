using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

/// <summary>
/// Lobby transport, auth/roster sends, and inbound lobby packet handling.
/// Owned by <see cref="NetSession"/>; public entry points remain on the session.
/// </summary>
public sealed class NetLobbyFlow
{
    NetTransport _lobby;
    bool _helloLobby;
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

    public async Task BeginLobby(string account, string password, bool registerIfMissing = false)
    {
        _resetLobbySession();
        try
        {
            _lobby?.Dispose();
            _helloLobby = false;
            _lobby = new NetTransport(OnLobbyPacket, reason => _post(() => Debug.Log("[Net] lobby " + reason)));
            await _lobby.ConnectAsync(FineageWireConfig.LoginHost, FineageWireConfig.LoginPort);
            await SendHelloAsync(_lobby, "unity-net");
            await WaitUntil(() => _helloLobby, 5f);
            await _lobby.SendAsync(
                NetOpcode.Auth,
                NetCodec.EncodeAuth(new AuthMsg
                {
                    Account = account,
                    Password = password,
                    RegisterIfMissing = registerIfMissing
                }),
                _nextSeq());
        }
        catch (Exception ex)
        {
            Debug.LogError("[Net] BeginLobby failed: " + ex.Message);
        }
    }

    public async Task RequestRoster()
    {
        if (_lobby == null || !_lobby.Connected)
            return;
        await _lobby.SendAsync(
            NetOpcode.ListCharacters,
            NetCodec.EncodeListCharacters(new ListCharactersMsg()),
            _nextSeq());
    }

    public async Task SelectCharacter(int characterId, string name = "", int slot = -1)
    {
        if (_lobby == null || !_lobby.Connected)
            return;
        await _lobby.SendAsync(
            NetOpcode.SelectCharacter,
            NetCodec.EncodeSelectCharacter(new SelectCharacterMsg
            {
                CharacterId = characterId,
                Name = name ?? "",
                Slot = slot
            }),
            _nextSeq());
    }

    public async Task CreateCharacter(
        string name,
        int classId,
        int race,
        int sex,
        int hairStyle = 0,
        int hairColor = 0,
        int face = 0)
    {
        if (_lobby == null || !_lobby.Connected)
        {
            Debug.LogWarning("[Net] CreateCharacter: lobby not connected");
            return;
        }

        await _lobby.SendAsync(
            NetOpcode.CreateCharacter,
            NetCodec.EncodeCreateCharacter(new CreateCharacterMsg
            {
                Name = name ?? "",
                ClassId = classId,
                Race = race,
                Sex = sex,
                HairStyle = hairStyle,
                HairColor = hairColor,
                Face = face
            }),
            _nextSeq());
    }

    /// <summary>Deletes a character by id (slot = -1) or by roster slot (id = 0).</summary>
    public async Task DeleteCharacter(int characterId, int slot = -1)
    {
        if (_lobby == null || !_lobby.Connected)
        {
            Debug.LogWarning("[Net] DeleteCharacter: lobby not connected");
            return;
        }

        await _lobby.SendAsync(
            NetOpcode.DeleteCharacter,
            NetCodec.EncodeDeleteCharacter(new DeleteCharacterMsg
            {
                CharacterId = characterId,
                Slot = slot
            }),
            _nextSeq());
    }

    /// <summary>Clears a pending delete timer. Same identifying convention as <see cref="DeleteCharacter"/>.</summary>
    public async Task RestoreCharacter(int characterId, int slot = -1)
    {
        if (_lobby == null || !_lobby.Connected)
        {
            Debug.LogWarning("[Net] RestoreCharacter: lobby not connected");
            return;
        }

        await _lobby.SendAsync(
            NetOpcode.RestoreCharacter,
            NetCodec.EncodeRestoreCharacter(new RestoreCharacterMsg
            {
                CharacterId = characterId,
                Slot = slot
            }),
            _nextSeq());
    }

    public async Task EnterZoneWithHandoff(HandoffMsg handoff)
    {
        _handoff = handoff;
        _setCharacterId(handoff != null ? handoff.CharacterId : 0);
        _resetSessionObjectId();
        try
        {
            _lobby?.Dispose();
            _lobby = null;
            await _connectZoneForHandoff(handoff);
        }
        catch (Exception ex)
        {
            Debug.LogError("[Net] EnterZone failed: " + ex.Message);
        }
    }

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
                    _ = RequestRoster();
                break;
            }
            case NetOpcode.CharacterRoster:
            {
                var roster = NetCodec.DecodeCharacterRoster(envelope.Payload);
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
                break;
            }
        }
    }

    static void ApplyRosterUi(CharacterRosterMsg roster)
    {
        LobbyCharacter[] characters = LobbyCharacterMap.FromNetRoster(roster);
        if (LoginPacketServices.CharSelect != null)
            LoginPacketServices.CharSelect.SetLobbyCharacters(characters);
        CharacterSelector selector = LoginPacketServices.Characters;
        if (selector != null)
            selector.SetLobbyCharacters(characters, 0);
        if (LoginPacketServices.CharSelect != null)
            LoginPacketServices.CharSelect.SelectLobbySlot(LobbyCharacterMap.PreferredIndex(characters, 0));
        LoginWindow loginWindow = LoginPacketServices.LoginWindow;
        if (loginWindow != null)
            loginWindow.ShowErrorText("");
        GameManager manager = NetworkingPacketServices.Manager ?? GamePacketServices.Manager;
        if (manager != null)
            manager.OnAuthAllowed();
        Debug.Log("[Net] roster count=" + characters.Length);
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

    static async Task WaitUntil(Func<bool> pred, float seconds)
    {
        float start = Time.realtimeSinceStartup;
        while (!pred())
        {
            if (Time.realtimeSinceStartup - start > seconds)
                throw new TimeoutException("net wait");
            await Task.Delay(20);
        }
    }
}
