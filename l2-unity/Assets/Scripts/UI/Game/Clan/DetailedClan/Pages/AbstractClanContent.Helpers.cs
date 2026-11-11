using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractClanContent : AbstractButtonEvents
{
    protected void SubscribeApplyButton(Button applylButton, VisualElement detailedInfoElement , int useRank)
    {
        if (applylButton != null)
        {
            void OnClick()
            {
                detailedInfoElement.style.display = DisplayStyle.None;
                applylButton.clicked -= OnClick;  // Unsubscribe here
                OnClickApply?.Invoke(useRank);
            }

            applylButton.clicked += OnClick;
        }
    }

    protected void HideElement(VisualElement element)
    {
        if (element != null)
        {
            element.style.display = DisplayStyle.None;
        }
    }

    protected void ShowElement(VisualElement element)
    {
        if (element != null)
        {
            element.style.display = DisplayStyle.Flex;
        }
    }
}
