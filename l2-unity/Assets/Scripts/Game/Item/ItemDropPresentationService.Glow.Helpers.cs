using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemDropPresentationService
{
    void StopLandBurst(int itemObjectId)
    {
        if (!_landBurstByItemId.TryGetValue(itemObjectId, out GameObject anchor))
            return;

        _landBurstByItemId.Remove(itemObjectId);
        if (anchor != null)
            UnityEngine.Object.Destroy(anchor);
    }

    static bool NeedsLandBurstHeight(int effectId)
    {
        return effectId == ItemDropPresentationIds.LandBurstEffectId
            || effectId == ItemDropPresentationIds.CoinSparkleEffectId;
    }

    static int ResolveFallEffectId(ItemDropVisualKind kind)
    {
        switch (kind)
        {
            case ItemDropVisualKind.Weapon:
                return ItemDropPresentationIds.WeaponFallTrailEffectId;
            case ItemDropVisualKind.EtcStackable:
                return ItemDropPresentationIds.CoinSparkleEffectId;
            default:
                return 0;
        }
    }

    static int ResolveGroundGlowEffectId(ItemDropVisualKind kind)
    {
        return ItemDropPresentationIds.GroundGlowEffectId;
    }
}
