using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static partial class SkillCastSound
{
    static bool TryGetRow(int skillId, int level, out SkillSoundRow row)
    {
        row = default;
        if (_bySkillLevel != null && _bySkillLevel.TryGetValue(Pack(skillId, Mathf.Max(1, level)), out row))
            return true;
        if (_bySkill != null && _bySkill.TryGetValue(skillId, out row))
            return true;
        return false;
    }

    static long Pack(int skillId, int level) => ((long)skillId << 16) | (uint)level;

    static string KeyFor(int skillId, int level)
    {
        string fromId;
        if (SkillIdSfx.TryGetValue(skillId, out fromId))
            return fromId;

        string name = null;
        try
        {
            SkillNameTable table = TablePacketServices.SkillNames;
            SkillNameData data = table != null ? table.GetName(skillId, Mathf.Max(1, level)) : null;
            if (data == null && table != null)
                data = table.GetName(skillId, 1);
            name = data != null ? data.Name : null;
        }
        catch { }

        string normalized = Normalize(name);
        if (string.IsNullOrEmpty(normalized))
            return null;

        if (normalized == "battle_heal" || normalized == "group_heal"
            || normalized == "elemental_heal" || normalized == "self_heal"
            || normalized == "servitor_heal" || normalized.EndsWith("_heal")
            || normalized.StartsWith("heal_"))
            return "heal";
        if (normalized == "greater_battle_heal" || normalized == "greater_group_heal"
            || normalized.Contains("greater_heal"))
            return "greater_heal";

        if (normalized.Contains("heal") && Load(SfxRoot, normalized + "_cast") == null)
            return "heal";

        return normalized;
    }

    public static string Normalize(string skillName)
    {
        if (string.IsNullOrEmpty(skillName))
            return null;
        var sb = new StringBuilder(skillName.Length);
        bool us = false;
        foreach (char ch in skillName.ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(ch)) { sb.Append(ch); us = false; }
            else if (ch == '\'') { }
            else if (!us && sb.Length > 0) { sb.Append('_'); us = true; }
        }
        return sb.ToString().TrimEnd('_');
    }
}
