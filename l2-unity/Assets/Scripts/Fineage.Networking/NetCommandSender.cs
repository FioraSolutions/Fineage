using System;
using System.Threading.Tasks;
using Fineage.Net.Contract;

public sealed partial class NetCommandSender
{
    readonly Func<NetTransport> _zone;
    readonly Func<uint> _nextSeq;

    public NetCommandSender(Func<NetTransport> zone, Func<uint> nextSeq)
    {
        _zone = zone ?? throw new ArgumentNullException(nameof(zone));
        _nextSeq = nextSeq ?? throw new ArgumentNullException(nameof(nextSeq));
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
