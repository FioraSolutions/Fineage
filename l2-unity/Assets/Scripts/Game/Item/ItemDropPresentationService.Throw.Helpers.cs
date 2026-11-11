using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemDropPresentationService
{
    void LandItem(
        ItemEntity item,
        Vector3 landPos,
        int itemObjectId,
        ItemDropVisualKind kind,
        ThrowArcState throwState)
    {
        if (item == null)
            return;

        landPos = WorldPlacement.Resolve(WorldPlacement.PlacementKind.ItemDrop, landPos);
        item.transform.position = landPos;

        if (kind == ItemDropVisualKind.Adena)
        {
            HideCoinDropMeshUntilPile(item, itemObjectId);
            PlayLandBurst(itemObjectId, item.transform);
            StartGroundGlow(itemObjectId, item.transform, kind);
            _clickArea.Refresh(item);
            return;
        }

        bool stick = throwState != null && throwState.StickOnLand;
        if (stick)
        {
            Vector3 throwDir = throwState.To - throwState.From;
            if (!_weapons.AlignBladeTipDown(item.transform, throwDir))
                item.transform.rotation = Quaternion.Euler(throwState.LandEuler);
            _weapons.PlantStuckInGround(item, landPos);
        }
        else
        {
            item.transform.rotation = Quaternion.identity;
            _clickArea.SitOnGround(item);
        }

        _clickArea.Refresh(item);
        StartGroundGlow(itemObjectId, item.transform, kind);
    }

    void StopThrow(int itemObjectId)
    {
        for (int i = _throws.Count - 1; i >= 0; i--)
        {
            if (_throws[i].ItemObjectId != itemObjectId)
                continue;

            SetSpinEnabled(_throws[i].Item, false);
            _throws.RemoveAt(i);
        }
    }

    static void SetSpinEnabled(ItemEntity item, bool enabled)
    {
        ItemPickupMotion motion = item != null ? item.GetComponent<ItemPickupMotion>() : null;
        if (motion != null)
            motion.enabled = enabled;
    }

    static Vector3 ResolveThrowStart(Entity dropper, Vector3 landPos)
    {
        if (dropper != null)
        {
            Transform hand = dropper.GetWeaponTransform();
            if (hand == null && dropper.Gear != null)
                hand = dropper.Gear.GetTransformRightHandBone();
            if (hand != null)
                return hand.position;

            float ch = dropper.Appearance != null
                ? L2NameplateAnchor.CollisionHeightToUnityMeters(dropper.Appearance.CollisionHeight)
                : L2NameplateAnchor.DefaultCollisionHeightMeters;
            return dropper.transform.position + Vector3.up * (ch * 1.15f);
        }

        return landPos + Vector3.up * ItemDropPresentationIds.DropThrowStartHeightMeters;
    }
}
