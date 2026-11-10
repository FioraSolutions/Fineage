using System;

﻿using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Interlude TargetStatusWnd: name + HP (monsters only), live effect icons (PartySpelled /
/// AbnormalStatusUpdate + spoil), expand arrow reveals npcgrp property_list icons
/// (race 4416 + vulnerabilties / passives) with SkillName tooltips.
/// </summary>
public class TargetWindow : L2Window {
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

    /// <summary>Called when a peer AbnormalStatusUpdate lands for the current target.</summary>
    public void RefreshEffectsIfSelected(int objectId)
    {
        TargetManager targets = TargetPacketServices.Targets;
        if (targets == null || !targets.HasTarget())
            return;
        TargetData data = targets.Target;
        if (data?.Identity == null || data.Identity.Id != objectId)
            return;
        _effectsObjectId = -1;
        _effectsSignature = 0;
        BindEffects(data);
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

    void BindEffects(TargetData targetData)
    {
        if (_effectsRow == null)
            return;
        if (targetData?.Identity == null)
        {
            ClearEffects();
            return;
        }

        Entity entity = targetData.GetEntity();
        var list = entity != null ? entity.TargetEffects : null;
        int objectId = targetData.Identity.Id;
        int signature = EffectsSignature(list);
        if (objectId == _effectsObjectId && signature == _effectsSignature)
            return;

        _effectsObjectId = objectId;
        _effectsSignature = signature;
        _effectsRow.Clear();

        if (list == null || list.Count == 0)
        {
            _effectsRow.style.display = DisplayStyle.None;
            return;
        }

        int painted = 0;
        for (int i = 0; i < list.Count; i++)
        {
            EffectHolder effect = list[i];
            if (effect == null || effect.Id <= 0)
                continue;
            if (TryAddEffectIcon(effect.Id, effect.Value))
                painted++;
        }

        _effectsRow.style.display = painted > 0 ? DisplayStyle.Flex : DisplayStyle.None;
    }

    bool TryAddEffectIcon(int skillId, int level)
    {
        int safeLevel = level > 0 ? level : 1;
        Skillgrp skill = TablePacketServices.SkillGrps != null
            ? TablePacketServices.SkillGrps.GetSkill(skillId, safeLevel)
            : null;
        SkillNameData nameData = TablePacketServices.SkillNames != null
            ? TablePacketServices.SkillNames.GetName(skillId, safeLevel)
            : null;

        var icon = new VisualElement();
        icon.AddToClassList("target-effect-icon");

        bool painted = skill != null && L2SlotIconView.TrySetNamedIcon(icon, skill.Icon);
        if (!painted)
            painted = L2SlotIconView.TrySetNamedIcon(icon, "icon.skill" + skillId);
        if (!painted)
            painted = L2SlotIconView.TrySetNamedIcon(icon, "skill" + skillId);
        if (!painted && nameData == null)
            return false;

        string tip = BuildPropertyTooltip(nameData, skillId, safeLevel);
        if (!string.IsNullOrEmpty(tip))
            icon.AddManipulator(new TooltipManipulator(icon, tip));

        _effectsRow.Add(icon);
        return true;
    }

    static int EffectsSignature(System.Collections.Generic.List<EffectHolder> list)
    {
        if (list == null || list.Count == 0)
            return 0;
        unchecked
        {
            int hash = list.Count;
            for (int i = 0; i < list.Count; i++)
            {
                EffectHolder e = list[i];
                if (e == null)
                    continue;
                hash = (hash * 397) ^ e.Id;
                hash = (hash * 397) ^ e.Value;
                hash = (hash * 397) ^ e.Duration;
            }
            return hash;
        }
    }

    void ClearEffects()
    {
        _effectsObjectId = -1;
        _effectsSignature = 0;
        if (_effectsRow == null)
            return;
        _effectsRow.Clear();
        _effectsRow.style.display = DisplayStyle.None;
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
