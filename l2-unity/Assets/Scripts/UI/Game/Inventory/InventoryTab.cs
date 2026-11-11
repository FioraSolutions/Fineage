using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using static PlayerInventory;


[System.Serializable]
public partial class InventoryTab : L2Tab
{
    private InventorySlot[] _inventorySlots;
    [SerializeField] private int _selectedSlot = -1;
    private VisualElement _contentContainer;
    public List<ItemCategory> _filteredCategories;
    public List<ItemCategory> GetFilterCategories {  get { return _filteredCategories; } }
    public bool MainTab { get; internal set; }

    public override void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader, bool initEmpty, bool isSwitchTab)
    {
        base.Initialize(chatWindowEle, tabContainer, tabHeader, initEmpty, isSwitchTab);

        _selectedSlot = -1;
        _contentContainer = tabContainer.Q<VisualElement>("Content");
        CreateEmptyInventory(initEmpty);
    }






    //temporarily disabling




   




   
 


 




   












}
