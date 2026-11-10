using System;
using UnityEngine;

public sealed partial class CharacterArmorDresser
{
    public void UnequipArmorPiece(
        ItemSlot slot,
        int unequipId,
        Armor[] nakedArmor,
        GameObject[] nakedMeshes)
    {
        slot = ArmorBodySlots.Normalize(slot);
        if (slot == ItemSlot.fullarmor)
        {
            if (!_fullBody || _chest.Armor == null || _chest.Armor.Id != unequipId)
            {
                DiscardMeshes(nakedMeshes);
                return;
            }

            ApplyNakedPair(nakedArmor, nakedMeshes);
            return;
        }

        if (!IsSingleRegion(slot))
        {
            DiscardMeshes(nakedMeshes);
            return;
        }

        Worn worn = GetRegion(slot);
        if (worn.Armor == null || worn.Armor.Id != unequipId)
        {
            DiscardMeshes(nakedMeshes);
            return;
        }

        if (nakedMeshes != null && nakedMeshes.Length > 0 && nakedMeshes[0] != null &&
            nakedArmor != null && nakedArmor.Length > 0 && nakedArmor[0] != null)
        {
            ReplaceRegion(slot, nakedArmor[0], nakedMeshes[0]);
            Sync();
            return;
        }

        ClearRegion(slot);
        Sync();
        DiscardMeshes(nakedMeshes);
    }

    void ApplyNakedPair(Armor[] nakedArmor, GameObject[] nakedMeshes)
    {
        if (nakedArmor != null && nakedArmor.Length >= 2 &&
            IsArmorEquipped(nakedArmor[0], ItemSlot.chest) &&
            IsArmorEquipped(nakedArmor[1], ItemSlot.legs) &&
            !_fullBody)
            return;

        if (nakedMeshes != null && nakedMeshes.Length >= 2 &&
            nakedMeshes[0] != null && nakedMeshes[1] != null &&
            nakedArmor != null && nakedArmor.Length >= 2 &&
            nakedArmor[0] != null && nakedArmor[1] != null)
        {
            ClearRegion(ItemSlot.chest);
            ClearRegion(ItemSlot.legs);
            _fullBody = false;
            ReplaceRegion(ItemSlot.chest, nakedArmor[0], nakedMeshes[0]);
            ReplaceRegion(ItemSlot.legs, nakedArmor[1], nakedMeshes[1]);
            Sync();
            return;
        }

        ClearRegion(ItemSlot.chest);
        ClearRegion(ItemSlot.legs);
        _fullBody = false;
        Sync();
        DiscardMeshes(nakedMeshes);
    }

    /// <summary>
    /// Leaving fullarmor for a single chest or legs piece: clear both meshes,
    /// dress naked on the other region, discard unused naked mesh for the kept slot.
    /// </summary>
    void BreakFullBodyForSingle(ItemSlot keepSlot, Armor[] nakedArmor, GameObject[] nakedMeshes)
    {
        ClearRegion(ItemSlot.chest);
        ClearRegion(ItemSlot.legs);
        _fullBody = false;

        if (nakedArmor == null || nakedMeshes == null || nakedArmor.Length < 2 || nakedMeshes.Length < 2)
            return;

        if (keepSlot == ItemSlot.chest)
        {
            if (nakedArmor[1] != null && nakedMeshes[1] != null)
                ReplaceRegion(ItemSlot.legs, nakedArmor[1], nakedMeshes[1]);
            DestroyMesh(nakedMeshes[0]);
        }
        else if (keepSlot == ItemSlot.legs)
        {
            if (nakedArmor[0] != null && nakedMeshes[0] != null)
                ReplaceRegion(ItemSlot.chest, nakedArmor[0], nakedMeshes[0]);
            DestroyMesh(nakedMeshes[1]);
        }
    }

    void ReplaceRegion(ItemSlot slot, Armor armor, GameObject mesh)
    {
        ClearRegion(slot);
        Parent(mesh);
        mesh.SetActive(true);
        SetRegion(slot, new Worn { Armor = armor, Mesh = mesh });
        if (slot == ItemSlot.chest || slot == ItemSlot.legs)
            _fullBody = false;
    }

    void ClearRegion(ItemSlot slot)
    {
        Worn worn = GetRegion(slot);
        DestroyMesh(worn.Mesh);
        SetRegion(slot, default);
    }

    Worn GetRegion(ItemSlot slot)
    {
        return slot switch
        {
            ItemSlot.chest => _chest,
            ItemSlot.legs => _legs,
            ItemSlot.gloves => _gloves,
            ItemSlot.feet => _feet,
            _ => default
        };
    }

    void SetRegion(ItemSlot slot, Worn worn)
    {
        switch (slot)
        {
            case ItemSlot.chest:
                _chest = worn;
                break;
            case ItemSlot.legs:
                _legs = worn;
                break;
            case ItemSlot.gloves:
                _gloves = worn;
                break;
            case ItemSlot.feet:
                _feet = worn;
                break;
        }
    }

    static bool IsSingleRegion(ItemSlot slot)
    {
        return slot == ItemSlot.chest
            || slot == ItemSlot.legs
            || slot == ItemSlot.gloves
            || slot == ItemSlot.feet;
    }

    static bool Same(Armor a, Armor b)
    {
        return a != null && b != null && a.Id == b.Id;
    }

    void Parent(GameObject mesh)
    {
        if (mesh != null)
            mesh.transform.SetParent(_container, false);
    }

    void DestroyMesh(GameObject mesh)
    {
        if (mesh != null)
            OnDestroyGameObject?.Invoke(mesh);
    }

    void DiscardMeshes(GameObject[] meshes)
    {
        if (meshes == null)
            return;
        for (int i = 0; i < meshes.Length; i++)
            DestroyMesh(meshes[i]);
    }

    void Sync()
    {
        OnSyncMesh?.Invoke();
    }
}
