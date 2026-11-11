using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MenuWindow : L2Window
{
    void PaintNamed(string elementName, string resourcesPath)
    {
        VisualElement ele = _windowEle.Q(elementName);
        if (ele == null)
            return;
        L2SlotIconView.TrySetResourcesBackground(ele, resourcesPath);
    }

    void PaintButtonStates(string buttonName, string normalPath, string overPath, string downPath)
    {
        Button btn = _windowEle.Q<Button>(buttonName);
        if (btn == null)
            return;

        Texture2D normal = L2SlotIconView.LoadResourcesTexture(normalPath);
        Texture2D over = L2SlotIconView.LoadResourcesTexture(overPath);
        Texture2D down = L2SlotIconView.LoadResourcesTexture(downPath);
        if (normal != null)
            btn.style.backgroundImage = new StyleBackground(normal);

        bool hovered = false;
        btn.RegisterCallback<MouseEnterEvent>(_ =>
        {
            hovered = true;
            if (over != null)
                btn.style.backgroundImage = new StyleBackground(over);
        });
        btn.RegisterCallback<MouseLeaveEvent>(_ =>
        {
            hovered = false;
            if (normal != null)
                btn.style.backgroundImage = new StyleBackground(normal);
        });
        btn.RegisterCallback<MouseDownEvent>(_ =>
        {
            if (down != null)
                btn.style.backgroundImage = new StyleBackground(down);
        }, TrickleDown.TrickleDown);
        btn.RegisterCallback<MouseUpEvent>(_ =>
        {
            Texture2D tex = hovered && over != null ? over : normal;
            if (tex != null)
                btn.style.backgroundImage = new StyleBackground(tex);
        }, TrickleDown.TrickleDown);
    }

    public Vector2 GetWindowPosition()
    {
        return _windowEle.worldBound.position;
    }
}
