using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using VContainer;

public partial class HtmlWindow : L2PopupWindow
{

    private Label _titleLabel;
    private static HtmlWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static HtmlWindow Instance => _instance;

    private VisualElement _content;
    private ScrollView _scrollView;
    private VisualElement _communityTopButtons;

    private bool _isCommunityBoardOpen = false;
    private bool _normalWindowPositionInitialized = false;
    private bool _communityBoardPositionInitialized = false;
    private long _flood = 0;

    private string _normalHtml = "";
    private string _communityHtml = "";























}
