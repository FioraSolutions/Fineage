using System;
using UnityEngine;

public sealed partial class CharacterArmorDresser
{
    public void ApplySingle(
        Armor armor,
        ItemSlot slot,
        GameObject mesh,
        Armor[] nakedArmor,
        GameObject[] nakedMeshes)
    {
        if (armor == null || mesh == null)
            return;

        slot = ArmorBodySlots.Normalize(slot);
        if (!IsSingleRegion(slot))
            return;

        if (IsArmorEquipped(armor, slot))
            return;

        if ((slot == ItemSlot.chest || slot == ItemSlot.legs) && _fullBody)
            BreakFullBodyForSingle(slot, nakedArmor, nakedMeshes);
        else
            DiscardMeshes(nakedMeshes);

        ReplaceRegion(slot, armor, mesh);
        Sync();
    }

    public void ApplyFullBody(Armor armor, GameObject chestMesh, GameObject legsMesh)
    {
        if (armor == null || chestMesh == null)
            return;

        if (IsArmorEquipped(armor, ItemSlot.fullarmor))
            return;

        ClearRegion(ItemSlot.chest);
        ClearRegion(ItemSlot.legs);

        Parent(chestMesh);
        _chest = new Worn { Armor = armor, Mesh = chestMesh };
        chestMesh.SetActive(true);

        if (legsMesh != null)
        {
            Parent(legsMesh);
            _legs = new Worn { Armor = armor, Mesh = legsMesh };
            legsMesh.SetActive(true);
        }
        else
        {
            _legs = default;
        }

        _fullBody = true;
        Sync();
    }

    public void ApplyNaked(ItemSlot slot, Armor[] nakedArmor, GameObject[] nakedMeshes)
    {
        slot = ArmorBodySlots.Normalize(slot);

        if (slot == ItemSlot.fullarmor)
        {
            ApplyNakedPair(nakedArmor, nakedMeshes);
            return;
        }

        if (!IsSingleRegion(slot))
            return;

        if (nakedArmor != null && nakedArmor.Length > 0 &&
            IsArmorEquipped(nakedArmor[0], slot))
            return;

        if (nakedMeshes != null && nakedMeshes.Length > 0 && nakedMeshes[0] != null &&
            nakedArmor != null && nakedArmor.Length > 0 && nakedArmor[0] != null)
        {
            ReplaceRegion(slot, nakedArmor[0], nakedMeshes[0]);
            Sync();
            return;
        }

        ClearRegion(slot);
        if (slot == ItemSlot.chest || slot == ItemSlot.legs)
            _fullBody = false;
        Sync();
    }
}
