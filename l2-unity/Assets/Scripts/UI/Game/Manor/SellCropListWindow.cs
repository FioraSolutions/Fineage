using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public partial class SellCropListWindow : L2PopupWindow
{
    private static SellCropListWindow _instance;
    private ICreatorTables _creatorTableWindows;
    private VisualElement _content;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static SellCropListWindow Instance { get { return _instance; } }
    private List<TableColumn> _defaultColumns;












}
