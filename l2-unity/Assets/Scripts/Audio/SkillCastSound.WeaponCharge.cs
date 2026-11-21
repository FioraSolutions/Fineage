using UnityEngine;

public static partial class SkillCastSound
{
    /// <summary>Interlude weapon charge: skillsoundgrp cast at MagicSkillUse (charge on weapon).</summary>
    public static void PlayWeaponChargeActivation(int skillId, int skillLevel, Vector3 worldPos)
    {
        if (!WeaponChargeSkill.IsCharge(skillId))
            return;

        EnsureGrpLoaded();
        InterludeSfx.PlaceAtCaster(worldPos, true);

        if (!TryGetRow(skillId, skillLevel > 0 ? skillLevel : 1, out SkillSoundRow row))
        {
            Debug.LogError("[WeaponCharge] SkillSoundGrp missing skillId=" + skillId + " level=" + skillLevel);
            return;
        }

        if (string.IsNullOrEmpty(row.Cast))
        {
            Debug.LogError("[WeaponCharge] SkillSoundGrp empty cast skillId=" + skillId + " level=" + skillLevel);
            return;
        }

        PlayCastSfx(row.Cast, Volume);
    }

    /// <summary>Interlude weapon charge: skillsoundgrp shot when the charged hit lands (AttackShot consume).</summary>
    public static void PlayWeaponChargeImpact(int skillId, int skillLevel, Vector3 worldPos)
    {
        if (!WeaponChargeSkill.IsCharge(skillId))
            return;

        EnsureGrpLoaded();
        InterludeSfx.PlaceAtCaster(worldPos, true);

        if (!TryGetRow(skillId, skillLevel > 0 ? skillLevel : 1, out SkillSoundRow row))
        {
            Debug.LogError("[WeaponCharge] SkillSoundGrp missing skillId=" + skillId + " level=" + skillLevel);
            return;
        }

        if (string.IsNullOrEmpty(row.Shot))
        {
            Debug.LogError("[WeaponCharge] SkillSoundGrp empty shot skillId=" + skillId + " level=" + skillLevel);
            return;
        }

        PlayCastSfx(row.Shot, Volume);
    }
}