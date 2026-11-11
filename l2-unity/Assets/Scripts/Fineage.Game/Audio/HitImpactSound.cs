using UnityEngine;

/// <summary>Armor / crit hit SFX via <see cref="InterludeSfx"/>.</summary>
public static class HitImpactSound
{
    static int _armorIdx;

    public static bool Play(bool criticalHit)
    {
        AudioClip clip;
        if (criticalHit)
        {
            clip = InterludeSfx.Load("Data/Sounds/SkillSound/critical_hit")
                ?? InterludeSfx.Load("Data/Sounds/SkillSound/critical_hit_stereo")
                ?? InterludeSfx.Load("Data/Sounds/SkillSound/critical_hit_02");
        }
        else
        {
            _armorIdx = (_armorIdx % 4) + 1;
            clip = InterludeSfx.Load("Data/Sounds/ItemSound/armor_underwear_" + _armorIdx)
                ?? InterludeSfx.Load("Data/Sounds/ItemSound/armor_underwear_1");
        }

        if (clip == null)
            return false;

        InterludeSfx.Play(clip, 0.75f);
        return true;
    }
}
