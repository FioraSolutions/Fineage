using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static partial class AutoShotSlotGlow
{
    public static void EnsureOverlay(VisualElement slot, ref VisualElement glow)
    {
        if (slot == null)
            return;
        if (glow == null)
        {
            glow = new VisualElement
            {
                pickingMode = PickingMode.Ignore,
                name = "AutoShotGlow"
            };
            glow.AddToClassList("auto-shot-glow");
            slot.Add(glow);
        }
        glow.style.display = DisplayStyle.Flex;
        glow.style.opacity = 0.85f;
    }
}
