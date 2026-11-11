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
