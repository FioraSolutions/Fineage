using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetGearSession
{
    static bool _hasUser;
    static NetUserInfo _user;
    static bool _hasItems;
    static bool _itemsApplied;
    static NetItemEntry[] _items;
    static readonly List<NetInventoryChange[]> _updates = new List<NetInventoryChange[]>();
    static bool _warned;

    public static void Reset()
    {
        _hasUser = false;
        _user = default;
        _hasItems = false;
        _itemsApplied = false;
        _items = null;
        _updates.Clear();
        _warned = false;
        _lastBlock = null;
    }

    public static void NoteUserInfo(NetUserInfo user)
    {
        _user = user;
        _hasUser = true;
        int rhand = user.HasPaperdoll ? NetPaperdoll.RightHandItemId(user.Paperdoll) : 0;
        Debug.Log("[Net] UserInfo objectId=" + user.ObjectId
            + " class=" + user.ClassId
            + " rhand=" + rhand);

        IZoneTransport transport = ZoneCommands.Transport;
        if (transport != null && transport.PlayerInfo.Identity != null &&
            transport.PlayerInfo.Identity.Id == user.ObjectId)
            OverlayUserInfo(transport.PlayerInfo, user);

        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player != null && player.Identity != null && player.Identity.Id == user.ObjectId)
            ApplyToLivePlayer(player, user);

        // StorageNpc is how the rest of the client (inventory, clan, status) looks up "me".
        RegisterSelfInStorage(user, transport, player);
        // ItemList may have arrived before UserInfo; apply now that StorageNpc has "me".
        FlushPending();
    }

    /// <summary>
    /// Publishes the local player into <see cref="StorageNpc"/>. Prefers the transport's
    /// PlayerInfo (the full record the zone filled in), falling back to the live PlayerEntity
    /// and finally to a minimal record built from the UserInfo message itself.
    /// </summary>
    static void RegisterSelfInStorage(NetUserInfo user, IZoneTransport transport, PlayerEntity player)
    {
        PlayerInfoInterlude info;
        if (transport != null && transport.PlayerInfo.Identity != null)
        {
            info = transport.PlayerInfo;
        }
        else
        {
            // Last resort: the identity is all the inventory window needs to key its rows.
            info = new PlayerInfoInterlude
            {
                Identity = new EntityIdentity
                {
                    Id = user.ObjectId,
                    Name = user.Name,
                    PlayerClass = user.ClassId
                }
            };
        }

        if (info.Identity == null)
            return;

        StorageNpc.getInstance().AddUserInfo(new UserInfoDto(info));
    }

    public static void NoteItemList(NetItemEntry[] rows)
    {
        _items = rows ?? System.Array.Empty<NetItemEntry>();
        _hasItems = true;
        _itemsApplied = false;
        _updates.Clear();
        FlushPending();
    }

    public static void NoteInventoryUpdate(NetInventoryChange[] rows)
    {
        if (rows == null || rows.Length == 0)
            return;
        _updates.Add(rows);
        FlushPending();
    }

    public static void FlushPending()
    {
        bool hasWork = (_hasItems && !_itemsApplied) || _updates.Count > 0;
        if (!hasWork)
            return;

        if (!CanApplyInventory())
            return;

        try
        {
            if (_hasItems && !_itemsApplied)
            {
                ItemListDto list = ToItemListDto(_items);
                InventoryApplyService inventory = ApplyServices.Inventory;
                if (inventory != null) inventory.ApplyItemList(list);
                else ItemListApplyIncomingPacketServices.ApplyItemList(list);
                _itemsApplied = true;
                Debug.Log("[Net] ItemList rows=" + _items.Length);
            }

            if (!_itemsApplied)
                return;

            for (int i = 0; i < _updates.Count; i++)
            {
                InventoryUpdateDto update = ToInventoryUpdateDto(_updates[i]);
                InventoryApplyService inventory = ApplyServices.Inventory;
                if (inventory != null) inventory.ApplyInventoryUpdate(update);
                else InventoryUpdateApplyIncomingPacketServices.ApplyInventoryUpdate(update);
            }

            _updates.Clear();
        }
        catch (System.Exception ex)
        {
            if (!_warned)
            {
                Debug.LogWarning("[Net] inventory apply deferred: " + ex.Message);
                _warned = true;
            }
        }
    }

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
