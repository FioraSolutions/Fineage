using System;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using VContainer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DragIcon : L2PopupWindow
{
    public VisualElement icon;


    private static DragIcon _instance;
    private bool _isShow = false;
    [System.Obsolete("Use [Inject] or DragIconPacketServices.Icon instead")]
    public static DragIcon Instance
    {
        get { return _instance; }
    }







}
