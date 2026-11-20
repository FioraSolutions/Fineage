using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Collections;

public partial class TargetWindow
{
    bool TryAddEffectIcon(int skillId, int level, int durationSeconds)
    {
        int safeLevel = level > 0 ? level : 1;
        Skillgrp skill = TablePacketServices.SkillGrps != null
            ? TablePacketServices.SkillGrps.GetSkill(skillId, safeLevel)
            : null;
        SkillNameData nameData = TablePacketServices.SkillNames != null
            ? TablePacketServices.SkillNames.GetName(skillId, safeLevel)
            : null;

        string tip = BuildPropertyTooltip(nameData, skillId, safeLevel);
        L2AbnormalEffectIconView.BuiltIcon built = L2AbnormalEffectIconView.TryCreate(
            skillId, safeLevel, skill, tip, durationSeconds);
        if (built == null)
            return false;

        _effectsRow.Add(built.Root);
        _effectCells.Add(new TargetEffectCell
        {
            Root = built.Root,
            Timer = built.Timer,
            EndsAt = Time.time + Mathf.Max(0, durationSeconds)
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
