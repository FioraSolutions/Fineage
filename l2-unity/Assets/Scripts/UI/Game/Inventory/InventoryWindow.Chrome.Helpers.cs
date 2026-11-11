using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using System.Collections.Generic;
using VContainer;

public partial class InventoryWindow
{
    void ApplyInterludeTabChrome()
    {
        if (_inventoryTabView == null)
            return;

        Texture2D tabActive = L2SlotIconView.LoadResourcesTexture(
            "Data/UI/Inventory/Inventory_Interlude_Tab1");
        Texture2D tabIdle = L2SlotIconView.LoadResourcesTexture(
            "Data/UI/Inventory/Inventory_Interlude_Tab2");
        Texture2D tabHover = L2SlotIconView.LoadResourcesTexture(
            "Data/UI/Inventory/Inventory_Interlude_Tab2_over");

        foreach (VisualElement header in _inventoryTabView.Query(className: "tab-header").ToList())
        {
            ApplyTabHeaderBackground(header, tabActive, tabIdle);

            header.RegisterCallback<MouseEnterEvent>(_ =>
            {
                if (IsTabHeaderActive(header) || tabHover == null)
                    return;
                header.style.backgroundImage = new StyleBackground(tabHover);
            });
            header.RegisterCallback<MouseLeaveEvent>(_ =>
                ApplyTabHeaderBackground(header, tabActive, tabIdle));
        }
    }

    static bool IsTabHeaderActive(VisualElement header) =>
        header.ClassListContains("active") || header.ClassListContains("currentlySelectedTab");

    static void ApplyTabHeaderBackground(VisualElement header, Texture2D active, Texture2D idle)
    {
        Texture2D tex = IsTabHeaderActive(header) ? active : idle;
        if (tex != null)
            header.style.backgroundImage = new StyleBackground(tex);
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        CreateTabs();

        CreateMinimizedWindow();

        yield return new WaitForEndOfFrame();

       // UpdateItemList(_playerItems);
    }
}
