

using System;
using System.Collections.Generic;

using System.Linq;
using UnityEngine;

using UnityEngine.UIElements;


public partial class CreatorTableWindows : ICreatorTables
{
    private VisualElement _content;
    private const string _templateNameTable = "Data/UI/_Elements/Template/TableList";
    private const string _templateHeaderItem = "Data/UI/_Elements/Template/TableHeaderItem";
    private const string _templateColumnInside = "Data/UI/_Elements/Template/TableHeaderName";
    private const string _templateRowsName = "Data/UI/_Elements/Template/TableRows";
    private const string _templateRowBlackName = "Data/UI/_Elements/Template/TableRowBlack";



    private const string _tableListViewName = "table_listView";
    private const string _tableBodyListName = "table_body";
    private const string _tableHeaderListName = "table_header";
    private const string _tableHeaderLabelName = "columnLabel";
    private const string _tableHeaderBorder = "columnBorder";


    private VisualTreeAsset _templateTable;
    private VisualTreeAsset _templateHeader;
    private VisualTreeAsset _templateInside;
    private VisualTreeAsset _templateRows;

    private VisualTreeAsset _templateRowBlack;


    private int _lastSelectIndex = -1;
    private int _currentSelectIndex = -1;
    private VisualElement _lastSelectElement;

    private VisualElement _table;
    private List<VisualElement> _tableHeHeaderItem;
    private VisualElement containerHeader;

    private ListView _listView;
    private ScrollView _innerScrollView;
    private List<TableColumn> _columnsList;
    private Texture2D _defaultCursor;
    public int _index_row = 0;
    public event Action<int , string> OnRowClicked;
    private TableFunction _functions;
}
