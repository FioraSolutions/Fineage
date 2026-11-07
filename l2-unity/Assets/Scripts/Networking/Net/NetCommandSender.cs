using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;

/// <summary>
/// Outbound zone command sends. NetSession still owns the <see cref="NetTransport"/> and
/// request sequence; this type holds the encode/send bodies so the session does not absorb
/// every <c>Send*</c> implementation.
/// </summary>
public sealed class NetCommandSender
{
    readonly Func<NetTransport> _zone;
    readonly Func<uint> _nextSeq;

    public NetCommandSender(Func<NetTransport> zone, Func<uint> nextSeq)
    {
        _zone = zone ?? throw new ArgumentNullException(nameof(zone));
        _nextSeq = nextSeq ?? throw new ArgumentNullException(nameof(nextSeq));
    }

    public Task SendMoveAsync(int destX, int destY, int destZ)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.MoveInput,
            NetCodec.EncodeMoveInput(new MoveInputMsg { DestX = destX, DestY = destY, DestZ = destZ }),
            _nextSeq());
    }

    public Task SendSelectTargetAsync(int targetObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.SelectTarget,
            NetCodec.EncodeSelectTarget(new SelectTargetNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendAttackRequestAsync(int targetObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.AttackRequest,
            NetCodec.EncodeAttackRequest(new AttackRequestNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendUseSkillAsync(int skillId, int targetObjectId, bool ctrlPressed)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.UseSkill,
            NetCodec.EncodeUseSkill(new UseSkillNetMsg
            {
                SkillId = skillId,
                TargetObjectId = targetObjectId,
                CtrlPressed = ctrlPressed
            }),
            _nextSeq());
    }

    public Task SendPickupRequestAsync(int groundObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.PickupRequest,
            NetCodec.EncodePickupRequest(new PickupRequestNetMsg { GroundObjectId = groundObjectId }),
            _nextSeq());
    }

    public Task SendDropItemRequestAsync(int objectId, int count, int x, int y, int z)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.DropItemRequest,
            NetCodec.EncodeDropItemRequest(new DropItemRequestNetMsg
            {
                ObjectId = objectId,
                Count = count,
                X = x,
                Y = y,
                Z = z
            }),
            _nextSeq());
    }

    public Task SendUseItemAsync(int objectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.UseItem,
            NetCodec.EncodeUseItem(new UseItemNetMsg { ObjectId = objectId }),
            _nextSeq());
    }

    public Task SendUnequipItemAsync(int paperdollSlot)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.UnequipItem,
            NetCodec.EncodeUnequipItem(new UnequipItemNetMsg { PaperdollSlot = paperdollSlot }),
            _nextSeq());
    }

    public Task SendRequestBuyItemAsync(int npcObjectId, int listId, NetIdCount[] items)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestBuyItem,
            NetCodec.EncodeRequestBuyItem(new RequestBuyItemNetMsg
            {
                NpcObjectId = npcObjectId,
                ListId = listId,
                Items = MapShopCounts(items)
            }),
            _nextSeq());
    }

    public Task SendRequestSellItemAsync(int npcObjectId, NetIdCount[] items)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestSellItem,
            NetCodec.EncodeRequestSellItem(new RequestSellItemNetMsg
            {
                NpcObjectId = npcObjectId,
                Items = MapShopObjectCounts(items)
            }),
            _nextSeq());
    }

    public Task SendWarehouseDepositAsync(int npcObjectId, NetIdCount[] lines)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.WarehouseDepositRequest,
            NetCodec.EncodeWarehouseDepositRequest(new WarehouseDepositRequestNetMsg
            {
                NpcObjectId = npcObjectId,
                Lines = MapShopObjectCounts(lines)
            }),
            _nextSeq());
    }

    public Task SendWarehouseWithdrawAsync(int npcObjectId, NetIdCount[] lines)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.WarehouseWithdrawRequest,
            NetCodec.EncodeWarehouseWithdrawRequest(new WarehouseWithdrawRequestNetMsg
            {
                NpcObjectId = npcObjectId,
                Lines = MapShopObjectCounts(lines)
            }),
            _nextSeq());
    }

    public Task SendRequestQuestListAsync()
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(NetOpcode.RequestQuestList, NetCodec.EncodeRequestQuestList(), _nextSeq());
    }

    public Task SendRequestQuestAbortAsync(int questId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.RequestQuestAbort,
            NetCodec.EncodeRequestQuestAbort(new RequestQuestAbortNetMsg { QuestId = questId }),
            _nextSeq());
    }

    public Task SendPartyInviteRequestAsync(int targetObjectId, int itemDistribution)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.PartyInviteRequest,
            NetCodec.EncodePartyInviteRequest(new PartyInviteRequestNetMsg
            {
                TargetObjectId = targetObjectId,
                ItemDistribution = itemDistribution
            }),
            _nextSeq());
    }

    public Task SendPartyAnswerJoinRequestAsync(int requestorObjectId, int response)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.PartyAnswerJoinRequest,
            NetCodec.EncodePartyAnswerJoinRequest(new PartyAnswerJoinRequestNetMsg
            {
                RequestorObjectId = requestorObjectId,
                Response = response
            }),
            _nextSeq());
    }

    public Task SendPartyLeaveRequestAsync()
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(NetOpcode.PartyLeaveRequest, NetCodec.EncodePartyLeaveRequest(), _nextSeq());
    }

    public Task SendPartyKickRequestAsync(int targetObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.PartyKickRequest,
            NetCodec.EncodePartyKickRequest(new PartyKickRequestNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendTradeRequestAsync(int targetObjectId)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.TradeRequest,
            NetCodec.EncodeTradeRequest(new TradeRequestNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendTradeAnswerRequestAsync(int requestorObjectId, int response)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.TradeAnswerRequest,
            NetCodec.EncodeTradeAnswerRequest(new TradeAnswerRequestNetMsg
            {
                RequestorObjectId = requestorObjectId,
                Response = response
            }),
            _nextSeq());
    }

    public Task SendTradeAddItemRequestAsync(int inventoryObjectId, int count)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.TradeAddItemRequest,
            NetCodec.EncodeTradeAddItemRequest(new TradeAddItemRequestNetMsg
            {
                InventoryObjectId = inventoryObjectId,
                Count = count
            }),
            _nextSeq());
    }

    public Task SendTradeDoneRequestAsync(int response)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.TradeDoneRequest,
            NetCodec.EncodeTradeDoneRequest(new TradeDoneRequestNetMsg { Response = response }),
            _nextSeq());
    }

    public Task SendClanCreateRequestAsync(string name)
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.ClanCreateRequest,
            NetCodec.EncodeClanCreateRequest(new ClanCreateRequestNetMsg { Name = name ?? "" }),
            _nextSeq());
    }

    public Task SendClanInviteRequestAsync(int targetObjectId)
    {
        if (targetObjectId <= 0 || !TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.ClanInviteRequest,
            NetCodec.EncodeClanInviteRequest(new ClanInviteRequestNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendClanAnswerJoinRequestAsync(int requestorObjectId, int response)
    {
        if (requestorObjectId <= 0 || !TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.ClanAnswerJoinRequest,
            NetCodec.EncodeClanAnswer(new ClanAnswerNetMsg { RequestorObjectId = requestorObjectId, Response = response }),
            _nextSeq());
    }

    public Task SendClanLeaveRequestAsync()
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(NetOpcode.ClanLeaveRequest, Array.Empty<byte>(), _nextSeq());
    }

    public Task SendClanKickRequestAsync(int targetObjectId)
    {
        if (targetObjectId <= 0 || !TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.ClanKickRequest,
            NetCodec.EncodeClanKickRequest(new ClanKickRequestNetMsg { TargetObjectId = targetObjectId }),
            _nextSeq());
    }

    public Task SendClanDissolveRequestAsync()
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(NetOpcode.ClanDissolveRequest, Array.Empty<byte>(), _nextSeq());
    }

    public Task SendClanCancelDissolveRequestAsync()
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(NetOpcode.ClanCancelDissolveRequest, Array.Empty<byte>(), _nextSeq());
    }

    public Task SendTargetCancelAsync()
    {
        if (!TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(NetOpcode.TargetCancelRequest, Array.Empty<byte>(), _nextSeq());
    }

    static ShopCountNet[] MapShopCounts(NetIdCount[] items)
    {
        if (items == null || items.Length == 0)
            return Array.Empty<ShopCountNet>();
        var rows = new ShopCountNet[items.Length];
        for (int i = 0; i < items.Length; i++)
            rows[i] = new ShopCountNet { ItemId = items[i].Id, Count = items[i].Count };
        return rows;
    }

    static ShopObjectCountNet[] MapShopObjectCounts(NetIdCount[] items)
    {
        if (items == null || items.Length == 0)
            return Array.Empty<ShopObjectCountNet>();
        var rows = new ShopObjectCountNet[items.Length];
        for (int i = 0; i < items.Length; i++)
            rows[i] = new ShopObjectCountNet { ObjectId = items[i].Id, Count = items[i].Count };
        return rows;
    }

    bool TryGetZone(out NetTransport zone)
    {
        zone = _zone();
        return zone != null && zone.Connected;
    }
}
