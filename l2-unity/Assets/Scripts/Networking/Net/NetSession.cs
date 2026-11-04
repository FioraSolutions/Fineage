using System;
using System.Collections.Concurrent;
using Fineage.Net.Contract;
using UnityEngine;

/// <summary>
/// Fineage Net session: lobby + zone over PacketEnvelope. Maps inbound events to
/// <see cref="IWorldDomainSink"/> — no NetDomainMap / Interlude DTO bridge.
/// </summary>
public sealed class NetSession : MonoBehaviour
{
    public static NetSession Instance { get; private set; }

    readonly ConcurrentQueue<Action> _main = new ConcurrentQueue<Action>();
    NetTransport _lobby;
    NetTransport _zone;
    uint _seq = 1;
    IWorldDomainSink _sink;
    CombatAttackApplyService _combatAttack;
    CombatDieApplyService _combatDie;
    CombatTargetApplyService _combatTarget;
    StatusUpdateApplyService _statusUpdate;
    DeleteObjectGroundApplyService _deleteObjectGround;
    StatusApplyService _status;
    SkillApplyService _skills;
    PresenceApplyService _presence;
    InventoryApplyService _inventoryApply;
    PartyApplyService _party;
    TradeApplyService _trade;
    TradeRequestApplyService _tradeRequest;
    string _account;
    HandoffMsg _handoff;
    bool _helloLobby;
    bool _helloZone;

    public bool HasHandoff => _handoff != null;
    public HandoffMsg LastHandoff => _handoff;

    /// <summary>
    /// Stable lobby character id of the selected character, captured from the handoff in
    /// <see cref="EnterZoneWithHandoff"/>. Unlike <see cref="SessionObjectId"/> it does not
    /// change on every EnterWorld, so callers keyed by character (shortcuts, preferences)
    /// prefer it. Zero until a character has been selected, and reset in <see cref="BeginLobby"/>.
    /// </summary>
    public int CharacterId { get; private set; }

    /// <summary>
    /// Zone object id of the local player, captured from the self <c>EntitySpawn</c>
    /// received after EnterWorld. Zero until the world spawn arrives, and reset when a
    /// new lobby or zone session begins. Mirrors the old V2 session id so callers such as
    /// <c>PlayerShortcuts</c> and <c>ClanWindow</c> read one source.
    /// </summary>
    public int SessionObjectId { get; private set; }
    public event Action<CharacterRosterMsg> RosterReceived;
    public event Action<DeleteResultMsg> DeleteResultReceived;
    public event Action<AuthResultMsg> AuthResultReceived;
    public event Action<HandoffMsg> HandoffReceived;
    public event Action EnteredWorld;

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
    }

    void Update()
    {
        while (_main.TryDequeue(out Action action))
        {
            try { action(); }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        // ItemList may arrive before the local player / inventory UI exist (same as the legacy bridge).
        NetGearSession.FlushPending();
        // Same for the hotbar: ShortCutInit lands during EnterWorld, before the skillbar.
        NetShortCutReceiver.FlushPending();
    }

    public void BindDomainSink(IWorldDomainSink sink) => _sink = sink;

    /// <summary>
    /// Packet applies are container-constructed (#142 slice 3). NetSession is created by
    /// <see cref="Ensure"/> outside the VContainer scope, so the composition root calls
    /// <see cref="VContainer.IObjectResolver.InjectGameObject"/> on it — which fills the
    /// <c>[Inject]</c> members below. Each is null until then, and every call site keeps the
    /// legacy static facade as a fallback for that window.
    /// </summary>
    [VContainer.Inject]
    public void ConstructApplyServices(
        CombatAttackApplyService combatAttack,
        CombatDieApplyService combatDie,
        CombatTargetApplyService combatTarget,
        StatusUpdateApplyService statusUpdate,
        DeleteObjectGroundApplyService deleteObjectGround,
        StatusApplyService status,
        SkillApplyService skills,
        PresenceApplyService presence,
        InventoryApplyService inventory,
        PartyApplyService party,
        TradeApplyService trade,
        TradeRequestApplyService tradeRequest,
        IWorldDomainSink domain)
    {
        _combatAttack = combatAttack;
        _combatDie = combatDie;
        _combatTarget = combatTarget;
        _statusUpdate = statusUpdate;
        _deleteObjectGround = deleteObjectGround;
        _status = status;
        _skills = skills;
        _presence = presence;
        _inventoryApply = inventory;
        _party = party;
        _trade = trade;
        _tradeRequest = tradeRequest;
        // The sink used to be injected into the static facades only, so this seam never ran.
        // Wire it here as well: with a sink bound, NetSession applies domain moves directly.
        if (domain != null)
            BindDomainSink(domain);
    }

    public async void BeginLobby(string account, string password, bool registerIfMissing = false)
    {
        _account = account;
        CharacterId = 0;
        SessionObjectId = 0;
        NetGearSession.Reset();
        NetShortCutReceiver.Reset();
        try
        {
            _lobby?.Dispose();
            _lobby = new NetTransport(OnLobbyPacket, reason => Post(() => Debug.Log("[Net] lobby " + reason)));
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
                NextSeq());
        }
        catch (Exception ex)
        {
            Debug.LogError("[Net] BeginLobby failed: " + ex.Message);
        }
    }

    public async void RequestRoster()
    {
        if (_lobby == null || !_lobby.Connected)
            return;
        await _lobby.SendAsync(NetOpcode.ListCharacters, NetCodec.EncodeListCharacters(new ListCharactersMsg()), NextSeq());
    }

    public async void SelectCharacter(int characterId, string name = "", int slot = -1)
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
            NextSeq());
    }

    public async void CreateCharacter(
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
            NextSeq());
    }

    /// <summary>Deletes a character by id (slot = -1) or by roster slot (id = 0).</summary>
    public async void DeleteCharacter(int characterId, int slot = -1)
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
            NextSeq());
    }

    /// <summary>Clears a pending delete timer. Same identifying convention as <see cref="DeleteCharacter"/>.</summary>
    public async void RestoreCharacter(int characterId, int slot = -1)
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
            NextSeq());
    }

    public async void EnterZoneWithHandoff(HandoffMsg handoff)
    {
        _handoff = handoff;
        CharacterId = handoff != null ? handoff.CharacterId : 0;
        SessionObjectId = 0;
        try
        {
            _lobby?.Dispose();
            _lobby = null;
            _zone?.Dispose();
            _zone = new NetTransport(OnZonePacket, reason => Post(() => Debug.Log("[Net] zone " + reason)));
            string host = string.IsNullOrWhiteSpace(handoff.ZoneHost) ? FineageWireConfig.ZoneHost : handoff.ZoneHost;
            int port = handoff.ZonePort > 0 ? handoff.ZonePort : FineageWireConfig.ZonePort;
            await _zone.ConnectAsync(host, port);
            await SendHelloAsync(_zone, "unity-net-zone");
            await WaitUntil(() => _helloZone, 5f);
            await _zone.SendAsync(
                NetOpcode.ZoneAuth,
                NetCodec.EncodeZoneAuth(new ZoneAuthMsg { Account = handoff.Account, Token = handoff.Token }),
                NextSeq());
        }
        catch (Exception ex)
        {
            Debug.LogError("[Net] EnterZone failed: " + ex.Message);
        }
    }

    public async void SendMove(int destX, int destY, int destZ)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.MoveInput,
            NetCodec.EncodeMoveInput(new MoveInputMsg { DestX = destX, DestY = destY, DestZ = destZ }),
            NextSeq());
    }

    public async void SendSelectTarget(int targetObjectId)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.SelectTarget,
            NetCodec.EncodeSelectTarget(new SelectTargetNetMsg { TargetObjectId = targetObjectId }),
            NextSeq());
    }

    public async void SendAttackRequest(int targetObjectId)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.AttackRequest,
            NetCodec.EncodeAttackRequest(new AttackRequestNetMsg { TargetObjectId = targetObjectId }),
            NextSeq());
    }

    public async void SendPickupRequest(int groundObjectId)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.PickupRequest,
            NetCodec.EncodePickupRequest(new PickupRequestNetMsg { GroundObjectId = groundObjectId }),
            NextSeq());
    }

    public async void SendDropItemRequest(int objectId, int count, int x, int y, int z)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.DropItemRequest,
            NetCodec.EncodeDropItemRequest(new DropItemRequestNetMsg
            {
                ObjectId = objectId,
                Count = count,
                X = x,
                Y = y,
                Z = z
            }),
            NextSeq());
    }

    public async void SendUseItem(int objectId)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.UseItem,
            NetCodec.EncodeUseItem(new UseItemNetMsg { ObjectId = objectId }),
            NextSeq());
    }

    public async void SendUnequipItem(int paperdollSlot)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.UnequipItem,
            NetCodec.EncodeUnequipItem(new UnequipItemNetMsg { PaperdollSlot = paperdollSlot }),
            NextSeq());
    }

    /// <summary>
    /// Registers one hotbar slot. <paramref name="worldSlot"/> is the client world slot
    /// (slot + page * 12); the zone echoes the result as <c>ShortCutRegister</c> or
    /// <c>ShortCutDelete</c>.
    /// </summary>
    public async void SendShortCutReg(int type, int worldSlot, int id, int level)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.RequestShortCutReg,
            NetCodec.EncodeShortCut(new ShortCutNetMsg
            {
                Type = type,
                Slot = worldSlot,
                Id = id,
                Level = level
            }),
            NextSeq());
    }

    /// <summary>Clears one hotbar slot (world slot = slot + page * 12).</summary>
    public async void SendShortCutDel(int worldSlot)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.RequestShortCutDel,
            NetCodec.EncodeShortCutDel(new ShortCutDelNetMsg { Slot = worldSlot }),
            NextSeq());
    }

    /// <summary>
    /// Position heartbeat (L2 coordinates + heading). The zone reconciles the client's
    /// position against its own copy; without it a walk drifts and then snaps back.
    /// </summary>
    public async void SendValidatePosition(int x, int y, int z, int heading)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.ValidatePosition,
            NetCodec.EncodeValidatePosition(new ValidatePositionMsg
            {
                X = x,
                Y = y,
                Z = z,
                Heading = heading
            }),
            NextSeq());
    }

    public async void SendSay(string text, int chatType)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.Say,
            NetCodec.EncodeSay(new SayNetMsg { Text = text ?? "", ChatType = chatType }),
            NextSeq());
    }

    public async void SendUseSkill(int skillId, int targetObjectId, bool ctrlPressed)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.UseSkill,
            NetCodec.EncodeUseSkill(new UseSkillNetMsg
            {
                SkillId = skillId,
                TargetObjectId = targetObjectId,
                CtrlPressed = ctrlPressed
            }),
            NextSeq());
    }

    public async void SendReviveRequest(int action)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.ReviveRequest,
            NetCodec.EncodeReviveRequest(new ReviveRequestNetMsg { Action = action }),
            NextSeq());
    }

    public async void SendNpcTalk(int npcObjectId)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.NpcTalk,
            NetCodec.EncodeNpcTalk(new NpcTalkNetMsg { NpcObjectId = npcObjectId }),
            NextSeq());
    }

    public async void SendBypassRequest(int npcObjectId, string command)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.BypassRequest,
            NetCodec.EncodeBypassRequest(new BypassRequestNetMsg
            {
                NpcObjectId = npcObjectId,
                Command = command ?? ""
            }),
            NextSeq());
    }

    public void SendNpcLink(int npcObjectId, string command)
    {
        string list;
        int location;
        switch (NetBypass.Classify(command, out list, out location))
        {
            case NetBypass.Teleport:
            case NetBypass.SupportMagic:
            case NetBypass.GiveBlessing:
                // Same bypass verb as V2; specialized opcodes come later.
                SendBypassRequest(npcObjectId, NetBypass.Normalize(command));
                return;
            default:
                SendBypassRequest(npcObjectId, NetBypass.Normalize(command));
                return;
        }
    }

    public async void SendRequestBuyItem(int npcObjectId, int listId, NetIdCount[] items)
    {
        if (_zone == null || !_zone.Connected)
            return;
        ShopCountNet[] rows = MapShopCounts(items);
        await _zone.SendAsync(
            NetOpcode.RequestBuyItem,
            NetCodec.EncodeRequestBuyItem(new RequestBuyItemNetMsg
            {
                NpcObjectId = npcObjectId,
                ListId = listId,
                Items = rows
            }),
            NextSeq());
    }

    public async void SendRequestSellItem(int npcObjectId, NetIdCount[] items)
    {
        if (_zone == null || !_zone.Connected)
            return;
        ShopObjectCountNet[] rows = MapShopObjectCounts(items);
        await _zone.SendAsync(
            NetOpcode.RequestSellItem,
            NetCodec.EncodeRequestSellItem(new RequestSellItemNetMsg
            {
                NpcObjectId = npcObjectId,
                Items = rows
            }),
            NextSeq());
    }

    public async void SendRequestMultiSellChoose(int npcObjectId, int listId, int entryId, int amount)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.RequestMultiSellChoose,
            NetCodec.EncodeRequestMultiSellChoose(new RequestMultiSellChooseNetMsg
            {
                NpcObjectId = npcObjectId,
                ListId = listId,
                EntryId = entryId,
                Amount = amount
            }),
            NextSeq());
    }

    public async void SendWarehouseDeposit(int npcObjectId, NetIdCount[] lines)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.WarehouseDepositRequest,
            NetCodec.EncodeWarehouseDepositRequest(new WarehouseDepositRequestNetMsg
            {
                NpcObjectId = npcObjectId,
                Lines = MapShopObjectCounts(lines)
            }),
            NextSeq());
    }

    public async void SendWarehouseWithdraw(int npcObjectId, NetIdCount[] lines)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.WarehouseWithdrawRequest,
            NetCodec.EncodeWarehouseWithdrawRequest(new WarehouseWithdrawRequestNetMsg
            {
                NpcObjectId = npcObjectId,
                Lines = MapShopObjectCounts(lines)
            }),
            NextSeq());
    }

    public async void SendRequestQuestList()
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(NetOpcode.RequestQuestList, NetCodec.EncodeRequestQuestList(), NextSeq());
    }

    public async void SendRequestQuestAbort(int questId)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.RequestQuestAbort,
            NetCodec.EncodeRequestQuestAbort(new RequestQuestAbortNetMsg { QuestId = questId }),
            NextSeq());
    }

    public async void SendPartyInviteRequest(int targetObjectId, int itemDistribution = 0)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.PartyInviteRequest,
            NetCodec.EncodePartyInviteRequest(new PartyInviteRequestNetMsg
            {
                TargetObjectId = targetObjectId,
                ItemDistribution = itemDistribution
            }),
            NextSeq());
    }

    public async void SendPartyAnswerJoinRequest(int requestorObjectId, int response)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.PartyAnswerJoinRequest,
            NetCodec.EncodePartyAnswerJoinRequest(new PartyAnswerJoinRequestNetMsg
            {
                RequestorObjectId = requestorObjectId,
                Response = response
            }),
            NextSeq());
    }

    public async void SendPartyLeaveRequest()
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(NetOpcode.PartyLeaveRequest, NetCodec.EncodePartyLeaveRequest(), NextSeq());
    }

    public async void SendPartyKickRequest(int targetObjectId)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.PartyKickRequest,
            NetCodec.EncodePartyKickRequest(new PartyKickRequestNetMsg { TargetObjectId = targetObjectId }),
            NextSeq());
    }

    public async void SendTradeRequest(int targetObjectId)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.TradeRequest,
            NetCodec.EncodeTradeRequest(new TradeRequestNetMsg { TargetObjectId = targetObjectId }),
            NextSeq());
    }

    public async void SendTradeAnswerRequest(int requestorObjectId, int response)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.TradeAnswerRequest,
            NetCodec.EncodeTradeAnswerRequest(new TradeAnswerRequestNetMsg
            {
                RequestorObjectId = requestorObjectId,
                Response = response
            }),
            NextSeq());
    }

    public async void SendTradeAddItemRequest(int inventoryObjectId, int count)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.TradeAddItemRequest,
            NetCodec.EncodeTradeAddItemRequest(new TradeAddItemRequestNetMsg
            {
                InventoryObjectId = inventoryObjectId,
                Count = count
            }),
            NextSeq());
    }

    public async void SendTradeDoneRequest(int response)
    {
        if (_zone == null || !_zone.Connected)
            return;
        await _zone.SendAsync(
            NetOpcode.TradeDoneRequest,
            NetCodec.EncodeTradeDoneRequest(new TradeDoneRequestNetMsg { Response = response }),
            NextSeq());
    }

    static ShopCountNet[] MapShopCounts(NetIdCount[] items)
    {
        if (items == null || items.Length == 0)
            return System.Array.Empty<ShopCountNet>();
        var rows = new ShopCountNet[items.Length];
        for (int i = 0; i < items.Length; i++)
            rows[i] = new ShopCountNet { ItemId = items[i].Id, Count = items[i].Count };
        return rows;
    }

    static ShopObjectCountNet[] MapShopObjectCounts(NetIdCount[] items)
    {
        if (items == null || items.Length == 0)
            return System.Array.Empty<ShopObjectCountNet>();
        var rows = new ShopObjectCountNet[items.Length];
        for (int i = 0; i < items.Length; i++)
            rows[i] = new ShopObjectCountNet { ObjectId = items[i].Id, Count = items[i].Count };
        return rows;
    }

    async System.Threading.Tasks.Task SendHelloAsync(NetTransport transport, string clientName)
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
            NextSeq());
    }

    void OnLobbyPacket(PacketEnvelope envelope)
    {
        if (envelope.Opcode == NetOpcode.HelloAck)
            _helloLobby = true;
        Post(() => HandleLobby(envelope));
    }

    void OnZonePacket(PacketEnvelope envelope)
    {
        if (envelope.Opcode == NetOpcode.HelloAck)
            _helloZone = true;
        Post(() => HandleZone(envelope));
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
                AuthResultReceived?.Invoke(result);
                if (result.Ok)
                    RequestRoster();
                break;
            }
            case NetOpcode.CharacterRoster:
            {
                var roster = NetCodec.DecodeCharacterRoster(envelope.Payload);
                ApplyRosterUi(roster);
                RosterReceived?.Invoke(roster);
                break;
            }
            case NetOpcode.CreateResult:
            {
                var created = NetCodec.DecodeCreateResult(envelope.Payload);
                if (!created.Ok)
                {
                    Debug.LogWarning($"[Net] create failed {created.Code}: {created.Message}");
                    if (CharCreationWindow.Instance != null)
                        CharCreationWindow.Instance.SetlabelError(
                            LobbyCharacterText.ErrorMessage(created.Code, created.Message));
                    break;
                }

                CharacterSummaryNet summary = created.Character;
                int newId = summary != null ? summary.CharacterId : 0;
                Debug.Log("[Net] character created id=" + newId);
                if (CharCreationWindow.Instance != null)
                    CharCreationWindow.Instance.HideWindow();
                RequestRoster();
                break;
            }
            case NetOpcode.DeleteResult:
            {
                var deleted = NetCodec.DecodeDeleteResult(envelope.Payload);
                if (!deleted.Ok)
                    Debug.LogWarning($"[Net] delete/restore failed {deleted.Code}: {deleted.Message}");
                DeleteResultReceived?.Invoke(deleted);
                // Success also carries a fresh roster, so the UI redraws from server truth.
                break;
            }
            case NetOpcode.Handoff:
            {
                var handoff = NetCodec.DecodeHandoff(envelope.Payload);
                _handoff = handoff;
                HandoffReceived?.Invoke(handoff);
                EnterZoneWithHandoff(handoff);
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

    void ApplyRosterUi(CharacterRosterMsg roster)
    {
        LobbyCharacter[] characters = LobbyCharacterMap.FromNetRoster(roster);
        if (CharSelectWindow.Instance != null)
            CharSelectWindow.Instance.SetLobbyCharacters(characters);
        if (CharacterSelector.Instance != null)
            CharacterSelector.Instance.SetLobbyCharacters(characters, 0);
        if (CharSelectWindow.Instance != null)
            CharSelectWindow.Instance.SelectLobbySlot(LobbyCharacterMap.PreferredIndex(characters, 0));
        if (LoginWindow.Instance != null)
            LoginWindow.Instance.ShowErrorText("");
        if (GameManager.Instance != null)
            GameManager.Instance.OnAuthAllowed();
        Debug.Log("[Net] roster count=" + characters.Length);
    }

    void HandleZone(PacketEnvelope envelope)
    {
        switch (envelope.Opcode)
        {
            case NetOpcode.HelloAck:
                break;
            case NetOpcode.ZoneAuthResult:
            {
                var result = NetCodec.DecodeZoneAuthResult(envelope.Payload);
                if (!result.Ok)
                {
                    Debug.LogWarning($"[Net] zone auth failed {result.Code}: {result.Message}");
                    return;
                }

                _ = EnterWorldAsync();
                break;
            }
            case NetOpcode.EntitySpawn:
                ApplyEntitySpawn(NetCodec.DecodeEntitySpawn(envelope.Payload));
                break;
            case NetOpcode.EntityDespawn:
                ApplyEntityDespawn(NetCodec.DecodeEntityDespawn(envelope.Payload));
                break;
            case NetOpcode.PlayerStats:
                ApplyPlayerStats(NetCodec.DecodePlayerStats(envelope.Payload));
                break;
            case NetOpcode.ItemList:
                ApplyItemList(NetCodec.DecodeItemList(envelope.Payload));
                break;
            case NetOpcode.SkillList:
                ApplySkillList(NetCodec.DecodeSkillList(envelope.Payload));
                break;
            case NetOpcode.UserInfo:
                ApplyUserInfo(NetCodec.DecodeUserInfo(envelope.Payload));
                break;
            case NetOpcode.ShortCutInit:
                NetShortCutReceiver.ApplyInit(NetCodec.DecodeShortCutInit(envelope.Payload));
                break;
            case NetOpcode.ShortCutRegister:
                NetShortCutReceiver.ApplyRegister(NetCodec.DecodeShortCut(envelope.Payload));
                break;
            case NetOpcode.ShortCutDelete:
                NetShortCutReceiver.ApplyDelete(NetCodec.DecodeShortCutDel(envelope.Payload));
                break;
            case NetOpcode.EtcStatusUpdate:
                NetShortCutReceiver.ApplyEtcStatus(NetCodec.DecodeEtcStatusUpdate(envelope.Payload));
                break;
            case NetOpcode.MoveState:
                ApplyMoveState(NetCodec.DecodeMoveState(envelope.Payload));
                break;
            case NetOpcode.ChangeMoveType:
                NetShortCutReceiver.ApplyChangeMoveType(NetCodec.DecodeChangeMoveType(envelope.Payload));
                break;
            case NetOpcode.TargetSelected:
                ApplyTargetSelected(NetCodec.DecodeTargetSelected(envelope.Payload));
                break;
            case NetOpcode.AttackState:
                ApplyAttackState(NetCodec.DecodeAttackState(envelope.Payload));
                break;
            case NetOpcode.StatusUpdate:
                ApplyStatusUpdate(NetCodec.DecodeStatusUpdate(envelope.Payload));
                break;
            case NetOpcode.EntityDie:
                ApplyEntityDie(NetCodec.DecodeEntityDie(envelope.Payload));
                break;
            case NetOpcode.DropItem:
                ApplyDropItem(NetCodec.DecodeDropItem(envelope.Payload));
                break;
            case NetOpcode.InventoryUpdate:
                ApplyInventoryUpdate(NetCodec.DecodeInventoryUpdate(envelope.Payload));
                break;
            case NetOpcode.CreatureSay:
                ApplyCreatureSay(NetCodec.DecodeCreatureSay(envelope.Payload));
                break;
            case NetOpcode.SystemMessage:
                ApplySystemMessage(NetCodec.DecodeSystemMessage(envelope.Payload));
                break;
            case NetOpcode.MagicSkillUse:
                ApplyMagicSkillUse(NetCodec.DecodeMagicSkillUse(envelope.Payload));
                break;
            case NetOpcode.Revive:
                ApplyRevive(NetCodec.DecodeRevive(envelope.Payload));
                break;
            case NetOpcode.TeleportToLocation:
                ApplyTeleportToLocation(NetCodec.DecodeTeleportToLocation(envelope.Payload));
                break;
            case NetOpcode.AbnormalStatusUpdate:
                ApplyAbnormalStatusUpdate(NetCodec.DecodeAbnormalStatusUpdate(envelope.Payload));
                break;
            case NetOpcode.NpcHtml:
                ApplyNpcHtml(NetCodec.DecodeNpcHtml(envelope.Payload));
                break;
            case NetOpcode.BuyList:
                ApplyBuyList(NetCodec.DecodeBuyList(envelope.Payload));
                break;
            case NetOpcode.SellList:
                ApplySellList(NetCodec.DecodeSellList(envelope.Payload));
                break;
            case NetOpcode.MultiSellList:
                ApplyMultiSellList(NetCodec.DecodeMultiSellList(envelope.Payload));
                break;
            case NetOpcode.ShopBalance:
                ApplyShopBalance(NetCodec.DecodeShopBalance(envelope.Payload));
                break;
            case NetOpcode.WarehouseDepositList:
                ApplyWarehouseDepositList(NetCodec.DecodeWarehouseDepositList(envelope.Payload));
                break;
            case NetOpcode.WarehouseWithdrawList:
                ApplyWarehouseWithdrawList(NetCodec.DecodeWarehouseWithdrawList(envelope.Payload));
                break;
            case NetOpcode.QuestList:
                ApplyQuestList(NetCodec.DecodeQuestList(envelope.Payload));
                break;
            case NetOpcode.PartyAskJoin:
                ApplyPartyAskJoin(NetCodec.DecodePartyAskJoin(envelope.Payload));
                break;
            case NetOpcode.PartyJoinResult:
                ApplyPartyJoinResult(NetCodec.DecodePartyJoinResult(envelope.Payload));
                break;
            case NetOpcode.PartyMemberList:
                ApplyPartyMemberList(NetCodec.DecodePartyMemberList(envelope.Payload));
                break;
            case NetOpcode.TradeAsk:
                ApplyTradeAsk(NetCodec.DecodeTradeAsk(envelope.Payload));
                break;
            case NetOpcode.TradeRequestResult:
                ApplyTradeRequestResult(NetCodec.DecodeTradeRequestResult(envelope.Payload));
                break;
            case NetOpcode.TradeStart:
                ApplyTradeStart(NetCodec.DecodeTradeStart(envelope.Payload));
                break;
            case NetOpcode.TradeOfferUpdate:
                ApplyTradeOfferUpdate(NetCodec.DecodeTradeOfferUpdate(envelope.Payload));
                break;
            case NetOpcode.TradeConfirmState:
                ApplyTradeConfirmState(NetCodec.DecodeTradeConfirmState(envelope.Payload));
                break;
            case NetOpcode.TradeFinished:
                ApplyTradeFinished(NetCodec.DecodeTradeFinished(envelope.Payload));
                break;
            case NetOpcode.PositionCorrection:
                ApplyCorrection(NetCodec.DecodePositionCorrection(envelope.Payload));
                break;
            case NetOpcode.Reject:
            {
                var reject = NetCodec.DecodeReject(envelope.Payload);
                Debug.LogWarning($"[Net] zone reject {reject.Code}: {reject.Message}");
                break;
            }
        }
    }

    async System.Threading.Tasks.Task EnterWorldAsync()
    {
        if (_zone == null)
            return;
        await _zone.SendAsync(NetOpcode.EnterWorld, NetCodec.EncodeEnterWorld(new EnterWorldMsg()), NextSeq());
    }

    void ApplyEntitySpawn(EntitySpawnMsg spawn)
    {
        if (spawn.Kind == EntityKindNet.Npc)
        {
            ApplyNpcSpawn(spawn);
            return;
        }

        if (spawn.Kind != EntityKindNet.Player)
            return;

        if (!spawn.IsSelf)
        {
            ApplyRemotePlayerSpawn(spawn);
            return;
        }

        // Zone object id of the local player; stable only for this EnterWorld (see CharacterId).
        SessionObjectId = (int)spawn.EntityValue;

        var identity = new EntityIdentity
        {
            EntityType = EntityType.Player,
            Id = (int)spawn.EntityValue,
            Name = string.IsNullOrEmpty(spawn.Name) ? "Player" : spawn.Name,
            PlayerClass = spawn.ClassId,
            Owned = true,
            IsRunning = spawn.Running
        };
        identity.SetL2jPos(spawn.X, spawn.Y, spawn.Z);

        var status = new PlayerStatus();
        status.SetHp(100);
        status.SetMp(50);

        var stats = new PlayerStats
        {
            Level = spawn.Level > 0 ? spawn.Level : 1,
            MaxHp = spawn.MaxHp > 0 ? spawn.MaxHp : 100,
            MaxMp = 50,
            MaxCp = 0,
            BaseRunSpeed = Mathf.RoundToInt(spawn.RunSpeed > 0.001f ? spawn.RunSpeed : 120f),
            BaseWalkingSpeed = Mathf.RoundToInt(spawn.WalkSpeed > 0.001f ? spawn.WalkSpeed : 80f),
            RunRealSpeed = spawn.RunSpeed > 0.001f ? spawn.RunSpeed : 120f,
            WalkRealSpeed = spawn.WalkSpeed > 0.001f ? spawn.WalkSpeed : 80f
        };

        var appearance = new PlayerAppearance
        {
            Race = spawn.Race,
            Sex = spawn.Sex,
            Running = spawn.Running,
            // BaseClass is Fighter/Mystic (0/1), not L2 classId — ResolveRaceAnimation uses PlayerClass.
            BaseClass = 0
        };

        // Stash PlayerInfo then OnCharacterSelect → LoadSWMap.
        // Spawning happens in OnWorldSceneLoaded once the tile is ready.
        // Without PlayerInfo.Identity, LoadSWMap returns immediately and loading never ends.
        IZoneTransport transport = ZoneCommands.Transport;
        if (transport != null)
        {
            transport.PlayerInfo = new PlayerInfoInterlude
            {
                Identity = identity,
                Status = status,
                Stats = stats,
                Appearance = appearance
            };
        }
        else
            Debug.LogWarning("[Net] Zone transport missing on EntitySpawn — world load will stall");

        if (GameManager.Instance != null)
            GameManager.Instance.OnCharacterSelect();
        else
            Debug.LogWarning("[Net] GameManager missing on EntitySpawn — world load will stall");

        EnteredWorld?.Invoke();
    }

    void ApplyRemotePlayerSpawn(EntitySpawnMsg spawn)
    {
        CharInfoDto dto = new CharInfoDto();
        dto.Identity.Id = (int)spawn.EntityValue;
        dto.Identity.Name = string.IsNullOrEmpty(spawn.Name) ? "Unknown" : spawn.Name;
        dto.Identity.EntityType = EntityType.User;
        dto.Identity.SetL2jPos(spawn.X, spawn.Y, spawn.Z);
        dto.Identity.PlayerClass = spawn.ClassId;
        dto.Identity.IsRunning = spawn.Running;
        dto.Appearance.Race = spawn.Race;
        dto.Appearance.Sex = spawn.Sex;
        dto.Appearance.BaseClass = spawn.ClassId;
        dto.Appearance.Running = spawn.Running;
        float run = spawn.RunSpeed > 0.001f ? spawn.RunSpeed : 120f;
        float walk = spawn.WalkSpeed > 0.001f ? spawn.WalkSpeed : 80f;
        dto.Stats.BaseRunSpeed = Mathf.RoundToInt(run);
        dto.Stats.BaseWalkingSpeed = Mathf.RoundToInt(walk);
        dto.Stats.RunRealSpeed = run;
        dto.Stats.WalkRealSpeed = walk;
        dto.Stats.Level = spawn.Level > 0 ? spawn.Level : 1;
        dto.Status.SetHp(100);
        dto.Status.SetMp(100);
        dto.Stats.MaxHp = spawn.MaxHp > 0 ? spawn.MaxHp : 100;
        dto.Stats.MaxMp = 100;
        if (_presence != null) _presence.ApplyCharInfo(dto);
        else CharInfoApplyIncomingPacketServices.ApplyCharInfo(dto);
    }

    void ApplyNpcSpawn(EntitySpawnMsg spawn)
    {
        int objectId = (int)spawn.EntityValue;
        NpcInfoDto dto = new NpcInfoDto();
        dto.Identity.Id = objectId;
        dto.Identity.NpcId = spawn.TemplateId;
        if (!string.IsNullOrEmpty(spawn.Name))
            dto.Identity.Name = spawn.Name;
        dto.Identity.SetHideHp(spawn.TemplateId);
        dto.Identity.EntityType = EntityType.NPC;
        dto.Identity.SetL2jPos(spawn.X, spawn.Y, spawn.Z);
        dto.Identity.IsRunning = spawn.Running;
        dto.Stats.Level = spawn.Level > 0 ? spawn.Level : 1;
        dto.Status.SetHp(100);
        dto.Stats.MaxHp = spawn.MaxHp > 0 ? spawn.MaxHp : 100;
        const int defaultPAtkSpd = 253;
        dto.Stats.PAtkSpd = defaultPAtkSpd;
        dto.Stats.PAtkRealSpeed = defaultPAtkSpd;
        float run = spawn.RunSpeed > 0.001f ? spawn.RunSpeed : 120f;
        float walk = spawn.WalkSpeed > 0.001f ? spawn.WalkSpeed : 80f;
        dto.Stats.BaseRunSpeed = Mathf.RoundToInt(run);
        dto.Stats.BaseWalkingSpeed = Mathf.RoundToInt(walk);
        dto.Stats.WalkRealSpeed = walk;
        dto.Stats.RunRealSpeed = run;
        dto.Appearance.CollisionRadius = 10f;
        dto.Appearance.CollisionHeight = 22f;
        NetNpcSession.RememberNpc(objectId, spawn.Name, spawn.Attackable);
        if (_presence != null) _presence.ApplyNpcInfo(dto);
        else NpcInfoApplyIncomingPacketServices.ApplyNpcInfo(dto);
    }

    void ApplyEntityDespawn(EntityDespawnMsg despawn)
    {
        int objectId = (int)despawn.EntityValue;
        NetNpcSession.Forget(objectId);
        var dto = new DeleteObjectDto { ObjectId = objectId };
        DeleteObjectGroundApplyService deleteObjectGround = _deleteObjectGround;
        if (deleteObjectGround != null) deleteObjectGround.Apply(dto);
        else DeleteObjectGroundApplyIncomingPacketServices.ApplyDeleteObject(dto);
    }

    void ApplyTargetSelected(TargetSelectedNetMsg msg)
    {
        MyTargetSelectDto dto = MyTargetSelectDto.FromNet(msg.TargetObjectId, msg.ColorHint);
        if (_combatTarget != null) _combatTarget.ApplyMyTargetSelected(dto);
        else MyTargetSelectedApplyIncomingPacketServices.ApplyMyTargetSelected(dto);
    }

    void ApplyAttackState(AttackStateMsg attack)
    {
        var dto = new AttackDto();
        dto.InitFromNet(
            (int)attack.AttackerValue,
            (int)attack.TargetValue,
            attack.Damage,
            attack.Flags,
            attack.AttackerX,
            attack.AttackerY,
            attack.AttackerZ,
            attack.TargetX,
            attack.TargetY,
            attack.TargetZ);
        if (_combatAttack != null) _combatAttack.Apply(dto);
        else AttackApplyIncomingPacketServices.ApplyAttack(dto);
    }

    void ApplyStatusUpdate(StatusUpdateNetMsg status)
    {
        var attributes = new System.Collections.Generic.List<StatusUpdate.Attribute>();
        int n = status.AttributeIds == null || status.Values == null
            ? 0
            : Mathf.Min(status.AttributeIds.Length, status.Values.Length);
        for (int i = 0; i < n; i++)
            attributes.Add(new StatusUpdate.Attribute(status.AttributeIds[i], status.Values[i]));
        if (_statusUpdate != null) _statusUpdate.Apply((int)status.EntityValue, attributes);
        else StatusUpdateApplyIncomingPacketServices.ApplyStatusUpdate((int)status.EntityValue, attributes);
    }

    void ApplyEntityDie(EntityDieMsg die)
    {
        DieDto dieDto = DieDto.FromZoneWire((int)die.EntityValue);
        if (_combatDie != null) _combatDie.Apply(dieDto);
        else DieApplyIncomingPacketServices.ApplyDie(dieDto);
    }
    void ApplyDropItem(DropItemNetMsg drop)
    {
        DropItemDto dto = DropItemDto.FromNet(
            drop.DropperObjectId,
            (int)drop.EntityValue,
            drop.ItemId,
            drop.X,
            drop.Y,
            drop.Z,
            drop.Stackable,
            drop.Count,
            0);
        if (_presence != null) _presence.ApplyDropItem(dto);
        else DropItemApplyIncomingPacketServices.ApplyDropItem(dto);
    }

    void ApplyInventoryUpdate(InventoryUpdateNetMsg update)
    {
        InventoryUpdateEntryNet[] src = update.Changes ?? System.Array.Empty<InventoryUpdateEntryNet>();
        var rows = new NetInventoryChange[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            InventoryUpdateEntryNet row = src[i];
            rows[i] = new NetInventoryChange
            {
                Change = row.Change,
                ObjectId = row.ObjectId,
                ItemId = row.ItemId,
                Count = row.Count,
                EquippedSlot = row.EquippedSlot,
                EnchantLevel = row.EnchantLevel,
                BodySlot = "",
                Kind = "",
                Icon = "",
                WeaponType = "",
                ArmorType = ""
            };
        }

        NetGearSession.NoteInventoryUpdate(rows);
    }

    void ApplyCreatureSay(CreatureSayNetMsg say)
    {
        NetChatFeed.NoteSay(say.Name, say.Text, say.ChatType);
    }

    void ApplySystemMessage(SystemMessageNetMsg message)
    {
        string shown = NetSystemLines.Resolve(message.Id, message.Text, message.Params);
        NetChatFeed.NoteSystem(shown);
    }

    void ApplyMagicSkillUse(MagicSkillUseNetMsg use)
    {
        var mapped = new NetMagicSkillUse
        {
            CasterObjectId = use.CasterObjectId,
            TargetObjectId = use.TargetObjectId,
            SkillId = use.SkillId,
            SkillLevel = use.SkillLevel,
            HitTimeMs = use.HitTimeMs,
            ReuseDelayMs = use.ReuseDelayMs
        };
        MagicSkillUseDto dto = NetDomainMap.ToMagicSkillUseDto(mapped);
        if (_skills != null) _skills.ApplyMagicSkillUse(dto);
        else MagicSkillUseApplyIncomingPacketServices.ApplyMagicSkillUse(dto);
    }

    void ApplyRevive(ReviveNetMsg revive)
    {
        ReviveDto dto = NetDomainMap.ToReviveDto(new NetRevive { ObjectId = revive.ObjectId });
        if (_presence != null) _presence.ApplyRevive(dto);
        else ReviveApplyIncomingPacketServices.ApplyRevive(dto);
    }

    void ApplyTeleportToLocation(TeleportToLocationNetMsg teleport)
    {
        TeleportToLocationDto dto = NetDomainMap.ToTeleportToLocationDto(new NetTeleportToLocation
        {
            ObjectId = teleport.ObjectId,
            X = teleport.X,
            Y = teleport.Y,
            Z = teleport.Z,
            Heading = teleport.Heading
        });
        if (_presence != null) _presence.ApplyTeleportToLocation(dto);
        else TeleportToLocationApplyIncomingPacketServices.ApplyTeleportToLocation(dto);
    }

    void ApplyAbnormalStatusUpdate(AbnormalStatusUpdateNetMsg update)
    {
        AbnormalEffectNet[] src = update.Effects ?? System.Array.Empty<AbnormalEffectNet>();
        var effects = new NetAbnormalEffect[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            AbnormalEffectNet e = src[i] ?? new AbnormalEffectNet();
            effects[i] = new NetAbnormalEffect
            {
                Id = e.Id,
                Value = e.Value,
                DurationSeconds = e.DurationSeconds
            };
        }

        AbnormalStatusUpdateDto dto = NetDomainMap.ToAbnormalStatusUpdateDto(new NetAbnormalStatusUpdate
        {
            ObjectId = update.ObjectId,
            Effects = effects
        });
        if (_status != null) _status.ApplyAbnormalStatusUpdate(dto);
        else AbnormalStatusUpdateApplyIncomingPacketServices.ApplyAbnormalStatusUpdate(dto);
    }

    void ApplyNpcHtml(NpcHtmlNetMsg html)
    {
        HtmlWindow window = HtmlWindow.Instance;
        if (window == null)
            return;
        window.ShowNpcDialog(html.NpcObjectId, html.NpcId, html.Html ?? "");
    }

    void ApplyBuyList(BuyListNetMsg list)
    {
        BuyListItemNet[] src = list.Items ?? System.Array.Empty<BuyListItemNet>();
        var items = new NetBuyItem[src.Length];
        for (int i = 0; i < src.Length; i++)
            items[i] = new NetBuyItem { ItemId = src[i].ItemId, Price = src[i].Price, Count = src[i].Count };
        NetNpcWindows.ApplyBuyList(new NetBuyList
        {
            NpcObjectId = list.NpcObjectId,
            ListId = list.ListId,
            BaseTaxPercent = list.BaseTaxPercent,
            CastleTaxPercent = list.CastleTaxPercent,
            Items = items
        });
    }

    void ApplySellList(SellListNetMsg list)
    {
        SellListItemNet[] src = list.Items ?? System.Array.Empty<SellListItemNet>();
        var items = new NetSellItem[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            items[i] = new NetSellItem
            {
                ObjectId = src[i].ObjectId,
                ItemId = src[i].ItemId,
                Count = src[i].Count,
                Price = src[i].Price,
                EnchantLevel = src[i].EnchantLevel
            };
        }

        NetNpcWindows.ApplySellList(new NetSellList { NpcObjectId = list.NpcObjectId, Items = items });
    }

    void ApplyMultiSellList(MultiSellListNetMsg list)
    {
        MultiSellEntryNet[] src = list.Entries ?? System.Array.Empty<MultiSellEntryNet>();
        var entries = new NetMultiSellEntry[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            entries[i] = new NetMultiSellEntry
            {
                EntryId = src[i].EntryId,
                Ingredients = MapMultiParts(src[i].Ingredients),
                Products = MapMultiParts(src[i].Products)
            };
        }

        NetNpcWindows.ApplyMultiSellList(new NetMultiSellList
        {
            NpcObjectId = list.NpcObjectId,
            ListId = list.ListId,
            MaintainEnchantment = list.MaintainEnchantment,
            InventoryOnly = list.InventoryOnly,
            CastleTaxPercent = list.CastleTaxPercent,
            Entries = entries
        });
    }

    static NetMultiPart[] MapMultiParts(MultiSellPartNet[] parts)
    {
        if (parts == null || parts.Length == 0)
            return System.Array.Empty<NetMultiPart>();
        var rows = new NetMultiPart[parts.Length];
        for (int i = 0; i < parts.Length; i++)
            rows[i] = new NetMultiPart
            {
                ItemId = parts[i].ItemId,
                Count = parts[i].Count,
                EnchantLevel = parts[i].EnchantLevel
            };
        return rows;
    }

    void ApplyShopBalance(ShopBalanceNetMsg balance)
    {
        NetNpcWindows.ApplyShopBalance(new NetShopBalance
        {
            Adena = balance.Adena,
            Weight = balance.Weight,
            MaxWeight = balance.MaxWeight,
            Slots = balance.Slots,
            MaxSlots = balance.MaxSlots,
            BaseTaxPercent = balance.BaseTaxPercent,
            CastleTaxPercent = balance.CastleTaxPercent
        });
    }

    void ApplyWarehouseDepositList(WarehouseDepositListNetMsg list)
    {
        NetNpcWindows.ApplyWarehouseDepositList(new NetWarehouseDepositList
        {
            NpcObjectId = list.NpcObjectId,
            FeePerLine = list.FeePerLine,
            SlotsUsed = list.SlotsUsed,
            SlotsMax = list.SlotsMax,
            Adena = list.Adena,
            Items = MapWarehouseItems(list.Items)
        });
    }

    void ApplyWarehouseWithdrawList(WarehouseWithdrawListNetMsg list)
    {
        NetNpcWindows.ApplyWarehouseWithdrawList(new NetWarehouseWithdrawList
        {
            NpcObjectId = list.NpcObjectId,
            SlotsUsed = list.SlotsUsed,
            SlotsMax = list.SlotsMax,
            InventorySlotsUsed = list.InventorySlotsUsed,
            InventorySlotsMax = list.InventorySlotsMax,
            CurrentWeight = list.CurrentWeight,
            MaxWeight = list.MaxWeight,
            Items = MapWarehouseItems(list.Items)
        });
    }

    void ApplyQuestList(QuestListNetMsg list)
    {
        QuestJournalEntryNet[] active = list.Active ?? System.Array.Empty<QuestJournalEntryNet>();
        var rows = new NetQuestJournal[active.Length];
        for (int i = 0; i < active.Length; i++)
        {
            QuestJournalEntryNet row = active[i] ?? new QuestJournalEntryNet();
            rows[i] = new NetQuestJournal
            {
                QuestId = row.QuestId,
                Name = row.Name ?? "",
                State = row.State,
                Cond = row.Cond,
                ProgressFlags = row.ProgressFlags,
                JournalWord = row.JournalWord,
                Repeatable = row.Repeatable
            };
        }

        NetNpcWindows.ApplyQuestList(new NetQuestList
        {
            Active = rows,
            CompletedMask = list.CompletedMask ?? System.Array.Empty<byte>(),
            CompletedQuestIds = list.CompletedQuestIds ?? System.Array.Empty<int>(),
            MaxActive = list.MaxActive
        });
    }

    void ApplyPartyAskJoin(PartyAskJoinNetMsg ask)
    {
        var dto = NetDomainMap.ToAskJoinPartyDto(new NetPartyAskJoin
        {
            RequestorObjectId = ask.RequestorObjectId,
            RequestorName = ask.RequestorName ?? "",
            ItemDistribution = ask.ItemDistribution
        });
        if (_party != null) _party.ApplyAskJoinParty(dto);
        else AskJoinPartyApplyIncomingPacketServices.ApplyAskJoinParty(dto);
    }

    void ApplyPartyJoinResult(PartyJoinResultNetMsg result)
    {
        if (_party != null) _party.ApplyPartyJoinResult(result.Response);
        else PartyJoinResultApplyIncomingPacketServices.ApplyPartyJoinResult(result.Response);
    }

    void ApplyPartyMemberList(PartyMemberListNetMsg list)
    {
        PartyMemberEntryNet[] members = list.Members ?? System.Array.Empty<PartyMemberEntryNet>();
        var rows = new NetPartyMemberEntry[members.Length];
        for (int i = 0; i < members.Length; i++)
        {
            PartyMemberEntryNet row = members[i] ?? new PartyMemberEntryNet();
            rows[i] = new NetPartyMemberEntry
            {
                ObjectId = row.ObjectId,
                Name = row.Name ?? "",
                ClassId = row.ClassId,
                Level = row.Level,
                CurHp = row.CurHp,
                MaxHp = row.MaxHp,
                CurMp = row.CurMp,
                MaxMp = row.MaxMp
            };
        }

        var roster = new NetPartyMemberList
        {
            LeaderObjectId = list.LeaderObjectId,
            PartyId = list.PartyId,
            Members = rows
        };
        if (_party != null) _party.ApplyPartyMemberList(roster);
        else PartyMemberListApplyIncomingPacketServices.ApplyPartyMemberList(roster);
    }

    void ApplyTradeAsk(TradeAskNetMsg ask)
    {
        SendTradeRequestDto dto = NetDomainMap.ToSendTradeRequestDto(new NetTradeAsk
        {
            RequestorObjectId = ask.RequestorObjectId,
            RequestorName = ask.RequestorName ?? ""
        });
        if (_tradeRequest != null) _tradeRequest.ApplySendTradeRequest(dto);
        else SendTradeRequestApplyIncomingPacketServices.ApplySendTradeRequest(dto);
    }

    void ApplyTradeRequestResult(TradeRequestResultNetMsg result)
    {
        if (result.Response == 0)
            NetChatFeed.NoteSystem("Trade request declined.");
    }

    void ApplyTradeStart(TradeStartNetMsg start)
    {
        TradeStartDto dto = NetDomainMap.ToTradeStartDto(new NetTradeStart
        {
            PartnerObjectId = start.PartnerObjectId,
            PartnerName = start.PartnerName ?? ""
        });
        if (_trade != null) _trade.ApplyStart(dto);
        else TradeStartApplyIncomingPacketServices.ApplyTradeStart(dto);
    }

    void ApplyTradeOfferUpdate(TradeOfferUpdateNetMsg update)
    {
        ItemInstance item = NetDomainMap.ToTradeOfferItem(new NetTradeOfferUpdate
        {
            IsOwn = update.IsOwn,
            InventoryObjectId = update.InventoryObjectId,
            ItemId = update.ItemId,
            Count = (int)update.Count,
            EnchantLevel = update.EnchantLevel
        });
        if (update.IsOwn != 0)
            if (_trade != null) _trade.ApplyOwnAdd(item);
            else TradeOwnAddApplyIncomingPacketServices.ApplyOwnAdd(item);
        else
            if (_trade != null) _trade.ApplyOtherAdd(item);
            else TradeOtherAddApplyIncomingPacketServices.ApplyOtherAdd(item);
    }

    void ApplyTradeConfirmState(TradeConfirmStateNetMsg state)
    {
        if (_trade != null) _trade.ApplyPressOwnOk(state.SelfConfirmed != 0);
        else TradePressOwnOkApplyIncomingPacketServices.ApplyPressOwnOk(state.SelfConfirmed != 0);
        if (_trade != null) _trade.ApplyPressOtherOk(state.PartnerConfirmed != 0);
        else TradePressOtherOkApplyIncomingPacketServices.ApplyPressOtherOk(state.PartnerConfirmed != 0);
    }

    void ApplyTradeFinished(TradeFinishedNetMsg _)
    {
        if (_trade != null) _trade.ApplyDone();
        else TradeDoneApplyIncomingPacketServices.ApplyTradeDone();
    }

    static NetItemEntry[] MapWarehouseItems(ItemListEntryNet[] items)
    {
        if (items == null || items.Length == 0)
            return System.Array.Empty<NetItemEntry>();
        var rows = new NetItemEntry[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            rows[i] = new NetItemEntry
            {
                ObjectId = items[i].ObjectId,
                ItemId = items[i].ItemId,
                Count = items[i].Count,
                EquippedSlot = items[i].EquippedSlot,
                EnchantLevel = items[i].EnchantLevel,
                BodySlot = "none",
                Kind = "etc",
                Icon = "",
                WeaponType = "",
                ArmorType = ""
            };
        }

        return rows;
    }

    void ApplyPlayerStats(PlayerStatsMsg msg)
    {
        // Prefer live entity; if world tile not loaded yet, patch stashed PlayerInfo for spawn.
        PlayerEntity local = DomainPacketServices.LocalPlayer;
        PlayerStats stats = local != null ? local.Stats as PlayerStats : null;
        PlayerStatus status = local != null ? local.Status as PlayerStatus : null;

        IZoneTransport transport = ZoneCommands.Transport;
        if (stats == null && transport != null && transport.PlayerInfo.Stats != null)
            stats = transport.PlayerInfo.Stats;
        if (status == null && transport != null && transport.PlayerInfo.Status != null)
            status = transport.PlayerInfo.Status;

        if (stats == null)
            return;

        stats.Level = msg.Level;
        stats.Exp = msg.Exp;
        stats.Sp = msg.Sp;
        stats.MaxHp = msg.MaxHp;
        stats.MaxMp = msg.MaxMp;
        stats.MaxCp = msg.MaxCp;
        stats.Cp = msg.Cp;
        if (status != null)
        {
            status.SetHp(msg.Hp);
            status.SetMp(msg.Mp);
            status.Cp = msg.Cp;
        }
    }

    void ApplyUserInfo(UserInfoNetMsg msg)
    {
        // The zone server sends UserInfo right after the self EntitySpawn, while the
        // world scene is still loading. NoteUserInfo stashes it and overlays the live
        // player / stashed PlayerInfo as those appear, so the paperdoll and stats are
        // not lost to the load order.
        NetUserInfo user = NetDomainMap.ToUserInfo(msg);
        NetGearSession.NoteUserInfo(user);
    }

    void ApplyItemList(ItemListNetMsg list)
    {
        NetItemEntry[] rows = MapItemList(list);
        NetGearSession.NoteItemList(rows);
        Debug.Log("[Net] ItemList count=" + rows.Length);
    }

    void ApplySkillList(SkillListNetMsg list)
    {
        NetSkillEntry[] rows = MapSkillList(list);
        SkillListDto dto = NetDomainMap.ToSkillListDto(rows);
        if (_skills != null) _skills.ApplySkillList(dto);
        else SkillListApplyIncomingPacketServices.ApplySkillList(dto);
        Debug.Log("[Net] SkillList count=" + rows.Length);
    }

    static NetItemEntry[] MapItemList(ItemListNetMsg list)
    {
        if (list?.Items == null || list.Items.Length == 0)
            return System.Array.Empty<NetItemEntry>();
        var rows = new NetItemEntry[list.Items.Length];
        for (int i = 0; i < list.Items.Length; i++)
        {
            ItemListEntryNet src = list.Items[i];
            rows[i] = new NetItemEntry
            {
                ObjectId = src.ObjectId,
                ItemId = src.ItemId,
                Count = src.Count,
                EquippedSlot = src.EquippedSlot,
                EnchantLevel = src.EnchantLevel,
                BodySlot = src.BodySlot ?? "",
                Kind = src.Kind ?? "",
                Icon = src.Icon ?? "",
                WeaponType = src.WeaponType ?? "",
                ArmorType = src.ArmorType ?? ""
            };
        }

        return rows;
    }

    static NetSkillEntry[] MapSkillList(SkillListNetMsg list)
    {
        if (list?.Skills == null || list.Skills.Length == 0)
            return System.Array.Empty<NetSkillEntry>();
        var rows = new NetSkillEntry[list.Skills.Length];
        for (int i = 0; i < list.Skills.Length; i++)
        {
            SkillListEntryNet src = list.Skills[i];
            rows[i] = new NetSkillEntry
            {
                SkillId = src.SkillId,
                Level = src.Level,
                ReuseDelayMs = src.ReuseRemainingMs
            };
        }

        return rows;
    }

    void ApplyMoveState(MoveStateMsg move)
    {
        int objectId = (int)move.EntityValue;
        Vector3 dest = VectorUtils.ConvertPosToUnity(new Vector3(move.DestX, move.DestY, move.DestZ));
        Vector3 origin = VectorUtils.ConvertPosToUnity(new Vector3(move.OriginX, move.OriginY, move.OriginZ));
        var domain = new DomainMoveTo
        {
            ObjectId = objectId,
            Destination = dest,
            Origin = origin,
            // The intention layer (NewMoveToIntention) reads the Interlude DTO and bails out when it is
            // null, so the local player stayed put even though MoveState arrived. Synthesize it here.
            InterludeDto = CharMoveToLocationDto.FromUnity(objectId, dest, origin)
        };
        if (_sink != null)
            _sink.ApplyMoveTo(domain);
        else if (WorldApplyPacketServices.WorldApply != null)
            WorldApplyPacketServices.WorldApply.MoveTo(objectId, dest, origin, domain.InterludeDto);
    }

    void ApplyCorrection(PositionCorrectionMsg corr)
    {
        int objectId = (int)corr.EntityValue;
        Vector3 pos = VectorUtils.ConvertPosToUnity(new Vector3(corr.X, corr.Y, corr.Z));
        var domain = new DomainMoveTo
        {
            ObjectId = objectId,
            Destination = pos,
            Origin = pos,
            InterludeDto = CharMoveToLocationDto.FromUnity(objectId, pos, pos)
        };
        if (_sink != null)
            _sink.ApplyMoveTo(domain);
        else if (WorldApplyPacketServices.WorldApply != null)
            WorldApplyPacketServices.WorldApply.MoveTo(objectId, pos, pos, domain.InterludeDto);
    }

    uint NextSeq() => _seq++;

    void Post(Action action) => _main.Enqueue(action);

    static async System.Threading.Tasks.Task WaitUntil(Func<bool> pred, float seconds)
    {
        float start = Time.realtimeSinceStartup;
        while (!pred())
        {
            if (Time.realtimeSinceStartup - start > seconds)
                throw new TimeoutException("net wait");
            await System.Threading.Tasks.Task.Delay(20);
        }
    }

    void OnDestroy()
    {
        _lobby?.Dispose();
        _zone?.Dispose();
        if (Instance == this)
            Instance = null;
    }
}
