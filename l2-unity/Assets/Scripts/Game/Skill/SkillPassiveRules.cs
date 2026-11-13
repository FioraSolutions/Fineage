/// <summary>
/// Interlude skillgrp passive classification for SkillList Active/Passive tabs.
/// Re-resolves from DAT so a SkillList that arrived before skillgrp load cannot stick as Active.
/// </summary>
public static class SkillPassiveRules
{
    /// <summary>
    /// <c>operate_type=2</c> = passive; <c>icon_type=16</c> = weapon Special Ability (always passive).
    /// </summary>
    public static bool IsPassive(int skillId, int level)
    {
        if (TablePacketServices.SkillGrps == null)
            return false;
        Skillgrp skill = TablePacketServices.SkillGrps.GetSkill(skillId, level);
        if (skill == null)
            return false;
        return skill.OperateType == 2 || skill.Icon_type == 16;
    }

    public static void Refresh(SkillInstance skill)
    {
        if (skill == null)
            return;
        skill.IsPassive = IsPassive(skill.SkillID, skill.Level);
    }
}
