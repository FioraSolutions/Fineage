using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class Entity
{
    public void EquipWeapon(int weaponId , bool isLeftHand)
    {
        _gear.EquipWeapon(weaponId, isLeftHand);
    }

    public void EquipArrow(int itemId , bool leftSlot = false)
    {
        _gear.EquipArrowEtcItem(itemId, leftSlot);
    }

    public bool IsUseBow() => DomainPacketServices.LocalPlayer.GetCurrentAnimName().IndexOf("bow") > -1;

    public void EquipDualWeapon(int weaponId)
    {
        _gear.EquipLeftAndRightWeapon(weaponId);
    }

    public void UnequipWeapon(bool isLeftHand , int itemId , bool lrDestroy)
    {
        _gear.UnequipWeapon(isLeftHand , itemId, lrDestroy);
    }

    public void UnequipAndDetermineType(ItemInstance item)
    {
        if (_gear is not UserGear usergear)
        {
            // GearFlowLog.Warn("Unequip skip gear not UserGear " + GearFlowLog.Entity(this) +
                // " gear=" + (_gear != null ? _gear.GetType().Name : "null"));
            return;
        }

        // GearFlowLog.Info("UnequipAndDetermineType " + GearFlowLog.Entity(this) +
            // " itemId=" + (item != null ? item.ItemId : 0) +
            // " cat=" + (item != null ? item.Category.ToString() : "null"));

        switch (item.Category)
        {
            case ItemCategory.Weapon:
                bool leftHand = (item.IsBow())? true : false;
                bool lrDestroy = (item.IsDual())? true : false;
                UnequipWeapon(leftHand, item.ItemId , lrDestroy);
                break;

            case ItemCategory.ShieldArmor:
                HandleUnequipSheildArmorItem(item, usergear);
                break;

            default:
                // Debug.LogWarning($"Entity->UnEquipAndDetermineType->Unhandled item category: {item.Category}");
                break;
        }

    }
}
