using System.Collections.Generic;
using UnityEngine;
using VContainer;
using System;

public sealed partial class CombatFacingService
{
    public static bool IsUsingBow(Entity entity)
    {
        if (entity is PlayerEntity player)
            return CombatStyleEntity.IsRanged(player);

        if (entity != null && entity.Appearance != null)
        {
            int rhand = entity.Appearance.RHand;
            int lhand = entity.Appearance.LHand;
            if (CombatStyleEntity.ResolveFromItemId(rhand).IsRanged)
                return true;
            if (CombatStyleEntity.ResolveFromItemId(lhand).IsRanged)
                return true;
        }

        return CombatStyleEntity.IsRanged(entity);
    }

    public static bool IsPlayerUsingBow(PlayerEntity player)
    {
        return player != null && CombatStyleEntity.IsRanged(player);
    }
}