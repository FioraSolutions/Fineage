using System.Text;
using UnityEngine;

/// <summary>
/// Interlude-style abnormal icon tooltips: name, resolved level, description, optional remaining seconds.
/// Shared by BuffWnd (BufferPanel) and target abnormal strips.
/// </summary>
public static class AbnormalEffectTooltipBuilder
{
    /// <summary>
    /// Wire <c>value</c> is skill level when that row exists in skillgrp; otherwise fall back to 1.
    /// </summary>
    public static int ResolveSkillLevel(int skillId, int wireValue)
    {
        if (skillId <= 0)
            return 1;
        if (wireValue <= 0)
            return 1;
        ISkillgrpLookup table = TablePacketServices.SkillGrps;
        if (table != null && table.GetSkill(skillId, wireValue) != null)
            return wireValue;
        return 1;
    }

    public static string Build(
        int skillId,
        int wireValue,
        int remainingSeconds,
        bool includeRemainingTime = true)
    {
        int level = ResolveSkillLevel(skillId, wireValue);
        SkillNameData nameData = TablePacketServices.SkillNames?.GetName(skillId, level);
        Skillgrp skill = TablePacketServices.SkillGrps?.GetSkill(skillId, level);

        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(nameData?.Name))
            sb.Append(nameData.Name);
        else
            sb.Append("Skill ").Append(skillId);

        sb.Append(' ').Append("Lv.").Append(level);

        string desc = nameData?.Desc;
        if (!string.IsNullOrEmpty(desc))
        {
            sb.Append('\n');
            sb.Append(desc);
        }

        if (includeRemainingTime && remainingSeconds > 0)
        {
            sb.Append('\n');
            sb.Append(remainingSeconds).Append('s');
        }

        return sb.ToString();
    }

    /// <summary>
    /// Charge stacks (e.g. Sonic Focus): show count on the buff cell label; Interlude has no "x" prefix.
    /// </summary>
    public static bool TryFormatChargeLabel(int skillId, int wireValue, out string label)
    {
        label = null;
        if (wireValue <= 1)
            return false;
        Skillgrp skill = TablePacketServices.SkillGrps?.GetSkill(skillId, ResolveSkillLevel(skillId, wireValue));
        if (skill == null)
            return false;
        // operate_type 6 = charge / force on many Interlude skillgrp rows (Sonic Focus family).
        if (skill.OperateType != 6)
            return false;
        label = wireValue.ToString();
        return true;
    }
}