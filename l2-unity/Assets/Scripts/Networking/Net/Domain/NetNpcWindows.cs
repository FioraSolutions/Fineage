using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies NPC HTML, quest, shop, and service packets onto the existing windows.
/// </summary>
public static partial class NetNpcWindows
{
    public static void Tick()
    {
        if (!NetNpcSession.WindowsOpen)
            return;
        if (PollInput.EscapeDown)
        {
            Close();
            return;
        }

        int npcId = NetNpcSession.ActiveNpcObjectId;
        if (npcId <= 0)
            return;
        World world = WorldApplyIncomingPacketServices.GameWorld;
        if (world == null)
            return;
        Entity npc = world.GetEntityNoLockSync(npcId);
        if (npc == null || !InRange(npc))
            Close();
    }

    public static bool InRange(Entity npc)
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null || npc == null || npc.Identity == null)
            return true;
        Vector3 from = player.Identity.GetL2jPos(player.transform.position);
        Vector3 to = npc.Identity.GetL2jPos(npc.transform.position);
        float dx = from.x - to.x;
        float dy = from.y - to.y;
        float dz = from.z - to.z;
        float limit = NetNpcSession.InteractionDistance;
        return dx * dx + dy * dy + dz * dz <= limit * limit;
    }

    public static void Close()
    {
        NetNpcSession.CloseWindows();
        if (GameWindowPacketServices.Html != null)
            GameWindowPacketServices.Html.HideWindow();
        if (MerchantPacketServices.Dealer != null)
            MerchantPacketServices.Dealer.HideWindow();
        if (MultiSellPacketServices.MultiSell != null)
            MultiSellPacketServices.MultiSell.HideWindow();
    }

    static NetShopBalance _balance;
    static bool _hasBalance;
}

public static class NetNpcOrders
{
    public static NetIdCount[] ItemCounts(List<Product> products)
    {
        return Counts(products, true);
    }

    public static NetIdCount[] ObjectCounts(List<Product> products)
    {
        return Counts(products, false);
    }

    static NetIdCount[] Counts(List<Product> products, bool itemId)
    {
        if (products == null || products.Count == 0)
            return Array.Empty<NetIdCount>();
        var rows = new List<NetIdCount>(products.Count);
        for (int i = 0; i < products.Count; i++)
        {
            Product product = products[i];
            if (product == null || product.Count <= 0)
                continue;
            int id = itemId ? product.ItemId : product.ObjId;
            if (id <= 0)
                continue;
            rows.Add(new NetIdCount { Id = id, Count = product.Count });
        }

        return rows.ToArray();
    }
}
