using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;

public sealed partial class NetCommandSender
{
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

    public Task SendDestroyItemRequestAsync(int objectId, int count)
    {
        if (objectId <= 0 || !TryGetZone(out NetTransport zone))
            return Task.CompletedTask;
        return zone.SendAsync(
            NetOpcode.DestroyItemRequest,
            NetCodec.EncodeDestroyItemRequest(new DestroyItemRequestMsg { ObjectId = objectId, Count = count }),
            _nextSeq());
    }

    public Task SendRequestItemListAsync()
    {
        if (!TryGetZone(out NetTransport zone))
        {
            UnityEngine.Debug.LogError("[Inventory] RequestItemList was not sent; zone transport is down");
            return Task.CompletedTask;
        }
        // Interlude client → server RequestItemList. Opcode 0x0F, no body.
        return zone.SendAsync((ushort)GameClientPacketType.RequestItemList, System.Array.Empty<byte>(), _nextSeq());
    }
}
