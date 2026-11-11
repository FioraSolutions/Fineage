using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ClanAskJoinWindow : L2PopupWindow
{
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            ClanPacketServices.Bind(ask: this);
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        ClanPacketServices.Bind(ask: this);
    }

    public void Present(NetClanAskJoin ask)
    {
        _requestorObjectId = ask.RequestorObjectId;
        string who = string.IsNullOrEmpty(ask.RequestorName) ? "A player" : ask.RequestorName;
        string clan = string.IsNullOrEmpty(ask.ClanName) ? "a clan" : ask.ClanName;
        if (_invitationText == null)
        {
            _pendingShow = true;
            _pendingText = who + " invites you to join the clan " + clan + ".";
            return;
        }

        _invitationText.text = who + " invites you to join the clan " + clan + ".";
        ShowWindow();
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ClanAskJoinWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();
        OnCenterScreen(root);
        if (_pendingShow)
        {
            _pendingShow = false;
            if (_invitationText != null && !string.IsNullOrEmpty(_pendingText))
                _invitationText.text = _pendingText;
            ShowWindow();
        }
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);
        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        _progressBar = GetElementById("ProgressGauge");
        _invitationText = (Label)GetElementById("invitationText");
        _yesButton = (Button)GetElementById("YesButton");
        _noButton = (Button)GetElementById("NoButton");
        RegisterClickWindowEvent(_windowEle, dragArea);
        _yesButton.RegisterCallback<ClickEvent>(OnYesClicked);
        _noButton.RegisterCallback<ClickEvent>(OnNoClicked);
    }
}
