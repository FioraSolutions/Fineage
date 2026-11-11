using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;

public partial class TargetWindow
{
    bool TryAddPropertyIcon(int skillId, int level)
    {
        int resolvedId = skillId;
        if (ClassicPropertySkillRemap.TryGetValue(skillId, out int remapped))
            resolvedId = remapped;

        Skillgrp skill = SkillGrps != null
            ? SkillGrps.GetSkill(resolvedId, level)
            : null;
        SkillNameData nameData = SkillNames != null
            ? SkillNames.GetName(resolvedId, level)
            : null;

        var icon = new VisualElement();
        icon.AddToClassList("target-property-icon");

        bool painted = skill != null && L2SlotIconView.TrySetNamedIcon(icon, skill.Icon);
        if (!painted)
            painted = L2SlotIconView.TrySetNamedIcon(icon, "icon.skill" + resolvedId);
        if (!painted)
            painted = L2SlotIconView.TrySetNamedIcon(icon, "skill" + resolvedId);

        // Skip Classic/unknown property rows that would only show NO IMAGE.
        if (!painted)
            return false;

        string tip = BuildPropertyTooltip(nameData, resolvedId, level);
        if (!string.IsNullOrEmpty(tip))
            icon.AddManipulator(new TooltipManipulator(icon, tip));

        _propertiesRow.Add(icon);
        return true;
    }

    static string BuildPropertyTooltip(SkillNameData nameData, int skillId, int level)
    {
        if (nameData == null)
            return "Skill " + skillId + " Lv." + level;

        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(nameData.Name))
            sb.Append(nameData.Name);
        if (!string.IsNullOrEmpty(nameData.Desc))
        {
            if (sb.Length > 0)
                sb.Append('\n');
            sb.Append(nameData.Desc);
        }
        return sb.Length > 0 ? sb.ToString() : ("Skill " + skillId + " Lv." + level);
    }

    void ClearProperties()
    {
        _boundObjectId = -1;
        _boundNpcId = -1;
        if (_propertiesRow != null)
            _propertiesRow.Clear();
        if (_expandButton != null)
            _expandButton.style.display = DisplayStyle.None;
        SetExpanded(false);
    }
}
