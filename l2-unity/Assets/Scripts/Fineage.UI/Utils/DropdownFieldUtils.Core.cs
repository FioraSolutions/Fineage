using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

public partial class DropdownFieldUtils
{
    public void SubscribeToDropdownOpenClose(VisualElement root, System.Action<bool> onOpen, System.Action<bool> onClose , ref bool isCloseCreate)
    {
        if (root == null) return;

        var panels = root.Query<VisualElement>(null, "unity-base-dropdown__container-inner").ToList();
        foreach (var p in panels) HookPanel(p, onOpen, onClose , ref isCloseCreate);
    }

    public  bool IsDropdownPanelOpen(VisualElement root)
    {
        if (root == null) return false;

        var inner = root.Query<VisualElement>(null, "unity-base-dropdown__container-inner").ToList();
        if (IsAnyVisible(inner))
        {
            return true;
        }


       // var contentWidth = root.Query<VisualElement>(null, "unity-base-dropdown--content-width-menu").ToList();
        //if (IsAnyVisible(contentWidth))
       // {
        //    Debug.Log("Return true 2");
        //    return true;
       // }

        //var items = root.Query<VisualElement>(null, "unity-base-dropdown__item").ToList();
       // if (IsAnyVisible(items)){
         //   Debug.Log("Return true 3");
        //    return true;
       // }



        return false;
    }
}
