using UnityEngine;

/// <summary>
/// Wall-clock orb launch tied to SkillCastSound RELEASE (known-good timer).
/// Composite AnimationShoot / CastClip often never reach V2 shoot subscribers.
/// </summary>
public static class MagicSkillOrbLaunch
{
    public static void TryForceAtRelease(Entity caster, int skillId)
    {
        if (caster == null)
        {
            Debug.LogWarning("[ShotOrb] RELEASE force abort caster=null skill=" + skillId);
            return;
        }

        CompositeEffectV2[] composites =
            caster.GetComponentsInChildren<CompositeEffectV2>(true);
        Debug.Log(
            "[ShotOrb] RELEASE force skill=" + skillId +
            " caster=" + caster.name +
            " composites=" + composites.Length);
        if (composites.Length == 0)
        {
            return;
        }

        for (int i = 0; i < composites.Length; i++)
        {
            CompositeEffectV2 composite = composites[i];
            if (composite == null)
            {
                continue;
            }

            composite.ForceShootNow("sfx-release");
        }
    }
}
