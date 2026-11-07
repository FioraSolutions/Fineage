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
