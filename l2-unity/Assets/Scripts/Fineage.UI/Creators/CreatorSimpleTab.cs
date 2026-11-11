using System;
using UnityEngine;
using UnityEngine.UIElements;


public class CreatorSimpleTab : AbstractLoaderTemplate, ICreatorSimpleTab
{
    public event Action<int> EventSwitchTabByIndexOfTab;
    public event Action<int, ItemCategory, int> EventLeftClick;

    private VisualElement _injectTemplate;
    private string[] _nameTabs;
    private ContentTabState[] _tabsContentTabs;
    private ContentTabState _activeTab;

    public void InitContentTabs(string[] nameTabs)
    {
        _nameTabs = nameTabs;
        _tabsContentTabs = new ContentTabState[nameTabs.Length];
    }

    public void CreateTabs(VisualElement inventoryTabView, VisualTreeAsset tabTemplate, VisualTreeAsset tabHeaderTemplate)
    {
        VisualElement tabHeaderContainer = inventoryTabView.Q<VisualElement>("tab-header-container");
        VisualElement tabContainer = inventoryTabView.Q<VisualElement>("tab-content-container");

        if (inventoryTabView != null & tabTemplate != null & tabHeaderTemplate != null & tabContainer != null)
        {
            for (int i = _tabsContentTabs.Length - 1; i >= 0; i--)
            {
                string tabName = _nameTabs[i];

                VisualElement tabElement = tabTemplate.CloneTree()[0];
                tabElement.Clear();
                tabElement.Add(CreateEmptyContentVisualElement());
                tabElement.name = tabName;
                tabElement.AddToClassList("unselected-tab");

                VisualElement tabHeaderElement = tabHeaderTemplate.CloneTree()[0];
                tabHeaderElement.name = tabName;
                tabHeaderElement.Q<Label>().text = tabName;

                tabHeaderContainer.Add(tabHeaderElement);
                tabContainer.Add(tabElement);

                ContentTabState tab = new ContentTabState(tabName, tabElement, tabHeaderElement);
                tab.EventSwitch += OnSwitchEvent;
                _tabsContentTabs[i] = tab;
            }
        }

        if (_tabsContentTabs != null && _tabsContentTabs.Length > 0)
        {
            SwitchTab(0, false, true);
            SetMainTab(0);
        }
    }

    public void SwitchTab(int idTab, bool isTrade, bool useEvent)
    {
        if (_tabsContentTabs == null)
        {
            return;
        }

        if (!ArrayUtils.IsValidIndexArray(_tabsContentTabs, idTab))
        {
            return;
        }

        SwitchTab(_tabsContentTabs[idTab], useEvent);
    }

    public void SetContent(int idTemplate)
    {
        VisualElement tabContent =  GetActiveContent();
        _injectTemplate = GetTemplateById(idTemplate);

        if (_injectTemplate == null)
        {
            // Debug.LogWarning("CreatorSimpleTab> SetContent: Not Fount Inject Template");
        }

        if (tabContent != null & _injectTemplate != null)
        {
            tabContent.Clear();
            tabContent.Add(_injectTemplate);
        }

    }

    public void SetClickActiveTab(int position)
    {
        throw new NotImplementedException();
    }


    public VisualElement[] GetVisualElements(string[] names)
    {
        VisualElement[] elements = new VisualElement[names.Length];

        for (int i =0; i< names.Length; i++)
        {
            string name = names[i];
            if (name == null) continue;
            elements[i] = _injectTemplate.Q<VisualElement>(name);
        }

        return elements;
    }

    void ICreatorSimpleTab.LoadAsset(Func<string, VisualTreeAsset> loaderFunc, string[] loadTemplate)
    {
        LoadAsset(loaderFunc, loadTemplate);
    }

    private void OnSwitchEvent(ContentTabState tab)
    {
        if (_tabsContentTabs == null || _tabsContentTabs.Length <= 1 || tab == null)
        {
            return;
        }

        SwitchTab(tab, true);
    }

    private void SwitchTab(ContentTabState switchTo, bool useEvent)
    {
        if (_activeTab == switchTo)
        {
            return;
        }

        if (_activeTab != null)
        {
            _activeTab.Unselect();
        }

        _activeTab = switchTo;
        if (_activeTab != null)
        {
            _activeTab.Select();
        }

        if (useEvent && _activeTab != null)
        {
            for (int i = 0; i < _tabsContentTabs.Length; i++)
            {
                if (_tabsContentTabs[i] == _activeTab)
                {
                    EventSwitchTabByIndexOfTab?.Invoke(i);
                    break;
                }
            }
        }
    }

    private void SetMainTab(int index)
    {
        if (!ArrayUtils.IsValidIndexArray(_tabsContentTabs, index))
        {
            return;
        }

        _tabsContentTabs[index].SetMain(true);
    }

    private VisualElement GetActiveContent()
    {
        return _activeTab != null ? _activeTab.GetContentElement() : null;
    }

    private static VisualElement CreateEmptyContentVisualElement()
    {
        var content = new VisualElement { name = "Content" };
        content.style.flexGrow = 1;
        content.style.flexShrink = 1;
        return content;
    }

    private sealed class ContentTabState
    {
        private readonly string _tabName;
        private readonly VisualElement _tabContainer;
        private readonly VisualElement _tabHeader;
        private readonly VisualElement _contentContainer;
        private bool _mainTab;

        public event Action<ContentTabState> EventSwitch;

        public ContentTabState(string tabName, VisualElement tabContainer, VisualElement tabHeader)
        {
            _tabName = tabName;
            _tabContainer = tabContainer;
            _tabHeader = tabHeader;
            _contentContainer = tabContainer.Q<VisualElement>("Content");
            tabHeader.RegisterCallback<MouseDownEvent>(
                evt => EventSwitch?.Invoke(this),
                TrickleDown.TrickleDown);
        }

        public VisualElement GetContentElement()
        {
            return _contentContainer;
        }

        public void SetMain(bool main)
        {
            _mainTab = main;
        }

        public void Unselect()
        {
            if (_tabContainer != null && _tabHeader != null)
            {
                _tabContainer.AddToClassList("unselected-tab");
                _tabHeader.RemoveFromClassList("active");
            }
        }

        public void Select()
        {
            if (_tabContainer != null && _tabHeader != null)
            {
                _tabContainer.RemoveFromClassList("unselected-tab");
                _tabHeader.AddToClassList("active");
            }
        }
    }
}
