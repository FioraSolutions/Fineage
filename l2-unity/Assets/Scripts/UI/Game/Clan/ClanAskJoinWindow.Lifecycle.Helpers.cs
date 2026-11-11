using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ClanAskJoinWindow : L2PopupWindow
{
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
