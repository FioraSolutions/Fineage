using FMOD.Studio;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR;

public partial class ItemInstance : AbstractServerItem
{
    public ItemInstance(int objectId, int itemId, ItemLocation location, int slot, int count, ItemCategory category, bool equipped, ItemSlot bodyPart, int enchantLevel, long remainingTime)
    {
        _objectId = objectId;
        _itemId = itemId;
        SetItemId(itemId);
        _location = location;
        _slot = slot;
        _count = count;
        _category = category;
        _equipped = equipped;
        _bodyPart = bodyPart;
        _remainingTime = remainingTime;
        _enchantLevel = enchantLevel;

        SetItemData(category, bodyPart);


        //Debug.Log(this.ToString());
    }

    private void SetItemData(ItemCategory category , ItemSlot bodyPart)
    {
        if (_category == ItemCategory.Weapon)
        {
            _itemData = TablePacketServices.Items.GetWeapon(_itemId);
        }
        else if (_category == ItemCategory.ShieldArmor || _category == ItemCategory.Jewel)
        {
            // Armor/jewels in armorgrp; Interlude shields are ShieldArmor with data in weapongrp.
            AbstractItem armor = TablePacketServices.Items.GetArmor(_itemId);
            _itemData = armor != null
                ? armor
                : TablePacketServices.Items.GetWeapon(_itemId);
        }
        else
        {
            _itemData = TablePacketServices.Items.GetEtcItem(_itemId);
        }
    }

    public ItemInstance(int itemId, int count, int position)
      : this(-1, itemId, ItemLocation.Void, position, count, ItemCategory.Item, false, ItemSlot.none, 0, -1)
    {

    }

    public int GetPosition()
    {
        return _slot;
    }

    public void SetSlot(int slot)
    {
        _slot = slot;
    }
}
