using UnityEngine.UIElements;

public partial class PassiveSkillsHide
{
    public bool IsReady => _passiveSkillPanel != null;

    public bool IsMountedInHierarchy()
    {
        return _passiveSkillPanel != null && _passiveSkillPanel.panel != null;
    }

    public int CountVisibleSkillSlots()
    {
        if (_passiveSkillPanel == null)
            return 0;
        int n = 0;
        n += CountSlotsInRow(_rowAbility ?? _passiveSkillPanel.Q(_rowNameAbility));
        n += CountSlotsInRow(_rowSubject ?? _passiveSkillPanel.Q(_rowNameSubject));
        return n;
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
}