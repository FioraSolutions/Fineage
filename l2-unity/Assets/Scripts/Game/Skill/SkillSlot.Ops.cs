using Newtonsoft.Json.Bson;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillSlot : L2DraggableSlot
{
    public void UpdateData(SkillInstance skillInstance)
    {
        _id = skillInstance.SkillID;
        _level = skillInstance.Level;
        _skillInstance = skillInstance;
    }

    private void Assign(Skillgrp skill , bool isPassive)
    {
        if (skill == null)
        {
            //Debug.LogWarning($"AssignSkill: skill not found (id={skillId}, level={level})");
            return;
        }

        _empty = false;

        if (_slotDragManipulator != null)
            _slotDragManipulator.enabled = true;


        _slotElement?.RemoveFromClassList("empty");

        AddIcon(skill);
        ToggleDrag(_isPassive);


    }

    private void ToggleDrag(bool isPassive)
    {
        if (_isPassive)
        {
            if (_slotDragManipulator != null)
                _slotDragManipulator.enabled = false;
        }
        else
        {
            if (_slotDragManipulator != null)
                _slotDragManipulator.enabled = true;
        }
    }

    public void AssignDestroy()
    {
        _empty = true;
        _id = 0;
        _name = "Unknown";
        _description = "Unknown item.";
        _skillInstance = null;

        _slotElement.parent?.Remove(_slotElement);
        ClearManipulators();
    }

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
