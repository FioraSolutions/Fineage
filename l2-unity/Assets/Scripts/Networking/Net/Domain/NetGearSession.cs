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
}
