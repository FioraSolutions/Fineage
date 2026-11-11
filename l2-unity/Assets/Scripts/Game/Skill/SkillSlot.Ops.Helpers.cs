using Newtonsoft.Json.Bson;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillSlot : L2DraggableSlot
{
    public void AssignEmpty()
    {
        _empty = true;
        _id = 0;
        _name = "Unknown";
        _description = "Unknown item.";
        _skillInstance = null;

        if (_slotElement != null)
        {
            _slotElement.style.display = DisplayStyle.None;
            ResetIcon();
        }

        // Отключаем drag
        if (_slotDragManipulator != null)
            _slotDragManipulator.enabled = false;
    }

    private void AddIcon(Skillgrp skill)
    {
        L2SlotIconView.SetSkillIcon(_slotBg, skill != null ? skill.Icon : null);
    }

    public void ResetIcon()
    {
        if (SlotBg != null)
        {
            L2SlotIconView.Clear(_slotBg);
            if (_slotDragManipulator != null)
                _slotDragManipulator.enabled = false;
        }
        else
        {
            L2SlotIconView.Clear(_slotElement);
        }
    }

    public void OnClickLeftEvent(int position)
    {
        // Debug.Log("Click Event Skill Slot");
    }
}
