using UnityEngine;
using UnityEngine.UIElements;

public static class ClanUiTextures
{
    public static bool SetIcon(VisualElement target, string resourcesPath, string interludeKey)
    {
        if (target == null)
            return false;

        if (L2SlotIconView.TrySetResourcesBackground(target, resourcesPath))
            return true;

        Debug.LogError("[ClanWindow] Missing UI texture '" + interludeKey + "' at Resources/" + resourcesPath);
        L2SlotIconView.Clear(target);
        return false;
    }
}