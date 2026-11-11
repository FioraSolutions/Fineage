using System.Collections.Generic;
using UnityEngine.UIElements;
using System;

public partial class DropdownFieldUtils
{
    private bool IsAnyVisible(List<VisualElement> elements)
    {
        foreach (var e in elements)
        {
            if (e == null) continue;

            if (e.resolvedStyle.display == DisplayStyle.None) continue;
            if (e.worldBound.width <= 0f || e.worldBound.height <= 0f) continue;

            if (!IsVisibleInHierarchy(e)) continue;

            return true;
        }
        return false;
    }

    private bool IsVisibleInHierarchy(VisualElement element)
    {
        for (var cur = element; cur != null; cur = cur.parent)
        {
            if (cur.resolvedStyle.display == DisplayStyle.None)
                return false;

            // Дополнительно можно проверить opacity (нулевая непрозрачность делает невидимым)
            if (cur.resolvedStyle.opacity == 0f)
                return false;
        }
        return true;
    }

    private void HookPanel(VisualElement panel, System.Action<bool> onOpen, System.Action<bool> onClose , ref bool isCloseCreate)
    {
        if (panel == null) return;


        if (panel.panel != null)
            onOpen?.Invoke(true);

        panel.RegisterCallback<AttachToPanelEvent>(evt => onOpen?.Invoke(true));
        panel.RegisterCallback<DetachFromPanelEvent>(evt => onClose?.Invoke(true));
        isCloseCreate = true;
    }
}
