using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{
    internal void ApplyDropItem(DropItemNetMsg drop)
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

    internal void ApplyInventoryUpdate(InventoryUpdateNetMsg update)
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
                BodySlot = row.BodySlot ?? "",
                Kind = row.Kind ?? "",
                Icon = row.Icon ?? "",
                WeaponType = row.WeaponType ?? "",
                ArmorType = row.ArmorType ?? ""
            };
        }

        NetGearSession.NoteInventoryUpdate(rows);
    }

    internal static void NoteReject(string code, string message)
    {
        string line = NetRejectLines.Resolve(code, message);
        if (!string.IsNullOrEmpty(line))
            NetChatFeed.NoteSystem(line);
    }

    internal void ApplyPlayerStats(PlayerStatsMsg msg)
    {
        // Prefer live entity; if world tile not loaded yet, patch stashed PlayerInfo for spawn.
        PlayerEntity local = DomainPacketServices.LocalPlayer;
        PlayerStats stats = local != null ? local.Stats as PlayerStats : null;
        PlayerStatus status = local != null ? local.Status as PlayerStatus : null;

        PlayerInfoInterlude playerInfo = ZoneCommands.LocalPlayerInfo;
        if (stats == null && playerInfo.Stats != null)
            stats = playerInfo.Stats;
        if (status == null && playerInfo.Status != null)
            status = playerInfo.Status;

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
}
