using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractSkillbar
{
    public void UpdateShortcut(Shortcut shortcut, int slot)
    {
        for (int i = 0; i < _barSlots.Count; i++)
        {
            if (_barSlots[i].Position == _page * PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR + slot)
            {
                // Debug.Log($"Found slot at index {i} with slot {slot}.");
                _barSlots[i].AssignShortcut(shortcut);
                break;
            }
        }
    }

    public void RefreshSlotMarks()
    {
        if (_barSlots == null)
            return;
        for (int i = 0; i < _barSlots.Count; i++)
            _barSlots[i]?.RefreshMarks();
    }

    public void HideBar()
    {
        _windowEle.style.display = DisplayStyle.None;
    }

    public void ShowBar()
    {
        _windowEle.style.display = DisplayStyle.Flex;
    }

    public void DeleteShortcut(int slot)
    {
        for (int i = 0; i < _barSlots.Count; i++)
        {
            if (_barSlots[i].Position == slot)
            {
                // Debug.Log($"Succesfully deleted slot {slot}.");
                _barSlots.Remove(_barSlots[i]);
                break;
            }
        }

        CreateSlot(slot % 12);
    }
}
