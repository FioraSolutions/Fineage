using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VContainer;

using UnityEngine.UIElements;

public partial class ShowListWindow : L2PopupWindow
{

    private static ShowListWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static ShowListWindow Instance { get { return _instance; } }
    private Button _СЃancelButton;
    private Button _okButton;
    private DropdownField _dropdown;
    private Dictionary<string, int> _players;
    private string _select = "";












}
