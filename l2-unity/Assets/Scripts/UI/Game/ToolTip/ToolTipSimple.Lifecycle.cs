using System;
using System.Collections;
using UnityEngine.UIElements;
using static L2Slot;
using System.Collections.Generic;
using UnityEngine;

public partial class ToolTipSimple : L2PopupWindow, IToolTips
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _showToolTip = new ShowToolTip(this);
            _dataProvider = new TooltipDataProvider();
            _calcPosition = new CalcNewPosition();
            WireToolTipSimplePacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireToolTipSimplePacketServices();
    }

    private void WireToolTipSimplePacketServices()
    {
        ToolTipSimplePacketServices.Bind(window: this);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipObject");
        _windowTemplateString = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipString");
        _windowTemplateSimple = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipSimple");
        _windowTemplateWeapon = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipWeapon");
        _windowTemplateAcccesories = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipAccessories");
        _windowTemplateRequiredAcccesories = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipRequiredAccessories");
        _windowTemplateArmor = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipArmor");
        _windowTemplateSkill = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipSkill");
        _setsElements = LoadAsset("Data/UI/_Elements/Game/ToolTips/Elements/SetsElements");
        _setsEffects = LoadAsset("Data/UI/_Elements/Game/ToolTips/Elements/SetsEffectNameElements");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);



        yield return new WaitForEndOfFrame();


        _content = GetElementByClass("control-box");
        _windowEle.style.display = DisplayStyle.None;

    }

    public void ManualHide()
    {
        StartCoroutine(AllHide());
    }
}
