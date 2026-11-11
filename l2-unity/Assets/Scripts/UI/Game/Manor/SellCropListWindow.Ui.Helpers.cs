using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public partial class SellCropListWindow : L2PopupWindow
{
    private void CreateEmptyTable()
    {
        _defaultColumns = GetColumnWindow();
        _creatorTableWindows.CreateTable(_defaultColumns);
    }

    private List<TableColumn> GetColumnWindow()
    {
 
        var harvest = new TableColumn(false, "Harvest Name", 0, new List<string>(), 13);
        var auth = new TableColumn(true, "Lvl", 0, new List<string>(), 0);
        var buy = new TableColumn(true, "Buy Remaining", 0, new List<string>(), 0);
        var price = new TableColumn(true, "Purchase Price", 0, new List<string>(), 0);
        var reward = new TableColumn(true, "Reward", 0, new List<string>(), 13);
        var prop = new TableColumn(false, "My Prop.", 0, new List<string>(), 13);
        var sales = new TableColumn(false, "Sales", 0, new List<string>(), 13);


       return  new List<TableColumn> { harvest, auth, buy, price, reward, prop, sales };
    }

    private void OnCloseButtonMouseUp(MouseUpEvent evt)
    {
        _creatorTableWindows.ClearTable();
        base.HideWindow();
    }
}
