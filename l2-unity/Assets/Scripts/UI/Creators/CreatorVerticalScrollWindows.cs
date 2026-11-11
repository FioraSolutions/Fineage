using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorVerticalScrollWindows : ICreatorVerticalScroll
{
    public event Action<int, ItemCategory, int> EventLeftClick;
    public event Action<int> EventSwitchTabByIndexOfTab;

    private ScrollView _scrollView;
    private VisualElement _content;
    //private string[] _nameTabs;
    private VisualTreeAsset _templateItems;
    private DataProviderWindows _dataProvider;






    //public void InitTradeTabs(string[] nameTabs)
    //{
    //    _nameTabs = nameTabs ?? new string[0];
   // }








}
