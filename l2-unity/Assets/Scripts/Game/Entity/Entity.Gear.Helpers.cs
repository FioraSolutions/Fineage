using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class Entity
{
    public void EquipAndDetermineType(ItemInstance item, int objectId)
    {
        if (_gear is not UserGear usergear)
        {
            // GearFlowLog.Warn("Equip skip gear not UserGear " + GearFlowLog.Entity(this) +
                // " gear=" + (_gear != null ? _gear.GetType().Name : "null"));
            return;
        }

        // GearFlowLog.Info("EquipAndDetermineType " + GearFlowLog.Entity(this) +
            // " itemId=" + (item != null ? item.ItemId : 0) +
            // " cat=" + (item != null ? item.Category.ToString() : "null") +
            // " body=" + (item != null ? item.BodyPart.ToString() : "null"));

        switch (item.Category)
        {
            case ItemCategory.Weapon:
                HandleWeaponEquip(item, objectId);
                break;

            case ItemCategory.ShieldArmor:
                HandleShieldArmorItem(item, usergear);
                break;

            default:
                // GearFlowLog.Warn("EquipAndDetermineType unhandled cat=" + item.Category +
                    // " itemId=" + item.ItemId + " " + GearFlowLog.Entity(this));
                // Debug.LogWarning($"Entity->EquipAndDetermineType->Unhandled item category: {item.Category}");
                break;
        }
    }

    private void HandleWeaponEquip(ItemInstance item, int objectId)
    {
        if (item.IsDual())
        {
            EquipDualWeapon(item.ItemId);
        }
        else
        {
            EquipWeapon(item.ItemId, false);
        }
    }

    private void HandleUnequipSheildArmorItem(ItemInstance item, UserGear usergear)
    {
        int itemId = item.ItemId;
        Weapon weapon = TablePacketServices.Items.GetWeapon(itemId);

        if (weapon != null)
        {
            usergear.UnequipWeapon(true , item.ItemId);
            usergear.UnequipShield(item.ItemId);
        }
        else
        {

            usergear.UnequipArmor(item.ItemId , item.BodyPart);
        }
    }

    private void HandleShieldArmorItem(ItemInstance item, UserGear usergear)
    {
        int itemId = item.ItemId;

        Weapon weapon = TablePacketServices.Items.GetWeapon(itemId);

        if (weapon != null)
        {
            usergear.EquipShield(itemId);
        }
        else
        {
            usergear.EquipArmor(itemId, item.BodyPart);

        }
    }

    private void OnWeaponEquipped(int weaponId, Weapon weapon)
    {
        if(weapon != null)
        {
            if(this.GetType() == typeof(PlayerEntity))
            {
                PlayerEntity playerEntity = (PlayerEntity)this;
                playerEntity.RefreshRunSpeed();
                PlayerStatePacketServices.PlayerState.NotifyEvent(Event.CHANGE_EQUIP);
                
            }
        }
    }

    private void OnWeaponUnequipped(string lastWeaponAnim)
    {
       if (this.GetType() == typeof(PlayerEntity))
       {
          PlayerStatePacketServices.PlayerState.NotifyEvent(Event.CHANGE_EQUIP);
       }
    }
}
