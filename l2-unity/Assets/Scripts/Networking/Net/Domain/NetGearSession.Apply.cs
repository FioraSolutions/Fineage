using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetGearSession
{
    static void ApplyToLivePlayer(PlayerEntity player, NetUserInfo user)
    {
        PlayerAppearance appearance = player.Appearance as PlayerAppearance;
        PlayerStats stats = player.Stats as PlayerStats;
        PlayerStatus status = player.Status as PlayerStatus;
        if (player.Identity == null || appearance == null || stats == null)
            return;

        var info = new PlayerInfoInterlude
        {
            Identity = player.Identity,
            Appearance = appearance,
            Stats = stats,
            Status = status
        };
        OverlayUserInfo(info, user);
        if (player.Identity != null)
            L2OverheadAnchor.Invalidate(player.Identity.Id);
        player.Running = appearance.Running;
        if (stats.RunRealSpeed > 0.001f)
            player.UpdateRunSpeed(stats.RunRealSpeed);
        if (stats.WalkRealSpeed > 0.001f)
            player.UpdateWalkSpeed(stats.WalkRealSpeed);
        // Overlay writes PAtkSpd/MAtkSpd onto Stats; drive anim converters like spawn/CharInfo do.
        if (stats.PAtkSpd > 0)
            player.UpdatePAtkSpeed(stats.PAtkSpd);
        if (stats.MAtkSpd > 0)
            player.UpdateMAtkSpeed(stats.MAtkSpd);
        player.RefreshVisuals();
        CharacterInfoWindow characterInfo = HudCorePacketServices.CharacterInfo;
        if (characterInfo != null)
            characterInfo.UpdateValues();
        InventoryWindow inventory = InventoryPacketServices.Window;
        if (inventory != null)
            inventory.UpdateStats(stats);
    }

    static bool CanApplyInventory()
    {
        if (InventoryPacketServices.Inventory == null)
        {
            WarnBlocked("PlayerInventory");
            return false;
        }
        // The inventory window used to be a precondition here. That made the snapshot wait for
        // a window which itself stays empty until the snapshot arrives -- nobody could ever
        // open the bag. PlayerInventory now caches the rows and pushes them into the window
        // once it exists, so the data path no longer depends on the UI being built.
        if (!DispatcherPacketServices.Exists || GamePacketServices.Bus == null)
        {
            WarnBlocked("dispatcher/EventBus");
            return false;
        }
        if (TablePacketServices.Items.Weapons.Count == 0)
        {
            WarnBlocked("ItemTable.Weapons");
            return false;
        }
        UserInfoDto user = StorageNpc.getInstance().GetFirstUser();
        if (user == null)
        {
            WarnBlocked("StorageNpc user");
            return false;
        }
        if (user.PlayerInfoInterlude.Identity == null)
        {
            WarnBlocked("user identity");
            return false;
        }
        return true;
    }

    static string _lastBlock;

    static void WarnBlocked(string reason)
    {
        if (_lastBlock == reason)
            return;
        _lastBlock = reason;
        UnityEngine.Debug.Log("[Net] inventory apply blocked by: " + reason);
    }

    /// <summary>
    /// Copies UserInfo class, speeds, paperdoll, vitals, and template stats onto a spawn record.
    /// Position stays on the spawn packet.
    /// </summary>
    static void OverlayUserInfo(PlayerInfoInterlude info, NetUserInfo user)
    {
        if (info.Identity == null || info.Appearance == null || info.Stats == null)
            return;

        info.Identity.PlayerClass = user.ClassId;
        // Prefer race from UserInfo (server). Never invent via incomplete class→race maps
        // that historically fell through to DarkElf for 3rd professions (e.g. Archmage=94).
        if (user.Race >= 0 && user.Race <= (int)CharacterRace.Dwarf)
            info.Appearance.Race = user.Race;
        if (user.Sex == 0 || user.Sex == 1)
            info.Appearance.Sex = user.Sex;
        bool mage = CharacterClassParser.IsMage(user.ClassId);
        info.Appearance.BaseClass = mage ? (int)BaseClass.MMagic : (int)BaseClass.Fighter;
        if (!string.IsNullOrEmpty(user.Name))
            info.Identity.Name = user.Name;
        if (user.RunSpeed > 0.001f)
        {
            int run = Mathf.RoundToInt(user.RunSpeed);
            info.Stats.BaseRunSpeed = run;
            info.Stats.RunRealSpeed = user.RunSpeed;
            info.Stats.Speed = run;
        }

        if (user.WalkSpeed > 0.001f)
        {
            info.Stats.BaseWalkingSpeed = Mathf.RoundToInt(user.WalkSpeed);
            info.Stats.WalkRealSpeed = user.WalkSpeed;
        }

        info.Appearance.Running = user.Running;
        info.Identity.IsRunning = user.Running;
        if (user.HasClan)
            info.Identity.ClanId = user.ClanId;
        if (user.HasPaperdoll)
            ApplyUserPaperdoll(info.Appearance, user.Paperdoll);

        if (user.HasVitals)
        {
            if (user.Level > 0)
                info.Stats.Level = user.Level;
            if (user.MaxHp > 0)
                info.Stats.MaxHp = user.MaxHp;
            if (user.MaxMp > 0)
                info.Stats.MaxMp = user.MaxMp;
            if (user.MaxCp > 0)
                info.Stats.MaxCp = user.MaxCp;
            info.Stats.Exp = user.Exp;
            info.Stats.Sp = user.Sp;
            if (info.Status != null)
            {
                if (user.MaxHp > 0 || user.CurHp > 0)
                    info.Status.SetHp(user.CurHp);
                if (user.MaxMp > 0 || user.CurMp > 0)
                    info.Status.SetMp(user.CurMp);
                info.Status.Cp = user.CurCp;
                PlayerResourceCaps.EnsureMaxAtLeastCurrent(info.Stats, info.Status);
            }
        }

        if (user.HasStats)
        {
            info.Stats.Str = user.Str;
            info.Stats.Dex = user.Dex;
            info.Stats.Con = user.Con;
            info.Stats.Int = user.Int;
            info.Stats.Wit = user.Wit;
            info.Stats.Men = user.Men;
            info.Stats.PAtk = user.PAtk;
            info.Stats.MAtk = user.MAtk;
            info.Stats.PDef = user.PDef;
            info.Stats.MDef = user.MDef;
            info.Stats.PAtkSpd = user.PAtkSpd;
            info.Stats.MAtkSpd = user.MAtkSpd;
            info.Stats.PAccuracy = user.Accuracy;
            info.Stats.PEvasion = user.Evasion;
            info.Stats.PCritical = user.CritRate;
            info.Stats.MaxExp = LevelServer.GetExp(info.Stats.Level + 1);
        }

        // Weight travels on UserInfo with the combat sheet; apply whenever we have a Stats bag.
        info.Stats.CurrWeight = user.CurrWeight;
        info.Stats.MaxWeight = user.MaxWeight;
        info.Stats.Karma = user.Karma;
        if (info.Status != null)
            info.Status.PvpFlag = user.PvpFlag;
    }


}
