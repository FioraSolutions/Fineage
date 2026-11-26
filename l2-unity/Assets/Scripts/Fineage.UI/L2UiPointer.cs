using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Shared screen ↔ UI Toolkit panel coordinate conversion for hit-testing.
/// </summary>
public static class L2UiPointer
{
    public static Vector2 PanelPosition(VisualElement root, Vector2 screenPosition)
    {
        if (root == null || root.panel == null)
            return screenPosition;

        return RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(screenPosition.x, Screen.height - screenPosition.y));
    }

    public static bool PanelBoundsContain(VisualElement element, Vector2 screenPosition)
    {
        if (element == null)
            return false;

        Vector2 panelPos = PanelPosition(element, screenPosition);
        return element.worldBound.Contains(panelPos);
    }
}