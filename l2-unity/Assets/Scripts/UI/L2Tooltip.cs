using System;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using System.Collections;

public partial class L2ToolTip : L2PopupWindow
{

    private Label _title;
    private VisualElement _tooltipTarget;
    private Coroutine _updateStyleCoroutine;

    private static L2ToolTip _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static L2ToolTip Instance { get { return _instance; } }



    






}
