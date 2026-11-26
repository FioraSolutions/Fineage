using UnityEngine.UIElements;

public partial class ActiveSkillsHide
{
    public bool IsMountedInHierarchy()
    {
        return _activeSkillPanel != null && _activeSkillPanel.panel != null;
    }

    public int CountVisibleSkillSlots()
    {
        if (_activeSkillPanel == null)
            return 0;
        int n = 0;
        n += CountSlotsInRow(_rowPhysical ?? _activeSkillPanel.Q(_rowNamePhysical));
        n += CountSlotsInRow(_rowMagical ?? _activeSkillPanel.Q(_rowNameMagical));
        return n;
    }

    public string DescribePanelDisplayChain()
    {
        if (_activeSkillPanel == null)
            return "panel=null";
        VisualElement row = _rowPhysical ?? _activeSkillPanel.Q(_rowNamePhysical);
        return "panel=" + DisplayOf(_activeSkillPanel) + " rowPhysical=" + DisplayOf(row);
    }

    static int CountSlotsInRow(VisualElement rowRoot)
    {
        if (rowRoot == null || rowRoot.style.display == DisplayStyle.None)
            return 0;
        int n = 0;
        for (int b = 0; b < rowRoot.childCount; b++)
        {
            VisualElement box = rowRoot[b];
            for (int r = 0; r < box.childCount; r++)
            {
                VisualElement grid = box[r];
                VisualElement slotsHost = grid.Q(_rowNameInnerPanel);
                if (slotsHost == null)
                    continue;
                for (int s = 0; s < slotsHost.childCount; s++)
                {
                    if (slotsHost[s].resolvedStyle.display != DisplayStyle.None)
                        n++;
                }
            }
        }

        return n;
    }

    static string DisplayOf(VisualElement el)
    {
        if (el == null)
            return "null";
        return el.style.display.ToString() + (el.panel != null ? "+mounted" : "+detached");
    }
}