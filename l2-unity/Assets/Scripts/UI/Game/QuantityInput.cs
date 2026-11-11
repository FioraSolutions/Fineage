using System;
using System.Linq;
using UnityEngine.UIElements;
using static L2Slot;
using VContainer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class QuantityInput : L2PopupWindow
{
    private static QuantityInput _instance;
    private object _parentWindow;
    private TextField _userInput;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static QuantityInput Instance { get { return _instance; } }
    public event Action<string> OnButtonOk; 
    public event Action OnButtonClose;
    private int _count;


}
