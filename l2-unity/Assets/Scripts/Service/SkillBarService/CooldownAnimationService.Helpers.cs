using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CooldownAnimationService
{
    private void SetupCompletionFrame(VisualElement reuseElement, VisualElement rechargeMaskBg)
    {
        if (rechargeMaskBg != null)
        {
            rechargeMaskBg.style.opacity = 1;
            rechargeMaskBg.style.display = DisplayStyle.Flex;
        }

        L2SlotIconView.SetNamedIcon(reuseElement, "Icon_dualcap");
        reuseElement.style.backgroundColor = new Color(0f, 0f, 0f, 0.9f);
    }

    private void CleanupCompletionFrame(VisualElement reuseElement, VisualElement rechargeMaskBg)
    {
        if (reuseElement != null)
        {
            reuseElement.style.display = DisplayStyle.None;
            reuseElement.style.backgroundColor = new Color(0f, 0f, 0f, 0.1f);
            reuseElement.style.backgroundImage = null;
            reuseElement.style.opacity = 1;
        }

        if (rechargeMaskBg != null)
        {
            rechargeMaskBg.style.display = DisplayStyle.None;
            rechargeMaskBg.style.opacity = 1;
        }
    }
}
