using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds zone enter gear until the local player and inventory window exist.
/// UserInfo and ItemList arrive before the world scene is loaded.
/// </summary>
public static class NetGearSession
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
            NetDomainMap.OverlayUserInfo(transport.PlayerInfo, user);

        PlayerEntity player = PlayerEntity.Instance;
        if (player != null && player.Identity != null && player.Identity.Id == user.ObjectId)
            ApplyToLivePlayer(player, user);
    }

    public static PlayerInfoInterlude PrepareSpawn(NetPlayerSpawned spawned)
    {
        PlayerInfoInterlude info = NetDomainMap.ToPlayerInfo(spawned);
        if (_hasUser)
            NetDomainMap.OverlayUserInfo(info, _user);
        StorageNpc.getInstance().AddUserInfo(new UserInfoDto(info));
        int rhand = info.Appearance != null ? info.Appearance.RHand : 0;
        int classId = info.Identity != null ? info.Identity.PlayerClass : 0;
        Debug.Log("[Net] spawn class=" + classId + " rhand=" + rhand);
        return info;
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
        if (!CanApplyInventory())
            return;

        try
        {
            if (_hasItems && !_itemsApplied)
            {
                ItemListDto list = NetDomainMap.ToItemListDto(_items);
                ItemListApplyIncomingPacketServices.ApplyItemList(list);
                _itemsApplied = true;
                Debug.Log("[Net] ItemList rows=" + _items.Length);
            }

            if (!_itemsApplied)
                return;

            for (int i = 0; i < _updates.Count; i++)
            {
                InventoryUpdateDto update = NetDomainMap.ToInventoryUpdateDto(_updates[i]);
                InventoryUpdateApplyIncomingPacketServices.ApplyInventoryUpdate(update);
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
        NetDomainMap.OverlayUserInfo(info, user);
        player.Running = appearance.Running;
        if (stats.RunRealSpeed > 0.001f)
            player.UpdateRunSpeed(stats.RunRealSpeed);
        if (stats.WalkRealSpeed > 0.001f)
            player.UpdateWalkSpeed(stats.WalkRealSpeed);
        player.RefreshVisuals();
        CharacterInfoWindow characterInfo = CharacterInfoWindow.Instance;
        if (characterInfo != null)
            characterInfo.UpdateValues();
    }

    static bool CanApplyInventory()
    {
        if (PlayerInventory.Instance == null)
            return false;
        InventoryWindow window = InventoryWindow.Instance;
        if (window == null || !window.IsItemSurfaceReady)
            return false;
        if (!UnityMainThreadDispatcher.Exists() || EventBus.Instance == null)
            return false;
        if (ItemTable.Instance.Weapons.Count == 0)
            return false;
        UserInfoDto user = StorageNpc.getInstance().GetFirstUser();
        return user != null && user.PlayerInfoInterlude.Identity != null;
    }
}
