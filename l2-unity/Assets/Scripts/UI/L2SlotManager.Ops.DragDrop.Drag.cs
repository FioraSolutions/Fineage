using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class L2SlotManager
{
    private void DestroyRecipe()
    {
        if (_hoverSlot == null)
        {
            return;
        }

        SystemMessagePacketServices.Window.OnButtonOk += OkDestroyRecipe;
        SystemMessagePacketServices.Window.OnButtonClosed += OnРЎancelRecipe;
        SystemMessagePacketServices.Window.ShowWindowDialogYesOrNot("Are you sure you want to destroy the recipe?");

        int position = _hoverSlot.Position;
        TradingSlot sourceSlot = (TradingSlot)_draggedSlot;
        _recipeId = CraftPacketServices.RecipeBook.GetRecipeIdByPosition(sourceSlot.Position);

    }

    private void OkDestroyRecipe()
    {
        ZoneCommands.Send(new RequestRecipeBookDestroyCommand(_recipeId));
        CleanupMessageWindow();
    }

    private void OnРЎancelRecipe()
    {

        CleanupMessageWindow();
    }

    private void CleanupMessageWindow()
    {
        SystemMessageUtils.CancelEvent(SystemMessagePacketServices.Window, OkDestroyRecipe, OnРЎancelRecipe);
    }

    private void HandleSkillbarDrag()
    {
        if (SkillbarPacketServices.Skillbar.Locked)
        {
            return;
        }

        if (_hoverSlot == null)
        {
            if (!SystemUIPacketServices.Ui.MouseOverUI)
            {
                RemoveSkillbarSlot();
            }

            return;
        }

        switch (_hoverSlot.Type)
        {
            case SlotType.SkillBar:
                MoveSkillbarSlot();
                break;
            case SlotType.Enchant:
                int oldSlot = _draggedSlot.Id;
                int newSlot = _hoverSlot.Id;
                // Debug.LogWarning($"Moving skillbar enchant from slot {oldSlot} to slot {newSlot}.");
                break;
            default:
                if (!SystemUIPacketServices.Ui.MouseOverUI)
                {
                    RemoveSkillbarSlot();
                }
                break;
        }
    }

    private bool IsValidDrag() => _draggedSlot != null && (_hoverSlot != null || !SystemUIPacketServices.Ui.MouseOverUI);

    private bool IsSameSlot() => _draggedSlot != null && _hoverSlot != null && _draggedSlot.Position == _hoverSlot.Position && _draggedSlot.Type == _hoverSlot.Type;

    private bool IsItemGear(ItemCategory category)
    {
        return category == ItemCategory.Weapon
            || category == ItemCategory.ShieldArmor
            || category == ItemCategory.Jewel;
    }

    private void Unequip()
    {
        // Unequip logic
        GearSlot slot = (GearSlot)_draggedSlot;
        // Debug.Log($"Unequip {slot.Id}.");
        slot.UseItem();
    }

    private void Equip()
    {
        // Equip logic
        InventorySlot slot = (InventorySlot)_draggedSlot;
        // Debug.Log($"Equip {slot.Id}.");
        slot.UseItem();
    }
}
