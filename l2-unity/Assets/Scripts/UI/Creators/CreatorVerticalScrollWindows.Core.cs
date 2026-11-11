using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorVerticalScrollWindows : ICreatorVerticalScroll
{
    public CreatorVerticalScrollWindows()
    {
        _dataProvider = new DataProviderWindows();
    }

    public void CreateTabs(VisualElement content, VisualTreeAsset empty, VisualTreeAsset templateItems)
    {
        _scrollView = (ScrollView)content.Q<ScrollView>("ScrollView");

        if(_scrollView == null & templateItems == null)
        {
            // Debug.LogError("CreatorHorizontalWindows> Not found ScrollView in content element");
        }

        _content = _scrollView.contentContainer;
        _templateItems = templateItems;
    }

    private VisualElement CloneTemplate(VisualTreeAsset templateItems)
    {
        return ToolTipsUtils.CloneOne(templateItems);
    }
}
