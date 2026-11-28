using System;

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class TargetWindow : L2Window
{
    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);
        InterludeWindowLayout.Apply(_windowEle, "TargetStatusWnd");

        yield return new WaitForEndOfFrame();

        DragManipulator drag = new DragManipulator(_windowEle, _windowEle);
        _windowEle.AddManipulator(drag);

        float minWidth = _targetWindowMinWidth;
        float layoutWidth;
        float layoutHeight;
        if (InterludeWindowLayout.TryWindowSize("TargetStatusWnd", out layoutWidth, out layoutHeight))
            minWidth = layoutWidth;
        VisualElement horizontalResizeHandle = GetElementByClass("hor-resize-handle");
        AttachInvisibleHorizontalResize(horizontalResizeHandle, minWidth, _targetWindowMaxWidth);

        Button closeBtnHandle = (Button)GetElementById("btnClose");
        if (closeBtnHandle != null)
        {
            closeBtnHandle.AddManipulator(new ButtonClickSoundManipulator(closeBtnHandle));
            closeBtnHandle.RegisterCallback<MouseUpEvent>(evt => {
                TargetPacketServices.Targets?.ClearTarget();
            });
        }

        _nameLabel = (Label)GetElementById("UserName");
        _HPBar = GetElementById("barHP__fill");
        _HPBarBG = GetElementById("barHP");
        _effectsRow = GetElementById("EffectsRow");
        _propertiesRow = GetElementById("NpcInfo");
        _expandButton = GetElementById("ExpandBtn") as Button;

        if (_expandButton != null)
        {
            _expandButton.AddManipulator(new ButtonClickSoundManipulator(_expandButton));
            _expandButton.RegisterCallback<MouseDownEvent>(OnExpandPressed, TrickleDown.TrickleDown);
            _expandButton.style.display = DisplayStyle.None;
        }

        if (_propertiesRow != null)
            _propertiesRow.style.display = DisplayStyle.None;

        RefreshPlateHeight();
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
            if (CombatTrace.Enabled)
                TraceTargetHp(targetData);
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
            ClearEffects();
            ClearProperties();
            HideWindow();
        }
    }

    int _tracedHpTargetId = -1;
    double _tracedHp = double.NaN;

    // Combat trace: the moment the target window's HP gauge value changes (what the player sees drop).
    void TraceTargetHp(TargetData targetData)
    {
        int id = targetData.Identity != null ? targetData.Identity.Id : 0;
        double hp = targetData.Status != null ? targetData.Status.GetHp() : double.NaN;
        if (id == _tracedHpTargetId && hp.Equals(_tracedHp))
            return;
        bool retarget = id != _tracedHpTargetId;
        _tracedHpTargetId = id;
        _tracedHp = hp;
        if (!retarget)
            CombatTrace.Write("ui.target-hp", "tgt=" + id + " hp=" + hp.ToString("F0") +
                " max=" + (targetData.Stats != null ? targetData.Stats.MaxHp : 0));
    }
}
