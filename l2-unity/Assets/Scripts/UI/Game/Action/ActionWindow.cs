using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ActionWindow : L2PopupWindow
{
    private const int SLOTS_PER_ROW = 8;
    private VisualTreeAsset _slotTemplate;
    private VisualElement _basicContainer;
    private VisualElement _partyContainer;
    private VisualElement _tokenContainer;
    private VisualElement _socialContainer;
    private List<ActionSlot> _slots;

    private static ActionWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static ActionWindow Instance { get { return _instance; } }



    








    /// <summary>Embed ActionWnd content under Character MainWnd Actions tab (no second popup).</summary>



}
