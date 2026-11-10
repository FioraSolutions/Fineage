using FMOD.Studio;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR;

public partial class ItemInstance : AbstractServerItem
{
    [SerializeField] AbstractItem _itemData;
    [SerializeField] private int _objectId;
    [SerializeField] private int _itemId;
    [SerializeField] private ItemLocation _location;
    [SerializeField] private int _slot;
    [SerializeField] private int _count;
    [SerializeField] private ItemCategory _category;
    [SerializeField] private bool _equipped;
    [SerializeField] private ItemSlot _bodyPart;
    [SerializeField] private int _enchantLevel;
    [SerializeField] private long _remainingTime;
    [SerializeField] private int _lastChange;
    private bool  _requiredItems = false;
    private int  _requiredCount = 0;

    public AbstractItem ItemData { get { return _itemData; } }
    public int ObjectId { get { return _objectId; } }
    public int ItemId { get { return _itemId; } }
    public ItemLocation Location { get { return _location; } }
    public bool Equipped { get { return _equipped; } }
    public int Slot { get { return _slot; } }
    public int Count { get { return _count; } }
    public ItemCategory Category { get { return _category; } }
    public ItemSlot BodyPart { get { return _bodyPart; } }
    public int EnchantLevel { get { return _enchantLevel; } }
    public long RemainingTime { get { return _remainingTime; } }
    public int LastChange { get { return _lastChange; } set { _lastChange = value; } }

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
    public void Update(ItemInstance newItem)
    {
        _location = newItem.Location;
        _slot = newItem.Slot;
        _count = newItem.Count;
        _remainingTime = newItem.RemainingTime;
        _enchantLevel = newItem.EnchantLevel;

        if (_equipped == false && newItem.Equipped == true
        || _equipped == true && newItem.Equipped == false)
        {
            //AudioPacketServices.Audio.PlayEquipSound(_itemData.Itemgrp.EquipSound);
        }

        _equipped = newItem.Equipped;
        _objectId = newItem.ObjectId;
        _bodyPart = newItem.BodyPart;
    }

    public void SetItemRequiredType(bool isRequired , int count)
    {
        _requiredItems = isRequired;
        _requiredCount = count;
    }

    public int GetRequiredCount()
    {
        return _requiredCount;
    }

    public override string ToString()
    {
        return $"New item: ServerId:{_objectId} ItemId:{_itemId} Location:{_location} Slot:{_slot} Count:{_count} " +
        $"Cat:{_category} Equipped:{_equipped} Bodypart:{_bodyPart} Change:{_lastChange}";
    }

    public override bool Equals(object obj)
    {

        if (ReferenceEquals(this, obj)) return true;


        if (obj is ItemInstance other)
        {

            return _objectId == other._objectId && _itemId == other._itemId;
        }

        return false;
    }

    public override int GetHashCode()
    {

        unchecked 
        {
            int hash = 17; 
            hash = hash * 23 + _objectId.GetHashCode(); 
            hash = hash * 23 + _itemId.GetHashCode(); 
            return hash;
        }
    }
    
    //We pass the IsBow bool so that when the server receives information about what needs to be changed,
    //it doesn't select the first element in its LR hands. This is usually the left hand, which usually contains arrows,
    //so we specifically select only the bow from the right hand.
}
