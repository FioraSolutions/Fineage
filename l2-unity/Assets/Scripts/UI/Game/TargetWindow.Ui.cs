using System;

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class TargetWindow : L2Window
{
    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        var statusWindowDragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(statusWindowDragArea, _windowEle);
        statusWindowDragArea.AddManipulator(drag);

        var horizontalResizeHandle = GetElementByClass("hor-resize-handle");
        HorizontalResizeManipulator horizontalResize = new HorizontalResizeManipulator(
            horizontalResizeHandle, _windowEle, _targetWindowMinWidth, _targetWindowMaxWidth);
        horizontalResizeHandle.AddManipulator(horizontalResize);

        var closeBtnHandle = (Button) GetElementById("CloseBtn");
        closeBtnHandle.AddManipulator(new ButtonClickSoundManipulator(closeBtnHandle));
        closeBtnHandle.RegisterCallback<MouseUpEvent>(evt => {
            TargetPacketServices.Targets?.ClearTarget();
        });

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
        _windowEle.style.left = Screen.width / 2f - _windowEle.resolvedStyle.width / 2f;
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
                L2ResourceBarView.SetFill(
                    _HPBar,
                    _HPBarBG,
                    PlayerResourceCaps.BarFillRatio(
                        targetData.Status.GetHp(),
                        targetData.Stats.MaxHp),
                    _targetWindowMinWidth);

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

    static bool ShouldHideHpBar(TargetData targetData)
    {
        EntityIdentity identity = targetData?.Identity;
        if (identity == null)
            return true;
        if (identity.Owned)
            return true;
        if (identity.EntityType == EntityType.Player || identity.EntityType == EntityType.User)
            return true;
        return identity.IsHideHpBar;
    }

    private void TooogleHide(bool isHideHpBar)
    {
        if (isHideHpBar){
            HideBar();
        }
        else{
            ShowBar();
        }
    }

    private void HideBar()
    {
        _HPBarBG.style.display = DisplayStyle.None;
        _HPBar.style.display = DisplayStyle.None;
    }

    private void ShowBar()
    {
        _HPBarBG.style.display = DisplayStyle.Flex;
        _HPBar.style.display = DisplayStyle.Flex;
    }
}
