using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EntityActionSkill
{
    static bool IsSkillAnimationNone(MagicSkillUseDto magic)
    {
        if (magic == null)
            return true;
        if (magic.SkillGrp == null)
            return true;
        string anim = magic.SkillGrp.Animation;
        return string.IsNullOrWhiteSpace(anim) ||
               string.Equals(anim, "None", StringComparison.OrdinalIgnoreCase);
    }

    static AnimationCombo GetCombo(MagicSkillUseDto magic)
    {
        if (magic == null || TablePacketServices.SkillGrps == null)
            return null;
        return TablePacketServices.SkillGrps.GetAnimComboBySkillId(magic.SkillId, magic.SkillLvl);
    }

    static AnimationEventsBase ResolveEvents(Entity entity)
    {
        if (entity == null || entity.Identity == null || ControllerPacketServices.Animations == null)
            return null;
        return ControllerPacketServices.Animations.GetAnimationEvents(entity.Identity.Id);
    }

    static bool IsMagicSkill(MagicSkillUseDto magic)
    {
        return magic != null && magic.SkillGrp != null && magic.SkillGrp.IsMagic == 1;
    }
}
