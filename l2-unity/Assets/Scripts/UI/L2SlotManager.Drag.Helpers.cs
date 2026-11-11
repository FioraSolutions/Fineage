using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class L2SlotManager
{
    private void HandleGearDrag()
    {
        if (_hoverSlot != null && _hoverSlot.Type == SlotType.Enchant)
        {
            AddItemToEnchant();
            return;
        }

        if (_hoverSlot.Type == SlotType.Inventory || _hoverSlot.Type == SlotType.InventoryBis)
        {
            Unequip();
        }
        else if (_hoverSlot.Type == SlotType.Trash)
        {
            //QuantityInputPacketServices.Window.ShowWindow(this, source, type, listServer, position);
            DestroyItem();
        }
    }

    private void HandleInventoryDrag()
    {
        var inventorySlot = (InventorySlot)_draggedSlot;

        if (_hoverSlot == null)
        {
            if (!SystemUIPacketServices.Ui.MouseOverUI)
            {
                DropItem();
            }

            return;
        }

        switch (_hoverSlot.Type)
        {
            case SlotType.Gear when IsItemGear(inventorySlot.ItemCategory):
                Equip();
                break;
            case SlotType.Inventory:
                ChangeItemOrder();
                break;
            case SlotType.Trash:
                DestroyItem();
                break;
            case SlotType.SkillBar:
                AddItemToSkillbar();
                break;
            case SlotType.Enchant:
                AddItemToEnchant();
                break;
            default:
                if (!SystemUIPacketServices.Ui.MouseOverUI)
                {
                    DropItem();
                }
                break;
        }
    }

    private void HandleSkillWindowDrag()
    {
        if (_hoverSlot == null || SkillbarPacketServices.Skillbar == null || SkillbarPacketServices.Skillbar.Locked)
            return;

        if (_hoverSlot.Type != SlotType.SkillBar)
            return;

        int position = _hoverSlot.Position;
        SkillSlot sourceSlot = (SkillSlot)_draggedSlot;
        if (sourceSlot == null || sourceSlot.Id <= 0)
            return;

        ShortcutPacketServices.Shortcuts.AddShortcut(position, sourceSlot.Id, Shortcut.TYPE_SKILL, sourceSlot.Level);
    }
}
