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
    private void ResetProgressBar(VisualElement progressBar)
    {
        progressBar.style.width = Convert.ToSingle(245);
    }

    private void StartFinalEvent(float fadeStartTime, float elapsedTime, VisualElement element1)
    {
        if (elapsedTime >= fadeStartTime)
        {
            AlphaChanged(fadeStartTime, elapsedTime, element1);
            //PlayAnim(_loadingAnimPng, _animElement , fadeStartTime);
        }
            
    }

    public ItemInstance GetItemByPosition(int position)
    {
        if(position == 0)
        {
           return  _enchant1.ItemInstance;
        }else if (position == 1)
        {
            return _enchant2.ItemInstance;
        }

        return null;
    }

    private void SetIconGradeCrystal(ItemGrade itemGrade)
    {
        if(itemGrade == ItemGrade.d)
        {
            _enchant2.AssignItem(new ItemInstance(-1, 1458, ItemLocation.Inventory, 2, 1, ItemCategory.Item, false, ItemSlot.none, 0, 9999));
        }
        else if(itemGrade == ItemGrade.c)
        {
            _enchant2.AssignItem(new ItemInstance(-1, 1459, ItemLocation.Inventory, 2, 1, ItemCategory.Item, false, ItemSlot.none, 0, 9999));
        }
    }

    private void SetTextLabel(ItemInstance itemInstance)
    {
        if (itemInstance != null && itemInstance.EnchantLevel != 0)
        {
            _textLabel.text = "Success! The Item is now " + itemInstance.EnchantLevel;
        }
        else
        {
            _textLabel.text = "Success! The Item is now +3";
        }
    }

    private void SetTextLabelFail()
    {
        _textLabel.text = "Failed! You have obtained " + 0 + " of Crystal (C-Grade)";
    }

    private void AlphaChanged(float fadeStartTime , float elapsedTime , VisualElement element1)
    {
        if (elapsedTime >= fadeStartTime)
        {
            float fadeT = (elapsedTime - fadeStartTime) / (_moveDuration - fadeStartTime);
            float alpha = Mathf.Lerp(1f, 0f, fadeT);
            element1.style.opacity = alpha; 
        }
    }

    private void SetOpacity0(VisualElement element1)
    {
        element1.style.opacity = 0;
    }
}
