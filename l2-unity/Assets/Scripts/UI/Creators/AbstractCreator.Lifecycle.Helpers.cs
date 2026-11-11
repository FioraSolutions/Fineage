public abstract partial class AbstractCreator
{
    public void CreateTradeTabs(VisualElement inventoryTabView, VisualTreeAsset _tabTemplate, VisualTreeAsset _tabHeaderTemplate , SlotType slotType , bool isDragged)
    {
        VisualElement tabHeaderContainer = inventoryTabView.Q<VisualElement>("tab-header-container");
        VisualElement tabContainer = inventoryTabView.Q<VisualElement>("tab-content-container");

        if (inventoryTabView != null & _tabTemplate != null & _tabHeaderTemplate != null & tabContainer != null)
        {


            for (int i = _tabsTradeTabs.Count() - 1; i >= 0; i--)
            {
                string tabName = _nameTabs[i];

                VisualElement tabElement = _tabTemplate.CloneTree()[0];
                tabElement.name = tabName;
                tabElement.AddToClassList("unselected-tab");


                VisualElement tabHeaderElement = _tabHeaderTemplate.CloneTree()[0];
                tabHeaderElement.name = tabName;
                tabHeaderElement.Q<Label>().text = tabName;

                tabHeaderContainer.Add(tabHeaderElement);
                tabContainer.Add(tabElement);

                _tabsTradeTabs[i] = new TradeTab(tabName, 96, tabElement, tabHeaderElement, true , slotType , isDragged);

                _tabsTradeTabs[i].EventSwitch += OnSwitchEvent;
                _tabsTradeTabs[i].EventLeftClick += OnLeftClick;
            }
        }
        else
        {
            // Debug.LogError("CreatorTradingWindows: Not Create Windows tabs!!!!");
        }

        if (_tabsTradeTabs.Length > 0)
        {
            SwitchTab(_tabsTradeTabs[0] , true , true);
        }

        SetMainTab(0, true , true);
    }

    public void ClearSlotsActiveTab(List<ItemInstance> oldListItems)
    {
        _activeTab.ClearSlots(oldListItems);
    }

    public void CreateTabs(VisualElement _inventoryTabView, VisualTreeAsset _tabTemplate, VisualTreeAsset _tabHeaderTemplate)
    {
        VisualElement tabHeaderContainer = _inventoryTabView.Q<VisualElement>("tab-header-container");
        VisualElement tabContainer = _inventoryTabView.Q<VisualElement>("tab-content-container");

        if (_inventoryTabView != null & _tabTemplate != null & _tabHeaderTemplate != null & tabContainer != null)
        {


            for (int i = _tabsContentTabs.Count() - 1; i >= 0; i--)
            {
                string tabName = _nameTabs[i];

                VisualElement tabElement = _tabTemplate.CloneTree()[0];


                //delete Scroll View
                tabElement.Clear();
                tabElement.Add(CreateEmtyContentVisualElement());

                tabElement.name = tabName;
                tabElement.AddToClassList("unselected-tab");




                VisualElement tabHeaderElement = _tabHeaderTemplate.CloneTree()[0];
                tabHeaderElement.name = tabName;
                tabHeaderElement.Q<Label>().text = tabName;

                tabHeaderContainer.Add(tabHeaderElement);
                tabContainer.Add(tabElement);

                _tabsContentTabs[i] = new ContentTab(tabName, tabElement, tabHeaderElement, true);

                _tabsContentTabs[i].EventSwitch += OnSwitchEvent;
                _tabsContentTabs[i].EventLeftClick += OnLeftClick;
            }
        }
        else
        {
            // Debug.LogError("CreatorTradingWindows: Not Create Windows tabs!!!!");
        }

        if (_tabsContentTabs.Length > 0)
        {
            SwitchTab(_tabsContentTabs[0] , false , true);
        }

        SetMainTab(0, true , false);
    }

    public VisualElement CreateEmtyContentVisualElement()
    {
        var content = new VisualElement { name = "Content" };
        content.style.flexGrow = 1;
        content.style.flexShrink = 1;
        return content;
    }
}
