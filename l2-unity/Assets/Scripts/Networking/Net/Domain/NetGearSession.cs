using System.Collections.Generic;
using Fineage.Net.Contract;
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
            NetGearMap.OverlayUserInfo(transport.PlayerInfo, user);

        PlayerEntity player = PlayerEntity.Instance;
        if (player != null && player.Identity != null && player.Identity.Id == user.ObjectId)
            ApplyToLivePlayer(player, user);

        // StorageNpc is how the rest of the client (inventory, clan, status) looks up "me".
        RegisterSelfInStorage(user, transport, player);
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
        NetGearMap.OverlayUserInfo(info, user);
        player.Running = appearance.Running;
        if (stats.RunRealSpeed > 0.001f)
            player.UpdateRunSpeed(stats.RunRealSpeed);
        if (stats.WalkRealSpeed > 0.001f)
            player.UpdateWalkSpeed(stats.WalkRealSpeed);
        player.RefreshVisuals();
        CharacterInfoWindow characterInfo = HudCorePacketServices.CharacterInfo;
        if (characterInfo != null)
            characterInfo.UpdateValues();
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
        if (!UnityMainThreadDispatcher.Exists() || EventBus.Instance == null)
        {
            WarnBlocked("dispatcher/EventBus");
            return false;
        }
        if (ItemTable.Instance.Weapons.Count == 0)
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

    static ItemListDto ToItemListDto(NetItemEntry[] rows)
    {
        var bag = new Dictionary<int, ItemInstance>();
        var equip = new Dictionary<int, ItemInstance>();
        int indexBag = 0;
        int indexEquip = 0;
        if (rows != null)
        {
            for (int i = 0; i < rows.Length; i++)
                AddZoneRow(rows[i], -1, bag, equip, ref indexBag, ref indexEquip);
        }

        return ItemListDto.FromZoneInventory(bag, equip);
    }

    static InventoryUpdateDto ToInventoryUpdateDto(NetInventoryChange[] rows)
    {
        var bag = new Dictionary<int, ItemInstance>();
        var equip = new Dictionary<int, ItemInstance>();
        int indexBag = 0;
        int indexEquip = 0;
        if (rows != null)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                NetInventoryChange row = rows[i];
                var entry = new NetItemEntry
                {
                    ObjectId = row.ObjectId,
                    ItemId = row.ItemId,
                    Count = row.Count,
                    EquippedSlot = row.EquippedSlot,
                    BodySlot = row.BodySlot,
                    Kind = row.Kind,
                    Icon = row.Icon,
                    WeaponType = row.WeaponType,
                    ArmorType = row.ArmorType,
                    EnchantLevel = row.EnchantLevel
                };
                AddZoneRow(entry, row.Change, bag, equip, ref indexBag, ref indexEquip);
            }
        }

        return InventoryUpdateDto.FromZoneChanges(bag, equip);
    }

    static void AddZoneRow(
        NetItemEntry row,
        int zoneChange,
        Dictionary<int, ItemInstance> bag,
        Dictionary<int, ItemInstance> equip,
        ref int indexBag,
        ref int indexEquip)
    {
        bool worn = row.EquippedSlot >= 0;
        ItemCategory category = CategoryFromZone(row);
        ItemSlot slot = SlotFromBodyToken(row.BodySlot);
        // The Net ItemList row only carries the paperdoll index, not the body token or kind, so resolve
        // both from the local grp tables. Without this every item was ItemCategory.Item / ItemSlot.none,
        // and the equipped weapon never reached the paperdoll: the bag had items, the weapon did not.
        if (category == ItemCategory.Item && slot == ItemSlot.none)
            ResolveFromLocalTables(row.ItemId, ref category, ref slot);
        ItemLocation location = worn ? ItemLocation.Equipped : ItemLocation.Inventory;
        int index = worn ? indexEquip++ : indexBag++;
        var item = new ItemInstance(
            row.ObjectId,
            row.ItemId,
            location,
            index,
            row.Count,
            category,
            worn,
            slot,
            row.EnchantLevel,
            9999);
        if (zoneChange >= 0)
            item.LastChange = MapZoneInventoryChange(zoneChange);

        if (worn)
            equip[row.ObjectId] = item;
        else
            bag[row.ObjectId] = item;
    }

    static int MapZoneInventoryChange(int zoneChange)
    {
        // Fineage.Protocol.InventoryChange: 0 add, 1 modify, 2 remove.
        // PlayerInventory.InventoryChange stores those as 1, 2, and 3.
        return NetInventoryBook.ToClientChange(zoneChange);
    }

    /// <summary>
    /// Fills category/body slot from the client's own weapongrp / armorgrp tables when the wire row
    /// did not carry them. Net ItemList sends ObjectId, ItemId, Count, EnchantLevel and EquippedSlot only.
    /// </summary>
    static void ResolveFromLocalTables(int itemId, ref ItemCategory category, ref ItemSlot slot)
    {
        ItemTable table = ItemTable.Instance;
        if (table == null)
            return;

        Weapon weapon = table.GetWeapon(itemId);
        if (weapon != null && weapon.Weapongrp != null)
        {
            category = ItemCategory.Weapon;
            slot = weapon.Weapongrp.BodyPart;
            return;
        }

        Armor armor = table.GetArmor(itemId);
        if (armor != null && armor.Armorgrp != null)
        {
            slot = armor.Armorgrp.BodyPart;
            // Rings/earrings/necklaces live in armorgrp but tooltips use Jewel/ACCESSORY, not armor.
            category = IsJewelItemSlot(slot) ? ItemCategory.Jewel : ItemCategory.ShieldArmor;
            return;
        }

        if (table.GetEtcItem(itemId) != null)
            category = itemId == 57 ? ItemCategory.Adena : ItemCategory.Item;
    }

    static bool IsJewelItemSlot(ItemSlot slot)
    {
        switch (slot)
        {
            case ItemSlot.neck:
            case ItemSlot.lear:
            case ItemSlot.rear:
            case ItemSlot.lfinger:
            case ItemSlot.rfinger:
                return true;
            default:
                return false;
        }
    }

    static ItemCategory CategoryFromZone(NetItemEntry row)
    {
        if (row.ItemId == 57)
            return ItemCategory.Adena;
        if (IsJewelBody(row.BodySlot))
            return ItemCategory.Jewel;
        string kind = row.Kind ?? "";
        if (kind == "weapon")
            return ItemCategory.Weapon;
        if (kind == "armor")
            return ItemCategory.ShieldArmor;
        return ItemCategory.Item;
    }

    static ItemSlot SlotFromBodyToken(string bodySlot)
    {
        if (string.IsNullOrEmpty(bodySlot) || bodySlot == "none")
            return ItemSlot.none;
        switch (bodySlot)
        {
            case "neck":
                return ItemSlot.neck;
            case "head":
                return ItemSlot.head;
            case "lhand":
                return ItemSlot.lhand;
            case "gloves":
                return ItemSlot.gloves;
            case "chest":
                return ItemSlot.chest;
            case "rhand":
                return ItemSlot.rhand;
            case "lear":
                return ItemSlot.lear;
            case "feet":
                return ItemSlot.feet;
            case "legs":
                return ItemSlot.legs;
            case "lrhand":
                return ItemSlot.lrhand;
            case "fullarmor":
                return ItemSlot.fullarmor;
            case "rear":
                return ItemSlot.rear;
            case "lfinger":
                return ItemSlot.lfinger;
            case "rfinger":
                return ItemSlot.rfinger;
            case "underwear":
                return ItemSlot.underwear;
            case "both_ears":
                return ItemSlot.lear;
            case "both_fingers":
                return ItemSlot.lfinger;
            case "hair":
            case "hairall":
                return ItemSlot.hat1;
            case "hair2":
                return ItemSlot.hat2;
            case "alldress":
                return ItemSlot.chest;
            default:
                return ItemSlot.none;
        }
    }

    static bool IsJewelBody(string bodySlot)
    {
        switch (bodySlot)
        {
            case "neck":
            case "lear":
            case "rear":
            case "both_ears":
            case "lfinger":
            case "rfinger":
            case "both_fingers":
                return true;
            default:
                return false;
        }
    }
}
