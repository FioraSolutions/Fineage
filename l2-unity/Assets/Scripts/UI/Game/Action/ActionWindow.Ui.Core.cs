using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ActionWindow : L2PopupWindow
{
    void AssignSection(InterludeActionSection section, VisualElement _)
    {
        int baseIndex = SectionBaseSlotIndex(section);
        int maxSlots = SectionMaxSlots(section);
        InterludeActionCatalog.Entry[] rows = InterludeActionCatalog.SectionSlots(section);
        for (int i = 0; i < rows.Length && i < maxSlots; i++)
            _slots[baseIndex + i].AssignAction((ActionType)rows[i].Id);
    }

    static int SectionBaseSlotIndex(InterludeActionSection section)
    {
        switch (section)
        {
            case InterludeActionSection.Basic: return 0;
            case InterludeActionSection.Party: return SLOTS_PER_ROW * 3;
            case InterludeActionSection.Social: return SLOTS_PER_ROW * 3 + SLOTS_PER_ROW * 2;
            default: return 0;
        }
    }

    static int SectionMaxSlots(InterludeActionSection section)
    {
        switch (section)
        {
            case InterludeActionSection.Basic: return SLOTS_PER_ROW * 3;
            case InterludeActionSection.Party: return SLOTS_PER_ROW * 2;
            case InterludeActionSection.Social: return SLOTS_PER_ROW * 3;
            default: return 0;
        }
    }
}
