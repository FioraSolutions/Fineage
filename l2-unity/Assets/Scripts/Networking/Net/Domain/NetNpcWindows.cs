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



    /// <summary>
    /// Net <c>AcquireSkillList</c>: fill the trainer window (Mobius <c>Folk.showSkillList</c> answer).
    /// The window's own learn flow then sends <c>RequestAcquireSkillInfo</c> / <c>RequestAcquireSkill</c>.
    /// </summary>

    /// <summary>Net <c>AcquireSkillInfo</c>: the confirmation page (Mobius <c>AcquireSkillInfo</c>).</summary>

    /// <summary>
    /// Net <c>EnchantResult</c>: hand the finished roll to the enchant window, then let the
    /// following <c>InventoryUpdate</c> land the level and crystal rows.
    /// </summary>










    /// <summary>
    /// Interlude TYPE2 from client grp tables (BuyList/SellList/warehouse rows have no type on the wire).
    /// </summary>

        type2 == (int)EnumType2.TYPE2_WEAPON
        || type2 == (int)EnumType2.TYPE2_SHIELD_ARMOR
        || type2 == (int)EnumType2.TYPE2_ACCESSORY;

}

public static class NetNpcOrders
{


}
