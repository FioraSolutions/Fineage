using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Interlude TargetStatusWnd: name + HP, expand arrow reveals npcgrp property_list icons
/// (race 4416 + vulnerabilties / passives) with SkillName tooltips.
/// </summary>
public class TargetWindow : L2Window {
    private Label _nameLabel;
    private VisualElement _HPBar;
    private VisualElement _HPBarBG;
    private VisualElement _propertiesRow;
    private Button _expandButton;

    [SerializeField] private float _targetWindowMinWidth = 175.0f;
    [SerializeField] private float _targetWindowMaxWidth = 300.0f;

    private bool _expanded;
    private int _boundNpcId = -1;
    private int _boundObjectId = -1;

    private static TargetWindow _instance;
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

                TooogleHide(targetData.Identity.IsHideHpBar);
            }

            BindProperties(targetData);
        } else {
            if (!_isWindowHidden) {
                AudioPacketServices.Audio.PlayUISound("window_close");
            }
            ClearProperties();
            HideWindow();
        }
    }

    void OnExpandPressed(MouseDownEvent evt)
    {
        evt.StopPropagation();
        SetExpanded(!_expanded);
    }

    void SetExpanded(bool expanded)
    {
        _expanded = expanded;
        if (_windowEle == null)
            return;

        if (_expanded)
        {
            _windowEle.AddToClassList("expanded");
            if (_expandButton != null)
                _expandButton.AddToClassList("expanded");
            if (_propertiesRow != null)
                _propertiesRow.style.display = DisplayStyle.Flex;
        }
        else
        {
            _windowEle.RemoveFromClassList("expanded");
            if (_expandButton != null)
                _expandButton.RemoveFromClassList("expanded");
            if (_propertiesRow != null)
                _propertiesRow.style.display = DisplayStyle.None;
        }
    }

    void BindProperties(TargetData targetData)
    {
        if (targetData == null || targetData.Identity == null)
        {
            ClearProperties();
            return;
        }

        int objectId = targetData.Identity.Id;
        int npcId = targetData.Identity.NpcId;
        if (objectId == _boundObjectId && npcId == _boundNpcId)
            return;

        _boundObjectId = objectId;
        _boundNpcId = npcId;

        if (_propertiesRow != null)
            _propertiesRow.Clear();

        Npcgrp npcgrp = npcId > 0 && TablePacketServices.NpcGrps != null
            ? TablePacketServices.NpcGrps.GetNpcgrp(npcId)
            : null;
        int[] skillIds = npcgrp != null ? npcgrp.PropertySkillIds : null;
        int[] skillLevels = npcgrp != null ? npcgrp.PropertySkillLevels : null;

        bool hasProps = skillIds != null && skillLevels != null
            && skillIds.Length > 0 && skillIds.Length == skillLevels.Length;

        if (!hasProps || _propertiesRow == null)
        {
            if (_expandButton != null)
                _expandButton.style.display = DisplayStyle.None;
            SetExpanded(false);
            return;
        }

        int iconsAdded = 0;
        for (int i = 0; i < skillIds.Length; i++)
        {
            if (TryAddPropertyIcon(skillIds[i], skillLevels[i]))
                iconsAdded++;
        }

        if (iconsAdded == 0)
        {
            if (_expandButton != null)
                _expandButton.style.display = DisplayStyle.None;
            SetExpanded(false);
            return;
        }

        if (_expandButton != null)
            _expandButton.style.display = DisplayStyle.Flex;

        // Keep prior expand preference when retargeting another mob with properties.
        if (_expanded)
            _propertiesRow.style.display = DisplayStyle.Flex;
        else
            _propertiesRow.style.display = DisplayStyle.None;
    }

    bool TryAddPropertyIcon(int skillId, int level)
    {
        Skillgrp skill = TablePacketServices.SkillGrps != null
            ? TablePacketServices.SkillGrps.GetSkill(skillId, level)
            : null;
        SkillNameData nameData = TablePacketServices.SkillNames != null
            ? TablePacketServices.SkillNames.GetName(skillId, level)
            : null;

        // Classic property_list can reference skills without Skillgrp rows — still show name if known.
        var icon = new VisualElement();
        icon.AddToClassList("target-property-icon");

        bool painted = skill != null && L2SlotIconView.TrySetNamedIcon(icon, skill.Icon);
        if (!painted)
            painted = L2SlotIconView.TrySetNamedIcon(icon, "skill" + skillId);

        if (!painted && nameData == null)
            return false;

        string tip = BuildPropertyTooltip(nameData, skillId, level);
        if (!string.IsNullOrEmpty(tip))
            icon.AddManipulator(new TooltipManipulator(icon, tip));

        _propertiesRow.Add(icon);
        return true;
    }

    static string BuildPropertyTooltip(SkillNameData nameData, int skillId, int level)
    {
        if (nameData == null)
            return "Skill " + skillId + " Lv." + level;

        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(nameData.Name))
            sb.Append(nameData.Name);
        if (!string.IsNullOrEmpty(nameData.Desc))
        {
            if (sb.Length > 0)
                sb.Append('\n');
            sb.Append(nameData.Desc);
        }
        return sb.Length > 0 ? sb.ToString() : ("Skill " + skillId + " Lv." + level);
    }

    void ClearProperties()
    {
        _boundObjectId = -1;
        _boundNpcId = -1;
        if (_propertiesRow != null)
            _propertiesRow.Clear();
        if (_expandButton != null)
            _expandButton.style.display = DisplayStyle.None;
        SetExpanded(false);
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
