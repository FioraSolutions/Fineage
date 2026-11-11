using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ToolTipSkill : L2PopupWindow, IToolTips
{
    [Inject] ISkillNameTable _skillNames;

    ISkillNameTable SkillNames =>
        _skillNames != null ? _skillNames : TablePacketServices.SkillNames;

    private ShowToolTip _showToolTip;

    private static ToolTipSkill _instance;
    private VisualElement _content;

    private UnityEngine.UIElements.Label _hpLabel;
    private UnityEngine.UIElements.Label _mpLabel;
    private UnityEngine.UIElements.Label _rangeLabel;
    private UnityEngine.UIElements.Label _descriptedLabel;
    private UnityEngine.UIElements.Label _castlabel;
    private UnityEngine.UIElements.Label _typeSkill;
    private UnityEngine.UIElements.Label _nameSkill;
    private UnityEngine.UIElements.Label _lvlSkill;


    //Block
    private VisualElement _hpBlockText;
    private VisualElement _mpBlockText;
    private VisualElement _radiusBlockText;
    private VisualElement _castBlockText;
    private VisualElement _descriptedText;

    private VisualElement _icon;
    private float _heightContent = 0;
    private float _widtchContent = 0;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static ToolTipSkill Instance { get { return _instance; } }





}
