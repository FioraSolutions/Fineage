using System;
using UnityEngine;

/// <summary>
/// Parents body armor meshes under the character model container.
/// State is four regions (chest/legs/gloves/feet) plus a full-body flag when
/// chest+legs come from one onepiece item.
/// </summary>
public sealed partial class CharacterArmorDresser
{
    struct Worn
    {
        public Armor Armor;
        public GameObject Mesh;
    }

    readonly Transform _container;

    Worn _chest;
    Worn _legs;
    Worn _gloves;
    Worn _feet;
    bool _fullBody;

    public Action<GameObject> OnDestroyGameObject;
    public Action OnSyncMesh;

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
