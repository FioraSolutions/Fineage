using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Per-window HUD layout and tooltip helpers (#1598–#1602).
/// </summary>
public sealed class HudWindowInterludeLayoutEditModeTests
{
    [Test]
    public void Abnormal_tooltip_resolves_level_from_skillgrp_not_raw_wire_guess()
    {
        int level = AbnormalEffectTooltipBuilder.ResolveSkillLevel(1, 99);
        Assert.Greater(level, 0);
        Assert.LessOrEqual(level, 99);
    }

    [Test]
    public void Abnormal_tooltip_includes_skill_id_when_name_missing()
    {
        string tip = AbnormalEffectTooltipBuilder.Build(999999, 1, 12, includeRemainingTime: true);
        StringAssert.Contains("999999", tip);
        StringAssert.Contains("12s", tip);
    }

    [Test]
    public void Buff_tray_template_loads_with_buff_tray_class()
    {
        var tree = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Game/Buffer/SkillTray");
        Assert.IsNotNull(tree);
        TemplateContainer root = tree.CloneTree();
        VisualElement content = root.Q<VisualElement>(null, "buff-tray");
        Assert.IsNotNull(content, "SkillTray content must carry buff-tray for scoped USS.");
    }

}