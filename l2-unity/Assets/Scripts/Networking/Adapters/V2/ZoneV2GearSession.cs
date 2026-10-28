using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds zone enter gear until the local player and inventory window exist.
/// UserInfo and ItemList arrive before the world scene is loaded.
/// </summary>
public static class ZoneV2GearSession
{
    static bool _hasUser;
    static ZoneV2UserInfo _user;
    static bool _hasItems;
    static bool _itemsApplied;
    static ZoneV2ItemEntry[] _items;
    static readonly List<ZoneV2InventoryChange[]> _updates = new List<ZoneV2InventoryChange[]>();
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

    public static void NoteUserInfo(ZoneV2UserInfo user)
    {
        _user = user;
        _hasUser = true;
        int rhand = user.HasPaperdoll ? ZoneV2Paperdoll.RightHandItemId(user.Paperdoll) : 0;
        Debug.Log("[ZoneV2] UserInfo objectId=" + user.ObjectId
            + " class=" + user.ClassId
            + " rhand=" + rhand);

        GameClient game = GameClient.Instance;
        if (game != null && game.PlayerInfo.Identity != null && game.PlayerInfo.Identity.Id == user.ObjectId)
            ZoneV2DomainMap.OverlayUserInfo(game.PlayerInfo, user);

        PlayerEntity player = PlayerEntity.Instance;
        if (player != null && player.Identity != null && player.Identity.Id == user.ObjectId)
            ApplyToLivePlayer(player, user);
    }

    public static PlayerInfoInterlude PrepareSpawn(ZoneV2PlayerSpawned spawned)
    {
        PlayerInfoInterlude info = ZoneV2DomainMap.ToPlayerInfo(spawned);
        if (_hasUser)
            ZoneV2DomainMap.OverlayUserInfo(info, _user);
        StorageNpc.getInstance().AddUserInfo(new UserInfoDto(info));
        int rhand = info.Appearance != null ? info.Appearance.RHand : 0;
        int classId = info.Identity != null ? info.Identity.PlayerClass : 0;
        Debug.Log("[ZoneV2] spawn class=" + classId + " rhand=" + rhand);
        return info;
    }

    public static void NoteItemList(ZoneV2ItemEntry[] rows)
    {
        _items = rows ?? System.Array.Empty<ZoneV2ItemEntry>();
        _hasItems = true;
        _itemsApplied = false;
        _updates.Clear();
        FlushPending();
    }

    public static void NoteInventoryUpdate(ZoneV2InventoryChange[] rows)
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
                ItemListDto list = ZoneV2DomainMap.ToItemListDto(_items);
                ItemListApplyIncomingPacketServices.ApplyItemList(list);
                _itemsApplied = true;
                Debug.Log("[ZoneV2] ItemList rows=" + _items.Length);
            }

            if (!_itemsApplied)
                return;

            for (int i = 0; i < _updates.Count; i++)
            {
                InventoryUpdateDto update = ZoneV2DomainMap.ToInventoryUpdateDto(_updates[i]);
                InventoryUpdateApplyIncomingPacketServices.ApplyInventoryUpdate(update);
            }

            _updates.Clear();
        }
        catch (System.Exception ex)
        {
            if (!_warned)
            {
                Debug.LogWarning("[ZoneV2] inventory apply deferred: " + ex.Message);
                _warned = true;
            }
        }
    }

    static void ApplyToLivePlayer(PlayerEntity player, ZoneV2UserInfo user)
    {
        PlayerAppearance appearance = player.Appearance as PlayerAppearance;
        PlayerStats stats = player.Stats as PlayerStats;
        if (player.Identity == null || appearance == null || stats == null)
            return;

        var info = new PlayerInfoInterlude
        {
            Identity = player.Identity,
            Appearance = appearance,
            Stats = stats
        };
        ZoneV2DomainMap.OverlayUserInfo(info, user);
        player.Running = appearance.Running;
        if (stats.RunRealSpeed > 0.001f)
            player.UpdateRunSpeed(stats.RunRealSpeed);
        if (stats.WalkRealSpeed > 0.001f)
            player.UpdateWalkSpeed(stats.WalkRealSpeed);
        player.RefreshVisuals();
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
