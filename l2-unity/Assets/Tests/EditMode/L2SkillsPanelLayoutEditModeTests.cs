using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class L2SkillsPanelLayoutEditModeTests
{
    [SetUp]
    public void SetUp() => InterludeMagicSkillWndLayout.ResetForEditModeTests();

    [Test]
    public void Interlude_layout_config_defines_six_columns_per_row()
    {
        InterludeMagicSkillWndLayout.EnsureLoaded();
        Assert.AreEqual(6, InterludeMagicSkillWndLayout.ColumnsPerRow);
        Assert.AreEqual(6, L2SkillsPanelLayout.ResolveGridColumns());
    }

    [Test]
    public void ComputePanelWidth_matches_six_column_interlude_row()
    {
        float width = L2SkillsPanelLayout.ComputePanelWidth(6);
        Assert.AreEqual(216f, width, 0.01f);
    }

    [Test]
    public void RowFrameResource_uses_configured_interlude_skill_bar_texture()
    {
        Assert.AreEqual(
            "Data/UI/Window/Skills/Skills_SkillBar_6x1",
            L2SkillsPanelLayout.RowFrameResourceForColumns(6));
    }

    [Test]
    public void ApplyMagicSkillTabHostLayout_clears_inventory_one_pixel_tab_host()
    {
        var tabTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Game/Inventory/InventoryTab");
        Assert.NotNull(tabTemplate, "InventoryTab.uxml missing");

        var tabRoot = tabTemplate.Instantiate();
        var tabHost = tabRoot.Q<VisualElement>("Tab") ?? tabRoot[0];
        tabHost.style.height = 1;
        tabHost.Clear();
        tabHost.Add(new VisualElement { name = "Content" });

        L2SkillsPanelLayout.ApplyMagicSkillTabHostLayout(tabHost);
        Assert.AreEqual(StyleKeyword.Auto, tabHost.style.height.keyword);
    }

    [Test]
    public void ApplyGridRowMetrics_sets_interlude_row_height()
    {
        var row = new VisualElement();
        L2SkillsPanelLayout.ApplyGridRowMetrics(row);
        Assert.GreaterOrEqual(
            L2SkillsPanelLayout.ReadConfiguredRowHeight(row),
            L2SkillsPanelLayout.RowHeight - 0.01f);
    }

    [Test]
    public void ResolveGridColumns_ignores_hidden_tab_width_uses_fixed_six()
    {
        var tabHost = new VisualElement();
        tabHost.AddToClassList("unselected-tab");
        tabHost.style.width = 1;

        Assert.AreEqual(6, L2SkillsPanelLayout.ResolveGridColumns());
    }
}