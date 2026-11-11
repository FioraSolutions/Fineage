using UnityEngine;

public sealed partial class ItemDropVisualService
{
    public ItemDropVisualService(
        ItemDropPrefabLoader prefabs,
        ItemDropGrpCatalog grp,
        ItemDropMaterialService materials,
        ItemDropWeaponAligner weapons,
        ItemDropClickAreaService clickArea)
    {
        _prefabs = prefabs;
        _grp = grp;
        _materials = materials;
        _weapons = weapons;
        _clickArea = clickArea;
    }

    public void AttachVisual(ItemEntity entity, int itemId, int dropperCharObjId = 0)
    {
        if (entity == null)
            return;

        ClearChildren(entity.transform);

        GameObject prefab = _prefabs.Resolve(itemId);
        if (prefab != null)
        {
            Abstractgrp grp = _grp.ResolveGrp(itemId);
            bool coinVisual = ItemDropPrefabLoader.IsCoinPrefab(prefab);
            bool propFallback = !coinVisual && ItemDropPrefabLoader.IsFxDropPropPrefab(prefab);
            bool dropItems = !coinVisual && ItemDropPrefabLoader.IsDropItemsPrefab(prefab);
            bool herbVisual = !coinVisual && _grp.IsHerb(itemId);
            bool weaponVisual = !coinVisual && !propFallback && !dropItems && !herbVisual && _grp.IsWeapon(itemId);
            GameObject visual = Object.Instantiate(prefab, entity.transform);
            visual.name = "DropMesh";
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            float scale = coinVisual ? CoinDropScale : herbVisual ? HerbDropScale : 1f;
            visual.transform.localScale = Vector3.one * scale;
            if (herbVisual)
                ItemDropWeaponAligner.StandUpright(visual);
            else if (weaponVisual)
                _weapons.ScaleWeapon(visual, itemId);
            else if (propFallback)
                ItemDropWeaponAligner.ScaleProp(visual, DropPropWorldSize);
            if (coinVisual)
            {
                _materials.ApplyCoin(visual);
                _clickArea.SitMeshOnGround(visual);
            }
            else if (dropItems)
            {
                _materials.ApplyDropItems(visual, itemId, grp);
                _clickArea.SitMeshOnGround(visual);
            }
            else if (propFallback)
            {
                _materials.ApplyPropFallback(visual, grp);
                _clickArea.SitMeshOnGround(visual);
            }
            else if (!weaponVisual)
                _clickArea.SitMeshOnGround(visual);
        }
        else
        {
            Abstractgrp grp = _grp.ResolveGrp(itemId);
            // Debug.LogWarning(
                // $"[DropItemMesh] NO_VISUAL itemId={itemId} dropModel='{grp?.DropModel ?? "null"}' " +
                // $"equipModel='{_grp.ResolveEquipModel(grp) ?? "null"}'; item remains collider-only.");
        }

        _clickArea.Refresh(entity);
    }
}
