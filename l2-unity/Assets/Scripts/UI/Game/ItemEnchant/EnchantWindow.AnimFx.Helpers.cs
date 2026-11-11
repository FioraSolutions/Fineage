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
