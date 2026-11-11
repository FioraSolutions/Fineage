using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ActionWindow : L2PopupWindow
{
    void AssignBasic(params ActionType[] actions)
    {
        for (int i = 0; i < actions.Length && i < SLOTS_PER_ROW * 4; i++)
            _slots[i].AssignAction(actions[i]);
    }

    void AssignParty(params ActionType[] actions)
    {
        int baseIndex = SLOTS_PER_ROW * 4;
        for (int i = 0; i < actions.Length && i < SLOTS_PER_ROW * 2; i++)
            _slots[baseIndex + i].AssignAction(actions[i]);
    }

    void AssignSocial(params ActionType[] actions)
    {
        int baseIndex = SLOTS_PER_ROW * 4 + SLOTS_PER_ROW * 2 + SLOTS_PER_ROW * 2;
        for (int i = 0; i < actions.Length && i < SLOTS_PER_ROW * 3; i++)
            _slots[baseIndex + i].AssignAction(actions[i]);
    }
}
