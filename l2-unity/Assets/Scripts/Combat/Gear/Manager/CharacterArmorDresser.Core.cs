using System;
using UnityEngine;

public sealed partial class CharacterArmorDresser
{
    public CharacterArmorDresser(Transform containerTransform)
    {
        _container = containerTransform;
    }

    public bool IsArmorEquipped(Armor armor, ItemSlot slot)
    {
        if (armor == null)
            return false;

        slot = ArmorBodySlots.Normalize(slot);
        if (slot == ItemSlot.fullarmor)
            return _fullBody && Same(_chest.Armor, armor);

        return Same(GetRegion(slot).Armor, armor);
    }
}
