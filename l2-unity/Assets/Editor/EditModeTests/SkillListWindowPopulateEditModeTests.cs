using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

/// <summary>
/// Skills window (MagicSkillWnd) must populate physical/magic/passive rows after SkillList.
/// </summary>
public sealed class SkillListWindowPopulateEditModeTests
{
    GameObject _tooltipHost;
    GameObject _skillListHost;

    [SetUp]
    public void SetUp()
    {
        DestroyAllSkillListWindows();
        SkillListWindow.EditModeResetSingletonForTests();
        _tooltipHost = new GameObject("EditMode_ToolTipSimple");
        var tooltip = _tooltipHost.AddComponent<ToolTipSimple>();
        ToolTipSimplePacketServices.Bind(tooltip);
    }

    static void DestroyAllSkillListWindows()
    {
        var existing = Object.FindObjectsByType<SkillListWindow>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
        for (int i = 0; i < existing.Length; i++)
            Object.DestroyImmediate(existing[i].gameObject);
    }

    [TearDown]
    public void TearDown()
    {
        if (_skillListHost != null)
            Object.DestroyImmediate(_skillListHost);
        if (_tooltipHost != null)
            Object.DestroyImmediate(_tooltipHost);
        ToolTipSimplePacketServices.Bind(window: null);
        SkillListWindow.EditModeResetSingletonForTests();
    }

    [Test]
    public void ActiveSkillsHide_CreateSlots_populates_physical_and_magic_rows()
    {
        RunPopulateScenario(applyBeforeMount: false);
    }

    [Test]
    public void ActiveSkillsHide_CreateSlots_before_mount_produces_visible_slots_after_mount()
    {
        RunPopulateScenario(applyBeforeMount: true);
    }

    [Test]
    public void SkillListWindow_apply_before_tab_build_flushes_active_and_passive_slots_on_open()
    {
        var windowAsset = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Game/SkillLearn/SkillListWindow");
        Assert.NotNull(windowAsset, "SkillListWindow.uxml missing");

        DestroyAllSkillListWindows();
        SkillListWindow.EditModeResetSingletonForTests();

        _skillListHost = new GameObject("SkillListWindowEditModeHost");
        var window = _skillListHost.AddComponent<SkillListWindow>();
        window.EditorTestEnsureAwakeForEditMode();
        Assert.IsTrue(window.IsSkillListSingletonOwnerForTests, "test host must own SkillListWindow singleton");
        window.EditorTestLoadAssets();

        var visualRoot = windowAsset.Instantiate();
        var host = new VisualElement { name = "UITestRoot" };
        host.style.width = 420;
        host.style.height = 360;
        host.Add(visualRoot);

        window.EditorTestInitWindow(host);

        var skills = new List<SkillInstance>
        {
            new SkillInstance(3, 1, false, false),
            new SkillInstance(1177, 1, false, false),
            new SkillInstance(194, 1, true, false),
        };

        LogAssert.Expect(LogType.Warning, new Regex(@"\[SkillList\].*deferred.*tabs not built"));
        window.UpdateSkillList(skills);
        Assert.Greater(window.PendingSkillCountForEditModeTests, 0, "packet before tab chrome should defer");
        Assert.AreEqual(skills.Count, window.AppliedSkillCountForEditModeTests);

        var content = window.EditorTestQueryContent();
        Assert.NotNull(content, "SkillListWindow content missing");
        VisualElement tabView = content.Q("InventoryTabView") ?? content;

        window.EditorFinishTabBuildForTests(tabView);

        Assert.AreEqual(0, window.PendingSkillCountForEditModeTests, "pending should flush after tab build");
        Assert.Greater(window.ActiveVisibleSkillSlotsForEditModeTests, 0, "active tab should expose slots");
        Assert.Greater(window.PassiveVisibleSkillSlotsForEditModeTests, 0, "passive tab should expose slots");
    }

    static void RunPopulateScenario(bool applyBeforeMount)
    {
        var activeTemplate = Resources.Load<VisualTreeAsset>("Data/UI/_Elements/Game/SkillLearn/TabActive");
        var boxRow = Resources.Load<VisualTreeAsset>(L2SkillsUiAssets.SkillsBoxRow);
        var gridRow = Resources.Load<VisualTreeAsset>(L2SkillsUiAssets.SkillsGridRow);
        var slot = Resources.Load<VisualTreeAsset>(L2SkillsUiAssets.SkillsSlot);
        Assert.NotNull(activeTemplate, "TabActive.uxml missing");
        Assert.NotNull(boxRow, "SkillsBoxRow missing");
        Assert.NotNull(gridRow, "SkillsGridRow missing");
        Assert.NotNull(slot, "SkillsSlot missing");

        var hide = new ActiveSkillsHide();
        hide.SetActiveSkillTemplate(activeTemplate, boxRow, gridRow, slot);

        var host = new VisualElement();
        host.style.width = 260;

        var activeSkills = new List<SkillInstance>
        {
            new SkillInstance(3, 1, false, false),
            new SkillInstance(1177, 1, false, false),
        };

        if (applyBeforeMount)
            hide.CreateSlots(activeSkills, 0);

        hide.GetOrCreateTab(host);

        if (!applyBeforeMount)
            hide.CreateSlots(activeSkills, 0);

        VisualElement rowPhysical = host.Q("RowPhysical");
        VisualElement rowMagical = host.Q("RowMagical");
        Assert.NotNull(rowPhysical, "RowPhysical missing");
        Assert.NotNull(rowMagical, "RowMagical missing");
        Assert.Greater(rowPhysical.childCount, 0, "physical row has no slot panels");
        Assert.AreNotEqual(DisplayStyle.None, rowPhysical.style.display.value, "physical section collapsed");

        int physicalSlots = CountSkillSlots(rowPhysical);
        Assert.Greater(physicalSlots, 0, "physical active skills should create slots");

        Assert.Greater(hide.CountVisibleSkillSlots(), 0, "expected visible skill slots in hierarchy");
        Assert.IsFalse(DisplayHiddenUpChain(host), "tab host has DisplayStyle.None in ancestor chain");
    }

    static bool DisplayHiddenUpChain(VisualElement el)
    {
        while (el != null)
        {
            if (el.style.display == DisplayStyle.None)
                return true;
            el = el.hierarchy.parent;
        }

        return false;
    }

    static int CountSkillSlots(VisualElement rowRoot)
    {
        if (rowRoot == null)
            return 0;
        int n = 0;
        for (int b = 0; b < rowRoot.childCount; b++)
        {
            VisualElement box = rowRoot[b];
            for (int r = 0; r < box.childCount; r++)
            {
                VisualElement grid = box[r];
                VisualElement slotsHost = grid.Q("RowsVirtual");
                if (slotsHost != null)
                    n += slotsHost.childCount;
            }
        }

        return n;
    }

}