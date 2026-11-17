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
        RefreshToggleMark();
    }

    /// <summary>
    /// Interlude MagicSkillWnd: a running toggle draws the animated ToggleEffect shine over its icon, same as the
    /// shortcut bar slot, so the skill tab shows which toggle is switched on.
    /// <para>
    /// Only the skill window is handled here: on the shortcut bar the inner <see cref="SkillSlot"/> shares the bar
    /// button, and <see cref="SkillbarSlot"/> owns that marker (two overlays would stack on one slot).
    /// </para>
    /// </summary>
    public void RefreshToggleMark()
    {
        if (_slotType != SlotType.SkillWindow)
            return;

        // SlotSkill.uxml wraps the 32x32 icon in "SlotBorder"; the shine must sit on the icon, not on the
        // outer 34px group box, or it overlaps the neighbouring slot.
        VisualElement iconHost = _slotElement?.Q<VisualElement>("SlotBorder") ?? _slotElement;
        ToggleSkillShine.Apply(iconHost, _id, _level, ref _toggleShine, ref _togglePulse);
        if (iconHost != null && _id > 0 && SkillToggleState.IsActive(_id))
            iconHost.AddToClassList("toggle-on");
        else
            iconHost?.RemoveFromClassList("toggle-on");
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
}
