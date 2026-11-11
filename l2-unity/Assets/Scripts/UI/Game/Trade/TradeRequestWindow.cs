using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class TradeRequestWindow : L2PopupWindow
{
    private static TradeRequestWindow _instance;
    private VisualElement _progressBar;
    private VisualElement _progressBarBg;
    private Label _tradeRequestText;
    private Button _confirmButton;
    private Button _cancelButton;

    private string _requesterName;
    private float _timeoutSeconds;
    private int _senderId;
    private Coroutine _countdownCoroutine;

    [Inject] World _world;

    private World Worlds
    {
        get { return _world != null ? _world : WorldApplyPacketServices.GameWorld; }
    }

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static TradeRequestWindow Instance
    {
        get { return _instance; }
    }

    public event Action<bool> OnTradeResponse; // true = подтверждено, false = отменено



















}
