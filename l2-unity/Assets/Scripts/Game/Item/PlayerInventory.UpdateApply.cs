using System;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerInventory
{
    void Publish(bool show)
    {
        Dictionary<int, ItemInstance> beforeEquip = _playerEquipInventory;
        RebuildViews();
        var equipped = new List<ItemInstance>();
        var unequipped = new List<ItemInstance>();
        CollectGearDiff(beforeEquip, _playerEquipInventory, equipped, unequipped);
        NetGearSession.SyncWornEnchant(Book);
        List<string> problems = InventorySurface.Check(Book);
        for (int i = 0; i < problems.Count; i++)
            Debug.LogError("[Inventory] " + problems[i]);

        int userId = UserId();
        Action paint = () =>
        {
            RefreshWindow();
            if (show)
            {
                InventoryWindow window = InventoryPacketServices.Window;
                if (window != null)
                    window.ShowWindow();
            }
            for (int i = 0; i < unequipped.Count; i++)
                GamePacketServices.Bus?.UnEquipped(unequipped[i], userId);
            for (int i = 0; i < equipped.Count; i++)
                GamePacketServices.Bus?.Equipped(equipped[i], userId);
            SkillbarWindow bar = SkillbarPacketServices.Skillbar;
            if (bar != null)
                bar.UpdateAllShortcuts();
        };

        if (!DispatcherPacketServices.Exists)
        {
            Debug.LogError("[Inventory] main-thread dispatcher is missing; inventory UI was not painted");
            return;
        }
        DispatcherPacketServices.Dispatcher.Enqueue(paint);
    }

    void RebuildViews()
    {
        var bag = new Dictionary<int, ItemInstance>();
        var equip = new Dictionary<int, ItemInstance>();
        CharacterInventory.Row[] rows = Book.BagRows();
        for (int i = 0; i < rows.Length; i++)
        {
            ItemInstance item = NetGearSession.ToItemInstance(rows[i], i);
            bag[item.ObjectId] = item;
        }
        CharacterInventory.Row[] worn = Book.PaperdollRows();
        for (int i = 0; i < worn.Length; i++)
        {
            ItemInstance item = NetGearSession.ToItemInstance(worn[i], 0);
            equip[item.ObjectId] = item;
        }
        _playerInventory = bag;
        _playerEquipInventory = equip;
    }

    static void CollectGearDiff(
        Dictionary<int, ItemInstance> before,
        Dictionary<int, ItemInstance> after,
        List<ItemInstance> equipped,
        List<ItemInstance> unequipped)
    {
        var beforeSlots = SlotsOf(before);
        var afterSlots = SlotsOf(after);
        foreach (KeyValuePair<ItemSlot, ItemInstance> pair in beforeSlots)
        {
            ItemInstance now;
            if (!afterSlots.TryGetValue(pair.Key, out now) || now.ObjectId != pair.Value.ObjectId)
                unequipped.Add(pair.Value);
        }
        foreach (KeyValuePair<ItemSlot, ItemInstance> pair in afterSlots)
        {
            ItemInstance was;
            if (!beforeSlots.TryGetValue(pair.Key, out was) || was.ObjectId != pair.Value.ObjectId)
                equipped.Add(pair.Value);
        }
    }

    static Dictionary<ItemSlot, ItemInstance> SlotsOf(Dictionary<int, ItemInstance> source)
    {
        var slots = new Dictionary<ItemSlot, ItemInstance>();
        if (source == null)
            return slots;
        foreach (ItemInstance item in source.Values)
        {
            if (item == null)
                continue;
            slots[item.BodyPart] = item;
        }
        return slots;
    }

    int UserId()
    {
        if (_currentUser == null)
            _currentUser = StorageNpc.getInstance().GetFirstUser();
        if (_currentUser == null || _currentUser.PlayerInfoInterlude.Identity == null)
            return 0;
        return _currentUser.PlayerInfoInterlude.Identity.Id;
    }

    static CharacterInventory.Row RowFromItem(ItemInstance item)
    {
        int equippedSlot = -1;
        if (item.Equipped)
        {
            equippedSlot = PaperdollIndex(item.BodyPart);
            if (equippedSlot < 0)
                Debug.LogError("[Inventory] objectId " + item.ObjectId + " is equipped in " + item.BodyPart + " which has no paperdoll index");
        }
        return new CharacterInventory.Row
        {
            ObjectId = item.ObjectId,
            ItemId = item.ItemId,
            Count = item.Count,
            EnchantLevel = item.EnchantLevel,
            EquippedSlot = item.Equipped && equippedSlot >= 0 ? equippedSlot : -1,
            BodySlot = item.BodyPart.ToString(),
            Kind = KindFromCategory(item.Category),
            Icon = "",
            WeaponType = "",
            ArmorType = ""
        };
    }

    static string KindFromCategory(ItemCategory category)
    {
        if (category == ItemCategory.Weapon)
            return "weapon";
        if (category == ItemCategory.ShieldArmor || category == ItemCategory.Jewel)
            return "armor";
        return "etc";
    }

    static int PaperdollIndex(ItemSlot slot)
    {
        switch (slot)
        {
            case ItemSlot.underwear:
                return NetPaperdoll.Under;
            case ItemSlot.lear:
            case ItemSlot.earring:
                return NetPaperdoll.LeftEar;
            case ItemSlot.rear:
                return NetPaperdoll.RightEar;
            case ItemSlot.neck:
                return NetPaperdoll.Neck;
            case ItemSlot.lfinger:
            case ItemSlot.ring:
                return NetPaperdoll.LeftFinger;
            case ItemSlot.rfinger:
                return NetPaperdoll.RightFinger;
            case ItemSlot.head:
                return NetPaperdoll.Head;
            case ItemSlot.rhand:
            case ItemSlot.lrhand:
                return NetPaperdoll.RightHand;
            case ItemSlot.lhand:
            case ItemSlot.lhand_shield:
                return NetPaperdoll.LeftHand;
            case ItemSlot.gloves:
                return NetPaperdoll.Gloves;
            case ItemSlot.chest:
            case ItemSlot.fullarmor:
            case ItemSlot.alldress:
            case ItemSlot.onepiece:
                return NetPaperdoll.Chest;
            case ItemSlot.legs:
                return NetPaperdoll.Legs;
            case ItemSlot.feet:
            case ItemSlot.boots:
                return NetPaperdoll.Feet;
            case ItemSlot.hat1:
                return NetPaperdoll.Hair;
            case ItemSlot.hat2:
                return NetPaperdoll.Hair2;
            default:
                return -1;
        }
    }
}
