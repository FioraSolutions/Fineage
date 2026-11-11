using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using System.Collections;

public partial class SkillbarSlot
{
    public SkillbarSlot(VisualElement slotElement, int position, int skillbarId, int slot) : base(slotElement, position, SlotType.SkillBar, true, false)
    {
        _slotElement = slotElement;
        _position = position;
        _skillbarId = skillbarId;
        _slot = slot;
        _keyElement = _slotElement.Q<VisualElement>("Key");
        _reuseElement = _slotElement.Q<VisualElement>("ReuseBg");
        _rechargeMaskBg = _slotElement.Q<VisualElement>("RechargeMaskBg");
        // UITK Button.Clickable steals pointer capture from SlotDragManipulator and leaves the
        // floating skill icon stuck to the cursor. Clicks go through L2ClickableSlot callbacks.
        if (_slotElement is Button button)
            button.clickable = null;
    }

    public VisualElement GetReuseElement()
    {
        return _reuseElement;
    }

    public VisualElement GetRechargeMaskElement()
    {
        return _rechargeMaskBg;
    }

    public void AssignShortcut(Shortcut shortcut)
    {
        ClearInnerSlot();
        // Do not ClearManipulators() here — that nulls SkillbarSlot._hoverManipulator while
        // MouseUp handlers stay registered → NRE in HandleSlotClickUp and dead clicks.
        if (_buttonClickSoundManipulator != null)
        {
            _slotElement.RemoveManipulator(_buttonClickSoundManipulator);
            _buttonClickSoundManipulator = null;
        }

        _shortcut = shortcut;
        _itemId = 0;
        // Re-assert: UITK may reattach Clickable on Button targets.
        if (_slotElement is Button button)
            button.clickable = null;
        _buttonClickSoundManipulator = new ButtonClickSoundManipulator(_slotElement);
        _slotElement.AddManipulator(_buttonClickSoundManipulator);

        switch (shortcut.Type)
        {
            case Shortcut.TYPE_ACTION:
                AssignAction(shortcut.Id);
                break;
            case Shortcut.TYPE_ITEM:
                // Zone stores template itemId in Level so the bar can rebind after stack objectId churn.
                AssignItem(shortcut.Id, shortcut.Level);
                break;
            case Shortcut.TYPE_MACRO:
                break;
            case Shortcut.TYPE_RECIPE:
                break;
            case Shortcut.TYPE_SKILL:
                AssignSkill(shortcut.Id , shortcut.Level);
                break;
        }
    }
}
