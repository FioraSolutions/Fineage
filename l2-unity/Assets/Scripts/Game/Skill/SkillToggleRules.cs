/// <summary>
/// Interlude <c>skillgrp</c> toggle classification: <c>operate_type=3</c> ("T") is a toggle
/// (Fake Death 60, Silent Move 221, Relax 226, Vicious Stance 312, Focus Skill Mastery 334, …).
/// <para>
/// The original client marks a toggle as running by drawing the <c>l2_skilltime.ToggleEffect</c> shine over
/// its icon on the shortcut bar and in the skill window; the server tells it through the abnormal list.
/// </para>
/// </summary>
public static class SkillToggleRules
{
    /// <summary><c>skillgrp operate_type</c> for a toggle skill.</summary>
    public const int ToggleOperateType = 3;

    public static bool IsToggle(int skillId, int level)
    {
        if (skillId <= 0)
            return false;
        ISkillgrpLookup table = TablePacketServices.SkillGrps;
        if (table == null)
            return false;
        Skillgrp skill = table.GetSkill(skillId, level > 0 ? level : 1);
        return skill != null && skill.OperateType == ToggleOperateType;
    }

    /// <summary>Toggle check for a packet: the skill level travels with it, so no nearest-level guess is needed.</summary>
    public static bool IsToggle(int skillId, int level, Skillgrp fallback)
    {
        if (fallback != null)
            return fallback.OperateType == ToggleOperateType;
        return IsToggle(skillId, level);
    }
}
