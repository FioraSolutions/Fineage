using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemDropPresentationService
{
    void PlayEffect(int effectId, Transform target, string label)
    {
        if (effectId <= 0 || target == null)
        {
            // Debug.Log($"[ItemDropFx] effect stub ({label}) id={effectId} — assign in ItemDropPresentationIds");
            return;
        }

        EffectManager effects = Effects;
        if (effects == null)
            return;

        Transform playTarget = target;
        if (NeedsLandBurstHeight(effectId))
        {
            var anchor = new GameObject("HitPointProxy");
            Vector3 grounded = WorldPlacement.Resolve(WorldPlacement.PlacementKind.ItemDrop, target.position);
            if (ItemDropPresentationIds.LandBurstLocalY != 0f)
                grounded += Vector3.up * ItemDropPresentationIds.LandBurstLocalY;
            anchor.transform.SetParent(target, true);
            anchor.transform.position = grounded;
            playTarget = anchor.transform;
        }

        effects.PlayEffect(effectId, playTarget);
    }

    void PlayEffect(int effectId, Vector3 point, string label)
    {
        if (effectId <= 0)
        {
            // Debug.Log($"[ItemDropFx] effect stub ({label}) id={effectId} — assign in ItemDropPresentationIds");
            return;
        }

        EffectManager effects = Effects;
        if (effects == null)
            return;

        // Parent must outlive the FX. EffectManager.PlayEffect instantiates as a child
        // of target; BaseEffect.DestoryEffect also destroys a HitPointProxy parent.
        var anchor = new GameObject("HitPointProxy");
        Vector3 spawn = WorldPlacement.Resolve(WorldPlacement.PlacementKind.ItemDrop, point);
        if (NeedsLandBurstHeight(effectId) && ItemDropPresentationIds.LandBurstLocalY != 0f)
            spawn += Vector3.up * ItemDropPresentationIds.LandBurstLocalY;
        anchor.transform.SetPositionAndRotation(spawn, Quaternion.identity);
        effects.PlayEffect(effectId, anchor.transform);
    }
}
