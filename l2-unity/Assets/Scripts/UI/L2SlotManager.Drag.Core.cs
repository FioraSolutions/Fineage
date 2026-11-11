using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class L2SlotManager
{
    private void HandleDragRelease()
    {

        switch (_draggedSlot.Type)
        {
            case SlotType.Gear:
                HandleGearDrag();
                break;
            case SlotType.Inventory:
            case SlotType.InventoryBis:
                HandleInventoryDrag();
                break;
            case SlotType.SkillBar:
                HandleSkillbarDrag();
                break;
            case SlotType.Action:
                HandleActionDrag();
                break;
            case SlotType.SkillWindow:
                HandleSkillWindowDrag();
                break;
            case SlotType.Recipe:
                DestroyRecipe();
                break;
            default:
                break;
        }
    }

    private void HandleActionDrag()
    {
        if (_hoverSlot == null)
        {
            return;
        }

        if (_hoverSlot.Type == SlotType.SkillBar)
        {
            AddActionToSkillbar();
        }
    }
}
