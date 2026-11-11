using Newtonsoft.Json.Bson;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillSlot : L2DraggableSlot
{
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
}
