using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class TradeRequestWindow : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireTradePacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireTradePacketServices();
    }

    private void WireTradePacketServices()
    {
        TradePacketServices.Bind(tradeRequest: this);
    }

    public void AddData(SendTradeRequestDto request)
    {
        _senderId = request.SenderId;

        var target = Worlds.GetEntityName(_senderId);

        _requesterName = target;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/TradeRequestWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        OnCenterScreen(root);
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        _progressBar = GetElementById("ProgressGauge");
        _progressBarBg = GetElementById("ProgressBg");
        _tradeRequestText = (Label)GetElementById("tradeRequestText");
        _confirmButton = (Button)GetElementById("ConfirmButton");
        _cancelButton = (Button)GetElementById("CancelButton");

        RegisterClickWindowEvent(_windowEle, dragArea);
        _confirmButton.RegisterCallback<ClickEvent>(OnConfirmClicked);
        _cancelButton.RegisterCallback<ClickEvent>(OnCancelClicked);
    }

    public void ShowWindow(float timeoutSeconds = 20f)
    {
        _timeoutSeconds = timeoutSeconds;

        base.ShowWindow();

        UpdateTradeRequestText();
        ResetProgressBar();
        StartCountdown();
    }

    private void UpdateTradeRequestText()
    {
        _tradeRequestText.text = $"{_requesterName} is requesting a trade. Do you wish to continue?";
    }
}
