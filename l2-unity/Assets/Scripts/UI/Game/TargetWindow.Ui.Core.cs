using System;

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class TargetWindow : L2Window
{
    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        // The whole Interlude SmallWnd plate is the drag handle (as in the client: no title bar).
        var statusWindowDragArea = GetElementById("ChromeRow");
        if (statusWindowDragArea != null)
        {
            DragManipulator drag = new DragManipulator(statusWindowDragArea, _windowEle);
            statusWindowDragArea.AddManipulator(drag);
        }

        var horizontalResizeHandle = GetElementByClass("hor-resize-handle");
        if (horizontalResizeHandle != null)
        {
            HorizontalResizeManipulator horizontalResize = new HorizontalResizeManipulator(
                horizontalResizeHandle, _windowEle, _targetWindowMinWidth, _targetWindowMaxWidth);
            horizontalResizeHandle.AddManipulator(horizontalResize);
        }

        var closeBtnHandle = (Button) GetElementById("CloseBtn");
        if (closeBtnHandle != null)
        {
            closeBtnHandle.AddManipulator(new ButtonClickSoundManipulator(closeBtnHandle));
            closeBtnHandle.RegisterCallback<MouseUpEvent>(evt => {
                TargetPacketServices.Targets?.ClearTarget();
            });
        }

        _nameLabel = (Label)GetElementById("TargetName");
        _HPBar = GetElementById("HPBar");
        _HPBarBG = GetElementById("HPBarBG");
        _effectsRow = GetElementById("EffectsRow");
        _propertiesRow = GetElementById("PropertiesRow");
        _expandButton = GetElementById("ExpandBtn") as Button;

        if (_expandButton != null)
        {
            _expandButton.AddManipulator(new ButtonClickSoundManipulator(_expandButton));
            _expandButton.RegisterCallback<MouseDownEvent>(OnExpandPressed, TrickleDown.TrickleDown);
            _expandButton.style.display = DisplayStyle.None;
        }

        if (_propertiesRow != null)
            _propertiesRow.style.display = DisplayStyle.None;

        _windowEle.style.position = Position.Absolute;
        _windowEle.style.width = _targetWindowMinWidth;
        _windowEle.style.left = Screen.width / 2f - _targetWindowMinWidth / 2f;
        _windowEle.style.top = 0;
    }

    private void FixedUpdate()
    {
        if(_windowEle == null) {
            return;
        }

        TargetManager targets = TargetPacketServices.Targets;
        if (targets != null && targets.HasTarget())
        {
            ShowWindow();

            TargetData targetData = targets.Target;
            if(_nameLabel != null) {
                _nameLabel.text = targetData.Identity.Name;
                _nameLabel.style.color = targetData.GetColorName();
            }
            if(_HPBarBG != null && _HPBar != null) {
                // Percentage fill: the 8x6 barHP strip stays exact at any plate width.
                L2ResourceBarView.SetFill(
                    _HPBar,
                    _HPBarBG,
                    PlayerResourceCaps.BarFillRatio(
                        targetData.Status.GetHp(),
                        targetData.Stats.MaxHp));

                TooogleHide(ShouldHideHpBar(targetData));
            }

            BindEffects(targetData);
            TickEffectTimers();
            BindProperties(targetData);
        } else {
            if (!_isWindowHidden) {
                AudioPacketServices.Audio.PlayUISound("window_close");
            }
            ClearEffects();
            ClearProperties();
            HideWindow();
        }
    }
}
