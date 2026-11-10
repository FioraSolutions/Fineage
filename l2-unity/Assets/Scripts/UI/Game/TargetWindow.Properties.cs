using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public partial class TargetWindow
{
    /// <summary>Classic npcgrp property_list ids → Interlude skillgrp rows with real icons.</summary>
    static readonly System.Collections.Generic.Dictionary<int, int> ClassicPropertySkillRemap =
        new System.Collections.Generic.Dictionary<int, int>
        {
            // Gray Wolf etc.: animalWeakness + bow → Animal Vuln / Bow Vuln.
            { 47269, 4071 },
            { 47270, 4276 },
        };

    void OnExpandPressed(MouseDownEvent evt)
    {
        evt.StopPropagation();
        SetExpanded(!_expanded);
    }

    void SetExpanded(bool expanded)
    {
        _expanded = expanded;
        if (_windowEle == null)
            return;

        if (_expanded)
        {
            _windowEle.AddToClassList("expanded");
            if (_expandButton != null)
                _expandButton.AddToClassList("expanded");
            if (_propertiesRow != null)
                _propertiesRow.style.display = DisplayStyle.Flex;
        }
        else
        {
            _windowEle.RemoveFromClassList("expanded");
            if (_expandButton != null)
                _expandButton.RemoveFromClassList("expanded");
            if (_propertiesRow != null)
                _propertiesRow.style.display = DisplayStyle.None;
        }
    }

    void BindProperties(TargetData targetData)
    {
        if (targetData == null || targetData.Identity == null)
        {
            ClearProperties();
            return;
        }

        int objectId = targetData.Identity.Id;
        int npcId = targetData.Identity.NpcId;
        if (objectId == _boundObjectId && npcId == _boundNpcId)
            return;

        _boundObjectId = objectId;
        _boundNpcId = npcId;

        if (_propertiesRow != null)
            _propertiesRow.Clear();

        Npcgrp npcgrp = npcId > 0 && TablePacketServices.NpcGrps != null
            ? TablePacketServices.NpcGrps.GetNpcgrp(npcId)
            : null;
        int[] skillIds = npcgrp != null ? npcgrp.PropertySkillIds : null;
        int[] skillLevels = npcgrp != null ? npcgrp.PropertySkillLevels : null;

        bool hasProps = skillIds != null && skillLevels != null
            && skillIds.Length > 0 && skillIds.Length == skillLevels.Length;

        if (!hasProps || _propertiesRow == null)
        {
            if (_expandButton != null)
                _expandButton.style.display = DisplayStyle.None;
            SetExpanded(false);
            return;
        }

        int iconsAdded = 0;
        for (int i = 0; i < skillIds.Length; i++)
        {
            if (TryAddPropertyIcon(skillIds[i], skillLevels[i]))
                iconsAdded++;
        }

        if (iconsAdded == 0)
        {
            if (_expandButton != null)
                _expandButton.style.display = DisplayStyle.None;
            SetExpanded(false);
            return;
        }

        if (_expandButton != null)
            _expandButton.style.display = DisplayStyle.Flex;

        // Keep prior expand preference when retargeting another mob with properties.
        if (_expanded)
            _propertiesRow.style.display = DisplayStyle.Flex;
        else
            _propertiesRow.style.display = DisplayStyle.None;
    }

    bool TryAddPropertyIcon(int skillId, int level)
    {
        int resolvedId = skillId;
        if (ClassicPropertySkillRemap.TryGetValue(skillId, out int remapped))
            resolvedId = remapped;

        Skillgrp skill = TablePacketServices.SkillGrps != null
            ? TablePacketServices.SkillGrps.GetSkill(resolvedId, level)
            : null;
        SkillNameData nameData = TablePacketServices.SkillNames != null
            ? TablePacketServices.SkillNames.GetName(resolvedId, level)
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
