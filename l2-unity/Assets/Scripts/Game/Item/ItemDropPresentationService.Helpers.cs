using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class ItemDropPresentationService
{
    public void PlayDrop(ItemEntity item, Vector3 landPos, int dropperCharObjId)
    {
        if (item == null)
            return;

        int itemObjectId = item.Identity != null ? item.Identity.Id : 0;
        ItemDropVisualKind kind = ResolveVisualKind(item);
        Entity dropper = ResolveEntity(dropperCharObjId);
        landPos = WorldPlacement.Resolve(WorldPlacement.PlacementKind.ItemDrop, landPos);

        if (dropper != null)
            PlayActorAnimation(dropperCharObjId, ItemDropPresentationIds.DropAnimationTrigger);

        int fallEffectId = ResolveFallEffectId(kind);
        if (fallEffectId > 0 && fallEffectId != ItemDropPresentationIds.LandBurstEffectId)
        {
            PlayEffect(
                fallEffectId,
                dropper != null ? dropper.transform : item.transform,
                "fall-trail");
        }

        // Adena: FX toss, no mesh throw. Everything else flies from the hand
        // (or from a short hover if the packet has no dropper).
        bool throwMesh = kind != ItemDropVisualKind.Adena;
        if (throwMesh)
        {
            Vector3 from = ResolveThrowStart(dropper, landPos);
            StopThrow(itemObjectId);
            BeginThrow(item, from, landPos, itemObjectId, kind);
        }
        else
        {
            LandItem(item, landPos, itemObjectId, kind);
        }

    }

    public void StopItemPresentation(int itemObjectId)
    {
        StopThrow(itemObjectId);
        StopGroundGlow(itemObjectId);
        StopLandBurst(itemObjectId);
        CancelCoinMeshReveal(itemObjectId);
    }

    public void PlayPickup(int itemObjectId, int pickerCharObjId, Vector3 unityPos)
    {
        StopItemPresentation(itemObjectId);
        PlayActorCrossFade(pickerCharObjId, ItemDropPresentationIds.PickupAnimationState);
        PlayEffect(ItemDropPresentationIds.PickupBurstEffectId, unityPos, "pickup-burst");
    }
}
