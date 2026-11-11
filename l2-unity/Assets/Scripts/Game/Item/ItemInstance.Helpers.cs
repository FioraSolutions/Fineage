using FMOD.Studio;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR;

public partial class ItemInstance : AbstractServerItem
{
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
}
