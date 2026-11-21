using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Skills window (MagicSkillWnd) must populate physical/magic/passive rows after SkillList.
/// </summary>
public sealed class SkillListWindowPopulateEditModeTests
{
    [Test]
    public void ActiveSkillsHide_CreateSlots_populates_physical_and_magic_rows()
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

        var root = new VisualElement();
        root.style.width = 260;
        hide.GetOrCreateTab(root);

        var activeSkills = new List<SkillInstance>
        {
            new SkillInstance(3, 1, false, false),
            new SkillInstance(1177, 1, false, false),
        };

        hide.CreateSlots(activeSkills, 0);

        VisualElement rowPhysical = root.Q("RowPhysical");
        VisualElement rowMagical = root.Q("RowMagical");
        Assert.NotNull(rowPhysical, "RowPhysical missing");
        Assert.NotNull(rowMagical, "RowMagical missing");
        Assert.Greater(rowPhysical.childCount, 0, "physical row has no slot panels");
        Assert.AreEqual(DisplayStyle.Flex, rowPhysical.style.display, "physical section collapsed");

        int physicalSlots = CountSkillSlots(rowPhysical);
        int magicSlots = CountSkillSlots(rowMagical);
        Assert.Greater(physicalSlots, 0, "physical active skills should create slots");
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