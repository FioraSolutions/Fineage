using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static partial class SkillCastSound
{
    public static void OnMagicSkillLaunched(Entity caster, MagicSkillLaunchedDto dto)
    {
        // Launched is often cast-release on L2 packs — explode is projectile impact.
    }

    public static void OnProjectileImpact(int attackerEntityId, Vector3 hitPoint)
    {
        if (attackerEntityId <= 0)
            return;
        TryPlayExplode(attackerEntityId, hitPoint, "projectile");
    }
}
