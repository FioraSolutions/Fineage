using UnityEngine.UIElements;

public abstract partial class AbstractSkillbar
{
    protected void PaintInterludeHudChrome()
    {
        L2InterludeHudVerticalFrame.TryPaintVerticalColumn(_windowEle, GetType().Name);

        VisualElement drag = _windowEle?.Q<VisualElement>("DragArea");
        if (drag != null)
            L2InterludeHudVerticalFrame.PaintDragRail(drag, GetType().Name);
    }
}