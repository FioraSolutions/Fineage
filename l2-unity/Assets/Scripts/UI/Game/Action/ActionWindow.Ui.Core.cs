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
        InterludeActionAvailabilityService availability = ActionAvailabilityPacketServices.Availability;
        for (int i = 0; i < maxSlots; i++)
        {
            ActionSlot slot = _slots[baseIndex + i];
            if (i < rows.Length && availability.IsAvailable(in rows[i]))
                slot.AssignAction((ActionType)rows[i].Id);
            else
                slot.ClearAction();
        }
    }

    public void RefreshActionAvailability()
    {
        if (_slots == null || _slots.Count == 0)
            return;
        AssignSection(InterludeActionSection.Basic, _basicContainer);
        AssignSection(InterludeActionSection.Party, _partyContainer);
        AssignSection(InterludeActionSection.Social, _socialContainer);
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
