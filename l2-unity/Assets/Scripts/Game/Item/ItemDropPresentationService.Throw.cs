using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemDropPresentationService
{
    void TickThrows(float deltaTime)
    {
        for (int i = _throws.Count - 1; i >= 0; i--)
        {
            ThrowArcState state = _throws[i];
            if (state.Item == null)
            {
                _throws.RemoveAt(i);
                continue;
            }

            state.Elapsed += deltaTime;
            float u = Mathf.Clamp01(state.Elapsed / state.Duration);
            Vector3 pos = Vector3.Lerp(state.From, state.To, u);
            pos.y += Mathf.Sin(u * Mathf.PI) * state.ArcHeight;
            state.Item.transform.position = pos;

            if (state.TumbleDegPerSec != 0f && state.TumbleAxis.sqrMagnitude > 0.0001f)
                state.Item.transform.Rotate(state.TumbleAxis, state.TumbleDegPerSec * deltaTime, Space.World);

            if (state.Elapsed < state.Duration)
                continue;

            _throws.RemoveAt(i);
            LandItem(state.Item, state.To, state.ItemObjectId, state.Kind, state);
        }
    }

    void BeginThrow(ItemEntity item, Vector3 from, Vector3 to, int itemObjectId, ItemDropVisualKind kind)
    {
        SetSpinEnabled(item, false);
        item.transform.position = from;

        ItemDropPose.TryBuildThrowPose(item, _grp.ResolveGrp(item.ItemId), _grp.IsStickWeapon(item.ItemId), from, to, out ItemDropPose.ThrowPose pose);
        Vector3 throwDir = to - from;
        if (pose.TumbleInFlight || pose.StickOnLand)
        {
            Vector3 flat = new Vector3(throwDir.x, 0f, throwDir.z);
            Vector3 fly = flat.sqrMagnitude > 0.0001f
                ? (flat.normalized + Vector3.up * 0.4f).normalized
                : Vector3.forward;
            if (!_weapons.AlignBladeAlong(item.transform, fly))
                item.transform.rotation = Quaternion.LookRotation(fly, Vector3.up);
        }

        float duration = pose.StickOnLand
            ? ItemDropPresentationIds.WeaponThrowArcSeconds
            : ItemDropPresentationIds.DropThrowArcSeconds;

        _throws.Add(new ThrowArcState
        {
            ItemObjectId = itemObjectId,
            Item = item,
            From = from,
            To = to,
            Elapsed = 0f,
            Duration = Mathf.Max(0.05f, duration),
            Kind = kind,
            TumbleAxis = pose.TumbleAxis,
            TumbleDegPerSec = pose.TumbleDegPerSec,
            LandEuler = pose.LandEuler,
            StickOnLand = pose.StickOnLand,
            ArcHeight = pose.ArcHeightMeters
        });
    }

    void LandItem(ItemEntity item, Vector3 landPos, int itemObjectId, ItemDropVisualKind kind)
    {
        LandItem(item, landPos, itemObjectId, kind, null);
    }

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
