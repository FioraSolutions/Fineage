using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UnityEngine.UIElements;

public partial class L2GameUI : L2UI
{
    private NativeFunctions.NativeCoords _lastMousePosition;
    [SerializeField] private bool _mouseEnabled = true;

    private List<L2PopupWindow> _openedWindows;

    private static L2GameUI _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static L2GameUI Instance { get { return _instance; } }














}
