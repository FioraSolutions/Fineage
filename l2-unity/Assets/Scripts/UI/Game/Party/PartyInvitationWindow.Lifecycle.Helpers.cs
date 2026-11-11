using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class PartyInvitationWindow : L2PopupWindow
{
    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/JoinPartyWindow");
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
        _invitationText = (Label)GetElementById("invitationText");
        _yesButton = (Button)GetElementById("YesButton");
        _noButton = (Button)GetElementById("NoButton");

        RegisterClickWindowEvent(_windowEle, dragArea);
        _yesButton.RegisterCallback<ClickEvent>(OnYesClicked);
        _noButton.RegisterCallback<ClickEvent>(OnNoClicked);
    }

    public override void ShowWindow()
    {
        base.ShowWindow();

        UpdateInvitationText();
        ResetProgressBar();
        StartCountdown();
    }

    private void UpdateInvitationText()
    {
        _invitationText.text = $"Do you accept {_inviterName}'s party invitation?\n(Item Distribution: {_lootDistribution})"; //todo: get from resources(localization)
    }
}
