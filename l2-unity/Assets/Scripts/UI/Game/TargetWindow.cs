using System;

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using System.Collections.Generic;

public partial class TargetWindow : L2Window
{
    [Inject] ISkillNameTable _skillNames;
    [Inject] ISkillgrpLookup _skillGrps;

    ISkillNameTable SkillNames =>
        _skillNames != null ? _skillNames : TablePacketServices.SkillNames;

    ISkillgrpLookup SkillGrps =>
        _skillGrps != null ? _skillGrps : TablePacketServices.SkillGrps;

    private Label _nameLabel;
    private VisualElement _HPBar;
    private VisualElement _HPBarBG;
    private VisualElement _effectsRow;
    private VisualElement _propertiesRow;
    private Button _expandButton;

    [SerializeField] private float _targetWindowMinWidth = 176.0f;
    [SerializeField] private float _targetWindowMaxWidth = 420.0f;

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
        public TargetAbnormalTooltipManipulator Tooltip;
        public float EndsAt;
    }

    private static TargetWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static TargetWindow Instance { get { return _instance; } }

    /// <summary>
    /// Interlude TargetStatusWnd: barHP only for attackable monsters. Self / other characters /
    /// village NPCs show the name (and their own abnormal icons) without the red gauge.
    /// </summary>
    internal static bool ShouldHideHpBar(TargetData targetData)
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
}
