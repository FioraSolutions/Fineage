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


    internal void ApplyUserInfo(UserInfoNetMsg msg)
    {
        // The zone server sends UserInfo right after the self EntitySpawn, while the
        // world scene is still loading. NoteUserInfo stashes it and overlays the live
        // player / stashed PlayerInfo as those appear, so the paperdoll and stats are
        // not lost to the load order.
        NetUserInfo user = MapUserInfo(msg);
        NetGearSession.NoteUserInfo(user);
    }


    internal void ApplyItemList(ItemListNetMsg list)
    {
        NetItemEntry[] rows = MapItemList(list);
        NetGearSession.NoteItemList(rows);
        Debug.Log("[Net] ItemList count=" + rows.Length);
    }


    internal void ApplySkillList(SkillListNetMsg list)
    {
        NetSkillEntry[] rows = MapSkillList(list);
        SkillListDto dto = MapSkillListToDto(rows);
        if (_skills != null) _skills.ApplySkillList(dto);
        else SkillListApplyIncomingPacketServices.ApplySkillList(dto);
        Debug.Log("[Net] SkillList count=" + rows.Length);
    }

    /// <summary>
    /// <c>UserInfo</c> wire → <see cref="NetUserInfo"/> for <see cref="NetGearSession.NoteUserInfo"/>.
    ///
    /// <para>
    /// Every flag is set from the message rather than guessed: the zone server always
    /// populates vitals, stats and clan (0 meaning "none"), and an empty paperdoll is a
    /// legitimate answer for an unequipped character, not a missing tail.
    /// </para>
    /// </summary>

    static NetUserInfo MapUserInfo(UserInfoNetMsg msg)
    {
        var user = new NetUserInfo
        {
            ObjectId = (int)msg.EntityValue,
            Name = msg.Name,
            ClassId = msg.ClassId,
            Race = msg.Race,
            Sex = msg.Sex,
            X = msg.X,
            Y = msg.Y,
            Z = msg.Z,
            RunSpeed = msg.RunSpeed,
            WalkSpeed = msg.WalkSpeed,
            Running = msg.Running,
            HasClan = true,
            ClanId = msg.ClanId,
            HasVitals = true,
            Level = msg.Level,
            CurHp = msg.CurHp,
            MaxHp = msg.MaxHp,
            CurMp = msg.CurMp,
            MaxMp = msg.MaxMp,
            CurCp = msg.CurCp,
            MaxCp = msg.MaxCp,
            Exp = msg.Exp,
            Sp = msg.Sp,
            HasStats = true,
            Str = msg.Str,
            Dex = msg.Dex,
            Con = msg.Con,
            Int = msg.Int,
            Wit = msg.Wit,
            Men = msg.Men,
            PAtk = msg.PAtk,
            MAtk = msg.MAtk,
            PDef = msg.PDef,
            MDef = msg.MDef,
            PAtkSpd = msg.PAtkSpd,
            MAtkSpd = msg.MAtkSpd,
            Accuracy = msg.Accuracy,
            Evasion = msg.Evasion,
            CritRate = msg.CritRate,
            CurrWeight = msg.CurrentWeight,
            MaxWeight = msg.MaxWeight,
            Karma = msg.Karma,
            PvpFlag = msg.PvpFlag
        };

        PaperdollSlotNet[] slots = msg.Paperdoll;
        if (slots == null || slots.Length == 0)
        {
            user.HasPaperdoll = false;
            user.Paperdoll = System.Array.Empty<NetPaperdollSlot>();
            return user;
        }

        var paperdoll = new NetPaperdollSlot[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            PaperdollSlotNet src = slots[i];
            paperdoll[i] = new NetPaperdollSlot
            {
                ObjectId = src?.ObjectId ?? 0,
                ItemId = src?.ItemId ?? 0,
                EnchantLevel = src?.EnchantLevel ?? 0
            };
        }

        user.HasPaperdoll = true;
        user.Paperdoll = paperdoll;
        return user;
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


    static SkillListDto MapSkillListToDto(NetSkillEntry[] rows)
    {
        var dto = new SkillListDto();
        if (rows == null)
            return dto;
        for (int i = 0; i < rows.Length; i++)
        {
            NetSkillEntry row = rows[i];
            bool passive = IsPassiveSkill(row.SkillId, row.Level);
            bool disabled = row.ReuseDelayMs > 0;
            dto.Skills.Add(new SkillInstance(row.SkillId, row.Level, passive, disabled));
        }

        return dto;
    }


    static bool IsPassiveSkill(int skillId, int level)
    {
        Skillgrp skill = TablePacketServices.SkillGrps != null ? TablePacketServices.SkillGrps.GetSkill(skillId, level) : null;
        return skill != null && skill.OperateType == 2;
    }


    internal void ApplyMoveState(MoveStateMsg move)
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


    internal void ApplyCorrection(PositionCorrectionMsg corr)
    {
        int objectId = (int)corr.EntityValue;
        Vector3 pos = VectorUtils.ConvertPosToUnity(new Vector3(corr.X, corr.Y, corr.Z));
        // Domain apply — never INTENTION_MOVE_TO with dest==origin (fights approach MoveState).
        PostCritical(() =>
        {
            if (_sink != null)
                _sink.ApplyPositionCorrection(objectId, pos);
            else if (WorldApplyPacketServices.WorldApply != null)
                WorldApplyPacketServices.WorldApply.ApplyPositionCorrection(objectId, pos);
        });
    }
}
