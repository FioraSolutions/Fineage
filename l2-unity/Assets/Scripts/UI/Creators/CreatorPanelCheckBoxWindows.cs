using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatePanelCheckBoxWindows : ICreatorPanelCheckBox
{

    private const string _checkBoxTemplateName = "Data/UI/_Elements/Template/Elements/Checkbox/DefaultCheckBox";
    private const string _twoPanelsTemplateName = "Data/UI/_Elements/Template/Clan/TwoPanelsCheckBox";
    private List<VisualTreeAsset> _listTemplate  = new List<VisualTreeAsset>();
    private  string DEFAULT_DISABLED_COLOR = "#7f7f7f";
    private Color _colorTextDisabled;
    private CheckBoxRootElements _tempElements;
    private List<SettingCheckBox> _registeredCallbacks = new List<SettingCheckBox>();












}
