using UnityEngine.UIElements;

public partial class SkillbarMain
{
    void PaintInterludeChrome()
    {
        GroupBox column = _windowEle?.Q<GroupBox>("Frame_DF_Ver");
        if (column != null)
        {
            while (column.childCount > 3)
                column.RemoveAt(column.childCount - 1);

            L2InterludeHudVerticalFrame.PaintColumn(column, nameof(SkillbarMain));
        }

        VisualElement drag = _windowEle?.Q<VisualElement>("DragArea");
        if (drag != null)
            L2InterludeHudVerticalFrame.PaintDragRail(drag, nameof(SkillbarMain));
    }
}