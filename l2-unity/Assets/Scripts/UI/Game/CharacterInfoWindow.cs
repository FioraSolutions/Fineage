using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CharacterInfoWindow : L2PopupWindow
{
    const float BarWidth = 85f;
    // Same column as CP (bag icon sits just left of 156 on myinfo_back).
    const float WeightBarWidth = 85f;

    Label _nameLabel;
    Label _levelLabel;
    Label _clanLabel;
    Label _statusLabel;

    Label _hpLabel;
    Label _mpLabel;
    Label _spLabel;
    Label _expLabel;
    Label _weightLabel;
    Label _cpLabel;

    VisualElement _hpBar;
    VisualElement _hpBarBg;
    VisualElement _mpBar;
    VisualElement _mpBarBg;
    VisualElement _cpBar;
    VisualElement _cpBarBg;
    VisualElement _weightBar;
    VisualElement _weightBarBg;
    VisualElement _expBar;
    VisualElement _expBarBg;

    Label _patkLabel;
    Label _pdefLabel;
    Label _paccLabel;
    Label _pevaLabel;
    Label _pcritLabel;
    Label _patkspdLabel;
    Label _speedLabel;
    Label _matkLabel;
    Label _mdefLabel;
    Label _castspeedLabel;

    Label _strLabel;
    Label _intLabel;
    Label _dexLabel;
    Label _witLabel;
    Label _conLabel;
    Label _menLabel;

    Label _karmaLabel;
    Label _pvpLabel;
    Label _recLabel;
    Label _recLeftLabel;

    Label _windowTitleLabel;
    VisualElement _detailStatusWnd;
    VisualElement _skillsHost;
    VisualElement _actionsHost;
    int _mainTab = 1;

    static CharacterInfoWindow _instance;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static CharacterInfoWindow Instance => _instance;






}
