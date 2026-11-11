using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorTabsWindows : AbstractCreator, ICreator
{
    private bool _useAllTabs;
    private ICreatorTables _creatorTable;
    private List<TableColumn> _dataColumn;
    public event Action<int> EventSwitchTabByIndexOfTab;










    //Tabs do not own it, this is a separate element at the very bottom if you need to fill something with buttons or something else depending on the tab
}
