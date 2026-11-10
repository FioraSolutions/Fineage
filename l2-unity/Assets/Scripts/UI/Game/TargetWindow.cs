using System;

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Interlude TargetStatusWnd: name + HP (monsters only), live effect icons (PartySpelled /
/// AbnormalStatusUpdate + spoil), expand arrow reveals npcgrp property_list icons
/// (race 4416 + vulnerabilties / passives) with SkillName tooltips.
/// </summary>
public partial class TargetWindow : L2Window {
    private Label _nameLabel;
    private VisualElement _HPBar;
    private VisualElement _HPBarBG;
    private VisualElement _effectsRow;
    private VisualElement _propertiesRow;
    private Button _expandButton;

    [SerializeField] private float _targetWindowMinWidth = 175.0f;
    [SerializeField] private float _targetWindowMaxWidth = 300.0f;

    private bool _expanded;
    private int _boundNpcId = -1;
    private int _boundObjectId = -1;
    private int _effectsObjectId = -1;
    private int _effectsSignature;
    private readonly System.Collections.Generic.List<TargetEffectCell> _effectCells =
        new System.Collections.Generic.List<TargetEffectCell>();

    sealed class TargetEffectCell
    {
        public VisualElement Root;
        public Label Timer;
        public float EndsAt;
    }

    private static TargetWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static TargetWindow Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            WireHudCorePacketServices();
        } else {
            Destroy(this);
        }
    }

    private void OnDestroy() {
        _instance = null;
    }

    
    private void Start()
    {
        WireHudCorePacketServices();
    }

    private void WireHudCorePacketServices()
    {
        HudCorePacketServices.Bind(target: this);
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/TargetWindow");
    }

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

    /// <summary>
    /// Interlude TargetStatus: HP gauge only for attackable monsters. Self / other players /
    /// village NPCs show name (and effects) without the red bar.
    /// </summary>
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
