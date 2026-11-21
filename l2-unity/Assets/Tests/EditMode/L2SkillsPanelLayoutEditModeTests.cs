using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

public sealed class L2SkillsPanelLayoutEditModeTests
{
    [Test]
    public void ComputeColumns_fits_six_slots_in_mainwnd_skills_host()
    {
        // CharacterInfo #SkillsHost content width after skills chrome padding (~248px).
        Assert.AreEqual(6, L2SkillsPanelLayout.ComputeColumns(248f));
    }

    [Test]
    public void ComputeColumns_uses_default_when_tab_host_reports_one_pixel()
    {
        Assert.AreEqual(L2SkillsPanelLayout.DefaultColumns, L2SkillsPanelLayout.ComputeColumns(1f));
    }

    [Test]
    public void ResolveGridColumns_reads_skills_panel_when_passive_tab_is_display_none()
    {
        var skillsPanel = new VisualElement();
        skillsPanel.AddToClassList("skills-panel");
        skillsPanel.style.width = 280;

        var tabHost = new VisualElement();
        tabHost.AddToClassList("unselected-tab");
        skillsPanel.Add(tabHost);

        var content = new VisualElement { name = "Content" };
        tabHost.Add(content);

        var passiveRoot = new VisualElement();
        content.Add(passiveRoot);
        passiveRoot.style.width = 1;

        int columns = L2SkillsPanelLayout.ResolveGridColumns(passiveRoot);
        Assert.GreaterOrEqual(columns, L2SkillsPanelLayout.DefaultColumns);
    }

    [Test]
    public void ComputePanelWidth_matches_six_column_interlude_row()
    {
        float width = L2SkillsPanelLayout.ComputePanelWidth(6);
        Assert.AreEqual(216f, width, 0.01f);
    }

    [Test]
    public void RowFrameResource_uses_6x1_art_for_six_or_fewer_columns()
    {
        Assert.AreEqual(L2SkillsPanelLayout.RowFrame6, L2SkillsPanelLayout.RowFrameResourceForColumns(6));
        Assert.AreEqual(L2SkillsPanelLayout.RowFrame7, L2SkillsPanelLayout.RowFrameResourceForColumns(7));
    }

    [Test]
    public void ApplyMagicSkillTabHostLayout_clears_inventory_one_pixel_tab_host()
    {
        var tabTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Game/Inventory/InventoryTab");
        Assert.NotNull(tabTemplate, "InventoryTab.uxml missing");

        var tabHost = tabTemplate.Instantiate()[0];
        tabHost.Clear();
        tabHost.Add(new VisualElement { name = "Content" });
        Assert.AreEqual(1f, tabHost.style.height.value.value, 0.01f);

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
}