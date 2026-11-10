using Newtonsoft.Json.Bson;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillSlot : L2DraggableSlot
{
    private SkillInstance _skillInstance { get;  set; }
    private bool _empty = false;
    public event Action<int> EventLeftClick;
    private bool _isPassive = false;
    private VisualElement _reuseElement;
    private VisualElement _rechargeMaskBg;

    public SkillSlot(VisualElement slotElement, int position, SlotType slotType) : base(position, slotElement, slotType, true, false)
    {
        _slotElement = slotElement;
        _position = position;
        // GroupBox defaults to focusable and can steal pointer from SlotDragManipulator.
        if (_slotElement != null)
            _slotElement.focusable = false;
        // Skillbar slots are Buttons — Clickable fights pointer capture and sticks the drag ghost.
        if (_slotElement is Button button)
            button.clickable = null;
        _reuseElement = _slotElement?.Q<VisualElement>("ReuseBg");
        _rechargeMaskBg = _slotElement?.Q<VisualElement>("RechargeMaskBg");
    }

    public VisualElement GetReuseElement() => _reuseElement;
    public VisualElement GetRechargeMaskElement() => _rechargeMaskBg;

    protected override void HandleRightClick()
    {
    }

    protected override void HandleMiddleClick()
    {
    }

    protected override void HandleLeftClick()
    {
        if (!_isPassive)
        {
            // Debug.Log("Handle Left Click");
            EventLeftClick?.Invoke(_position);
            ShortcutPacketServices.Actions.UseSkill(_id);
        }
    }

    public void AssignSkill(int skillId, int level , bool isPassive)
    {
        _id = skillId;
        _level = level;
        _isPassive = isPassive;
        var skill = TablePacketServices.SkillGrps.GetSkill(skillId, level);
        Assign(skill , _isPassive);
    }

    public void AssignSkill(SkillInstance skillInstance)
    {
        _id = skillInstance.SkillID;
        _level = skillInstance.Level;
        _skillInstance = skillInstance;
        _isPassive = skillInstance.IsPassive;

        if(_id != -1)
        {
            var skill = TablePacketServices.SkillGrps.GetSkill(_id, _level);
            Assign(skill, _isPassive);
        }
    }

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
