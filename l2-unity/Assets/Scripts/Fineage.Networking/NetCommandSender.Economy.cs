using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;

public sealed partial class NetCommandSender
{
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

}
