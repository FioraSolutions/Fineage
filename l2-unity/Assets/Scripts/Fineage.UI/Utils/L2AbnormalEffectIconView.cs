using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Builds abnormal/buff icons with the same slot chrome as BufferPanel (status) cells.
/// </summary>
public static class L2AbnormalEffectIconView
{
    public sealed class BuiltIcon
    {
        public VisualElement Root { get; init; }
        public Label Timer { get; init; }
    }

    public static BuiltIcon TryCreate(
        int skillId,
        int level,
        Skillgrp skill,
        string tooltip,
        int durationSeconds)
    {
        int safeLevel = level > 0 ? level : 1;
        if (skill == null && TablePacketServices.SkillGrps != null)
            skill = TablePacketServices.SkillGrps.GetSkill(skillId, safeLevel);

        var root = new VisualElement();
        root.AddToClassList("l2-slot");
        root.AddToClassList("skillbar-slot");
        root.AddToClassList("abnormal-status-icon");
        root.pickingMode = PickingMode.Position;

        var bgContainer = new VisualElement();
        bgContainer.AddToClassList("slot-bg-container");
        bgContainer.pickingMode = PickingMode.Ignore;

        var slotBg = new VisualElement();
        slotBg.AddToClassList("slot-bg");
        slotBg.pickingMode = PickingMode.Ignore;

        bool painted = skill != null && L2SlotIconView.TrySetNamedIcon(slotBg, skill.Icon);
        if (!painted)
            painted = L2SlotIconView.TrySetNamedIcon(slotBg, "icon.skill" + skillId);
        if (!painted)
            painted = L2SlotIconView.TrySetNamedIcon(slotBg, "skill" + skillId);
        if (!painted)
            return null;

        bool isDebuff = skill != null && skill.Debuff != 0;
        if (isDebuff)
            root.AddToClassList("debuff");

        if (skill != null && !string.IsNullOrEmpty(skill.IconPanel) &&
            !skill.IconPanel.Equals("None", System.StringComparison.OrdinalIgnoreCase))
        {
            var panel = new VisualElement();
            panel.AddToClassList("abnormal-status-panel");
            panel.pickingMode = PickingMode.Ignore;
            if (L2SlotIconView.TrySetNamedIcon(panel, skill.IconPanel))
                slotBg.Add(panel);
        }

        bgContainer.Add(slotBg);
        root.Add(bgContainer);

        var timer = new Label();
        timer.AddToClassList("abnormal-status-timer");
        timer.pickingMode = PickingMode.Ignore;
        int remaining = Mathf.Max(0, durationSeconds);
        timer.text = remaining > 0 ? remaining.ToString() : "";
        root.Add(timer);

        if (!string.IsNullOrEmpty(tooltip))
            root.AddManipulator(new TooltipManipulator(root, tooltip));

        return new BuiltIcon { Root = root, Timer = timer };
    }
}