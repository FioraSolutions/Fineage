using System;
using UnityEngine;
using VContainer;

public partial class PlayerEntity : Entity
{
    public void RefreshVisuals()
    {
        // GearFlowLog.Info("PlayerEntity.RefreshVisuals " + GearFlowLog.Entity(this) +
            // " " + GearFlowLog.Paperdoll(this) +
            // " gear=" + (_gear != null ? _gear.GetType().Name : "null"));
        EquipAllWeapons();
        EquipAllArmors();
    }

    void OnDestroy()
    {
        _instance = null;
    }

    private void EquipAllArmors()
    {
        PlayerAppearance appearance = _appearance as PlayerAppearance;
        UserGear gear = _gear as UserGear;
        if (appearance == null || gear == null)
        {
            // GearFlowLog.Warn("PlayerEntity.EquipAllArmors abort " + GearFlowLog.Entity(this) +
                // " appearance=" + (appearance != null) + " gear=" + (_gear != null ? _gear.GetType().Name : "null"));
            return;
        }

        // GearFlowLog.Info("PlayerEntity.EquipAllArmors " + GearFlowLog.Entity(this) +
            // " " + GearFlowLog.Paperdoll(appearance));
        gear.SyncEquippedArmor(appearance);
    }

    public string GetEquippedWeaponName()
    {
        PlayerGear gear = (PlayerGear)_gear;
        return gear.GetNameWeapon();
    }
}
