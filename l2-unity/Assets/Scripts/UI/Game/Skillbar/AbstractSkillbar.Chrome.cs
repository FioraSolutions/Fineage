using UnityEngine.UIElements;

public abstract partial class AbstractSkillbar
{
    protected void PaintInterludeHudChrome()
    {
        L2InterludeHudVerticalFrame.TryPaintVerticalColumn(_windowEle, GetType().Name);

        VisualElement drag = _windowEle?.Q<VisualElement>("DragArea");
        if (drag != null)
            L2InterludeHudVerticalFrame.PaintDragRail(drag, GetType().Name);

        bool vertical = _windowEle?.ClassListContains("skillbar-vertical") == true;
        VisualElement inner = _windowEle?.Q<VisualElement>("InnerContainer");
        L2InterludeHudSkinPipeline.PaintSkillbarPanel(inner, vertical, GetType().Name);

        L2InterludeHudSkinPipeline.ValidateTextures(
            InterludeHudWindowCatalog.ShortcutWnd,
            GetType().Name);
    }
}