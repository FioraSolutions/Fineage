using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Shared horizontal resize affordance: keeps resizing working but never paints grip art
/// (retail Interlude plates have no visible size control on Target/Status windows).
/// </summary>
public static class HorizontalResizeBinding
{
    public const string InvisibleHandleClass = "invisible-resize-handle";
    public const string InvisibleGripClass = "invisible-resize-grip";

    public static bool TryBind(
        VisualElement windowRoot,
        VisualElement handle,
        float minWidth,
        float maxWidth,
        VisualElement gripContainer = null)
    {
        if (windowRoot == null || handle == null)
            return false;

        handle.AddToClassList(InvisibleHandleClass);
        ClearGripArt(handle);

        if (gripContainer != null)
        {
            gripContainer.AddToClassList(InvisibleGripClass);
            ClearGripArt(gripContainer);
        }

        if (handle.userData is HorizontalResizeManipulator)
            return true;

        var manipulator = new HorizontalResizeManipulator(handle, windowRoot, minWidth, maxWidth);
        handle.userData = manipulator;
        handle.AddManipulator(manipulator);
        return true;
    }

    static void ClearGripArt(VisualElement element)
    {
        element.style.backgroundImage = StyleKeyword.Null;
        element.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
    }
}