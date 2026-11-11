using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractSkillbar
{
    public void ChangePage(int page)
    {
        _page = page;
        _arrowInputManipulator.SelectIndex(page);

        ResetShortcuts();
    }

    public void ResetShortcuts()
    {
        // Clean up slot callbacks and manipulators
        if (_barSlots != null)
        {
            foreach (SkillbarSlot slot in _barSlots)
            {
                slot.ClearManipulators();
            }
            _barSlots.Clear();
        }

        _barSlots = new List<SkillbarSlot>();

        // Clean up gear anchors from any child visual element
        for (int i = 0; i < _slotAnchors.Count; i++)
        {
            CreateSlot(i);
        }
    }

    private void CreateSlot(int position)
    {

        VisualElement anchor = _slotAnchors[position];

        //Debug.Log($"Create slot at {position} with anchor {anchor.name}.");

        // Clear childs
        anchor.Clear();

        // Create slots
        VisualElement slotElement = SkillbarPacketServices.Skillbar.BarSlotTemplate.Instantiate()[0];
        anchor.Add(slotElement);

        SkillbarSlot slot = new SkillbarSlot(slotElement, _page * PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR + position, _skillbarIndex, position);
        _barSlots.Add(slot);
    }
}
