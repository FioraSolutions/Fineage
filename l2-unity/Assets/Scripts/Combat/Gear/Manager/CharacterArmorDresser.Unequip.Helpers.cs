using System;
using UnityEngine;

public sealed partial class CharacterArmorDresser
{
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
