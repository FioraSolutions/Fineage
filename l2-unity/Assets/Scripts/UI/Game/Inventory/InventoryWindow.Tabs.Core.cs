using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using VContainer;

public partial class InventoryWindow
{
    private void CreateTabs()
    {
        _inventoryTabView = GetElementById("InventoryTabView");

        VisualElement tabHeaderContainer = _inventoryTabView.Q<VisualElement>("tab-header-container");
        // Interlude: tabs left, (n/max) count right. Prefer dedicated host when present.
        VisualElement tabHeaderHost = _inventoryTabView.Q<VisualElement>("tab-headers") ?? tabHeaderContainer;
        VisualElement tabContainer = _inventoryTabView.Q<VisualElement>("tab-content-container");

        for (int i = 0; i < _tabs.Count; i++)
        {
            VisualElement tabElement = _tabTemplate.CloneTree()[0];
            tabElement.name = _tabs[i].TabName;
            tabElement.AddToClassList("unselected-tab");

            VisualElement tabHeaderElement = _tabHeaderTemplate.CloneTree()[0];
            tabHeaderElement.name = _tabs[i].TabName;
            tabHeaderElement.Q<Label>().text = _tabs[i].TabName;

            tabHeaderHost.Add(tabHeaderElement);
            tabContainer.Add(tabElement);

            _tabs[i].Initialize(_windowEle, tabElement, tabHeaderElement , true , true);
        }

        if (_tabs.Count > 0)
        {
            SwitchTab(_tabs[0]);
        }

        _tabs[0].MainTab = true;
        ApplyInterludeTabChrome();

        _gearTab = new InventoryGearTab();
        _gearTab.Initialize(_windowEle, null, null , true , true);
        _itemSurfaceReady = true;

        // ItemList normally arrives during EnterWorld, before this window is built, so the
        // rows are cached in PlayerInventory. Push them now that the tabs exist.
        if (InventoryPacketServices.Inventory != null)
            InventoryPacketServices.Inventory.RefreshWindow();
        RefreshWeightFromPlayer();
    }

    public bool SwitchTab(InventoryTab switchTo)
    {
        
        if (_activeTab != switchTo)
        {
            if (_activeTab != null)
            {
                _activeTab.TabContainer.AddToClassList("unselected-tab");
                _activeTab.TabHeader.RemoveFromClassList("active");
            }

            switchTo.TabContainer.RemoveFromClassList("unselected-tab");
            switchTo.TabHeader.AddToClassList("active");
            //ScrollDown(switchTo.Scroller);
            SwitchItemList(switchTo);
            _activeTab = switchTo;
            RefreshInterludeTabChromeImages();
            return true;
        }
        
        return false;
    }

    void RefreshInterludeTabChromeImages()
    {
        if (_inventoryTabView == null)
            return;

        Texture2D tabActive = L2SlotIconView.LoadResourcesTexture(
            "Data/UI/Inventory/Inventory_Interlude_Tab1");
        Texture2D tabIdle = L2SlotIconView.LoadResourcesTexture(
            "Data/UI/Inventory/Inventory_Interlude_Tab2");

        foreach (VisualElement header in _inventoryTabView.Query(className: "tab-header").ToList())
            ApplyTabHeaderBackground(header, tabActive, tabIdle);
    }
}
