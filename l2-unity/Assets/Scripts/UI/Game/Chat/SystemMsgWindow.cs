using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SystemMsgWindow : L2Window
{
    const int MaxLines = 80;
    const float DefaultHeight = 110f;
    const float GapAboveChat = 2f;

    static SystemMsgWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static SystemMsgWindow Instance => _instance;

    VisualElement _content;
    ScrollView _list;
    readonly List<Label> _lines = new();
    readonly List<string> _pending = new();
    VisualElement _chatAnchor;
    bool _docked;
    bool _ready;






    /// <summary>Keep flush above the Interlude ChatWnd (same left/width).</summary>






}
