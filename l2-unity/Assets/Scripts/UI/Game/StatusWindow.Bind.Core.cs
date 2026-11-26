using System;
using System.Collections;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VContainer;

public partial class StatusWindow
{
    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);
        InterludeWindowLayout.Apply(_windowEle, "StatusWnd");

        yield return new WaitForEndOfFrame();

        DragManipulator drag = new DragManipulator(_windowEle, _windowEle);
        _windowEle.AddManipulator(drag);

        _windowEle.RegisterCallback<ClickEvent>(OnStatusWindowClicked);

        _nameLabel = (Label)GetElementById("UserName");
        _levelLabel = (Label)GetElementById("StatusWnd_LevelTextBox");
        _CPTextLabel = (Label)GetElementById("CPBar__text");
        _HPTextLabel = (Label)GetElementById("HPBar__text");
        _MPTextLabel = (Label)GetElementById("MPBar__text");
        _EXPTextLabel = (Label)GetElementById("EXPBar__text");
        _CPBarBG = GetElementById("CPBar");
        _CPBar = GetElementById("CPBar__fill");
        _HPBarBG = GetElementById("HPBar");
        _HPBar = GetElementById("HPBar__fill");
        _MPBarBG = GetElementById("MPBar");
        _MPBar = GetElementById("MPBar__fill");
        _EXPBarBG = GetElementById("EXPBar");
        _EXPBar = GetElementById("EXPBar__fill");

        VisualElement grip = GetElementById("StatusWndRightTex");
        if (grip != null)
        {
            grip.pickingMode = PickingMode.Position;
            grip.AddToClassList("hor-resize-handle");
        }

        float minWidth = _statusWindowMinWidth;
        float layoutWidth;
        float layoutHeight;
        if (InterludeWindowLayout.TryWindowSize("StatusWnd", out layoutWidth, out layoutHeight))
            minWidth = layoutWidth;
        AttachInvisibleHorizontalResize(grip, minWidth, _statusWindowMaxWidth);
    }

    private void OnStatusWindowClicked(ClickEvent evt)
    {
        if (evt.button != 0)
        {
            return;
        }

        SelectPlayerAsTarget();
        evt.StopPropagation();
    }

    private void SelectPlayerAsTarget()
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null || TargetPacketServices.Targets == null)
        {
            return;
        }

        // Zone SelectTarget for self does not fan TargetSelected — set the local target window here.
        TargetPacketServices.Targets.SetTarget(
            new ObjectData(player.gameObject),
            "#ffffff");
        SendSelfTargetAction(player);
    }
}
