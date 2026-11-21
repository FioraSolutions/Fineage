using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Collections;

public partial class TargetWindow
{
    bool TryAddEffectIcon(int skillId, int wireValue, int durationSeconds)
    {
        int safeLevel = AbnormalEffectTooltipBuilder.ResolveSkillLevel(skillId, wireValue);
        Skillgrp skill = TablePacketServices.SkillGrps != null
            ? TablePacketServices.SkillGrps.GetSkill(skillId, safeLevel)
            : null;
        SkillNameData nameData = TablePacketServices.SkillNames != null
            ? TablePacketServices.SkillNames.GetName(skillId, safeLevel)
            : null;

        var icon = new VisualElement();
        icon.AddToClassList("l2-abnormal-icon");
        icon.AddToClassList("target-effect-icon");

        bool painted = skill != null && L2SlotIconView.TrySetNamedIcon(icon, skill.Icon);
        if (!painted)
            painted = L2SlotIconView.TrySetNamedIcon(icon, "icon.skill" + skillId);
        if (!painted)
            painted = L2SlotIconView.TrySetNamedIcon(icon, "skill" + skillId);
        if (!painted && nameData == null)
            return false;

        // Interlude: debuffs use skillgrp icon_panel (typically panel_2) — buffs leave it None.
        bool isDebuff = skill != null && skill.Debuff != 0;
        if (isDebuff)
            icon.AddToClassList("debuff");
        if (skill != null && !string.IsNullOrEmpty(skill.IconPanel) &&
            !skill.IconPanel.Equals("None", System.StringComparison.OrdinalIgnoreCase))
        {
            var panel = new VisualElement();
            panel.AddToClassList("target-effect-panel");
            panel.pickingMode = PickingMode.Ignore;
            if (L2SlotIconView.TrySetNamedIcon(panel, skill.IconPanel))
                icon.Add(panel);
        }

        // Retail TargetStatusWnd abnormal strip: icons only — no stack badge or per-second overlay (the
        // old target-effect-timer showed wire duration and read as a bogus "x3" stack count in playtests).
        int remaining = Mathf.Max(0, durationSeconds);
        var tipManipulator = new TargetAbnormalTooltipManipulator(icon, skillId, wireValue, remaining);
        icon.AddManipulator(tipManipulator);

        _effectsRow.Add(icon);
        _effectCells.Add(new TargetEffectCell
        {
            Root = icon,
            Tooltip = tipManipulator,
            EndsAt = Time.time + remaining
        });
        return true;
    }

    static int EffectsSignature(System.Collections.Generic.List<EffectHolder> list)
    {
        if (list == null || list.Count == 0)
            return 0;
        unchecked
        {
            int hash = list.Count;
            for (int i = 0; i < list.Count; i++)
            {
                EffectHolder e = list[i];
                if (e == null)
                    continue;
                hash = (hash * 397) ^ e.Id;
                hash = (hash * 397) ^ e.Value;
                hash = (hash * 397) ^ e.Duration;
            }
            return hash;
        }
    }

    void ClearEffects()
    {
        _effectsObjectId = -1;
        _effectsSignature = 0;
        _effectCells.Clear();
        if (_effectsRow == null)
        {
            RefreshPlateHeight();
            return;
        }
        _effectsRow.Clear();
        _effectsRow.style.display = DisplayStyle.None;
        RefreshPlateHeight();
    }
}
