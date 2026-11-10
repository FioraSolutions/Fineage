using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{
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


}
