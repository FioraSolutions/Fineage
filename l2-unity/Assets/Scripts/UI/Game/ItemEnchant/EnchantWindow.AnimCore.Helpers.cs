using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using VContainer;

public partial class EnchantWindow
{
    private void StartEnchantAnim(bool isSuccess)
    {
        if (!_isRun)
        {
            _isRun = true;
            DecreaseProgressBar();
            StartCoroutine(MoveElementsToCenter(_slot1, _slot2 , isSuccess));
        }
    }

    private void UpdateS1()
    {
        var itemInstance = InventoryPacketServices.Inventory.GetItem(_selectItem.ObjectId);
        if (itemInstance != null && itemInstance.EnchantLevel > 0)
        {
            //StorageVariable.getInstance().AddS1Items(new VariableItem(itemInstance.Count.ToString(), itemInstance.ObjectId));
            //StorageVariable.getInstance().AddS2Items(new VariableItem(itemInstance.ItemData.ItemName.Name, itemInstance.ObjectId));
        }
    }

    private void ResetStyle(VisualElement _slot1 , VisualElement _slot2 , Label text)
    {
        _slot2.style.opacity = 1;
        _slot1.style.opacity = 1;
        _slot1.style.left = offsetLeft1;
        _slot2.style.left = 0;
        text.style.marginTop = 0;

    }

    private void ClearSlot2(VisualElement _slot2)
    {
        _slot2.Clear();
        _enchant2 = ChangeSlotBoxTypeChoice(_slot2, _enchant2);
    }
}
