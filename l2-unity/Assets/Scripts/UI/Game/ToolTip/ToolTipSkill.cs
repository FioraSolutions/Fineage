using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ToolTipSkill : L2PopupWindow, IToolTips
{

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



    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _showToolTip = new ShowToolTip(this);
        }
        else
        {
            Destroy(this);
        }
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipSkill");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);



        yield return new WaitForEndOfFrame();


        _content = GetElementByClass("content");

        //only elements
        _hpLabel = (UnityEngine.UIElements.Label)GetElementByClass("HpLabel");
        _mpLabel = (UnityEngine.UIElements.Label)GetElementByClass("MpLabel");
        _rangeLabel = (UnityEngine.UIElements.Label)GetElementByClass("RangeLabel");
        _descriptedLabel = (UnityEngine.UIElements.Label)GetElementByClass("DescriptedLabel");
        _castlabel = (UnityEngine.UIElements.Label)GetElementByClass("Castlabel");
        _typeSkill = (UnityEngine.UIElements.Label)GetElementByClass("TypeSkill");
        _nameSkill = (UnityEngine.UIElements.Label)GetElementByClass("NameSkill");
        _lvlSkill = (UnityEngine.UIElements.Label)GetElementByClass("LvlSkill");

        //all GroupBox
        _hpBlockText = GetElementByClass("HpText");
        _mpBlockText = GetElementByClass("MpText");
        _radiusBlockText = GetElementByClass("RadiusText");
        _castBlockText = GetElementByClass("TimeText");
        _descriptedText = GetElementByClass("DescriptedText");


        _icon = GetElementByClass("Icon");


        _heightContent = _windowEle.worldBound.height;
        _widtchContent = _windowEle.worldBound.width;
        _windowEle.style.display = DisplayStyle.None;
    }
}
