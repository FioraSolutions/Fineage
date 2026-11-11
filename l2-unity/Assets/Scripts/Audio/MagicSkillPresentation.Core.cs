using UnityEngine;

public static partial class MagicSkillPresentation
{
    public static MagicSkillUseKind Classify(MagicSkillUseDto dto)
    {
        if (dto == null)
            return MagicSkillUseKind.Cast;
        if (WeaponChargeSkill.IsCharge(dto.SkillId))
            return MagicSkillUseKind.Charge;
        if (SetupDurationHelper.IsUsePotion(dto))
            return MagicSkillUseKind.Potion;
        return MagicSkillUseKind.Cast;
    }
}
