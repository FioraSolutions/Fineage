using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CharacterInfoWindow
{
    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();

        _nameLabel = GetLabelById("CharacterNameLabel");
        _levelLabel = GetLabelById("LvlLabelName");
        _clanLabel = GetLabelById("ClanLabelName");
        _statusLabel = GetLabelById("StatusLabelName");

        _hpLabel = GetLabelById("HpLabel");
        _mpLabel = GetLabelById("MpLabel");
        _spLabel = GetLabelById("SpLabel");
        _expLabel = GetLabelById("ExpLabel");
        _weightLabel = GetLabelById("WeightLabel");
        _cpLabel = GetLabelById("CpLabelB");

        _hpBar = GetElementById("HpGauge");
        _hpBarBg = GetElementById("HpBg");
        _mpBar = GetElementById("MpGauge");
        _mpBarBg = GetElementById("MpBg");
        _cpBar = GetElementById("CpGaugeB");
        _cpBarBg = GetElementById("CpBgB");
        _weightBar = GetElementById("WeightGauge");
        _weightBarBg = GetElementById("WeightBg");
        _expBar = GetElementById("ExpGauge");
        _expBarBg = GetElementById("ExpBg");

        _patkLabel = GetLabelById("PAtkLabel");
        _pdefLabel = GetLabelById("PDefLabel");
        _paccLabel = GetLabelById("PAccuracyLabel");
        _pevaLabel = GetLabelById("PEvasionLabel");
        _pcritLabel = GetLabelById("PCriticalLabel");
        _patkspdLabel = GetLabelById("PAtkSpdLabel");
        _speedLabel = GetLabelById("PSpeedLabel");
        _matkLabel = GetLabelById("MAtkLabel");
        _mdefLabel = GetLabelById("MDefLabel");
        _castspeedLabel = GetLabelById("MCastingLabel");

        _strLabel = GetLabelById("StrLabel");
        _intLabel = GetLabelById("IntLabel");
        _dexLabel = GetLabelById("DEXLabel");
        _witLabel = GetLabelById("WITLabel");
        _conLabel = GetLabelById("CONLabel");
        _menLabel = GetLabelById("MENLabel");

        _karmaLabel = GetLabelById("KarmaLabel");
        _pvpLabel = GetLabelById("PvpLabel");
        _recLabel = GetLabelById("RecLabel");
        _recLeftLabel = GetLabelById("RecLeftLabel");
        _windowTitleLabel = GetLabelById("windows-name-label");
        _detailStatusWnd = GetElementById("DetailStatusWnd");
        _skillsHost = GetElementById("SkillsHost");
        _actionsHost = GetElementById("ActionsHost");

        // Interlude MainWnd tabs: Status / Skills / Actions / Clan / Quest.
        // MenuWnd only has Char/Inventory/Map/System — these tabs are the entry points.
        WireMainWndTab(1, SelectStatusTab);
        WireMainWndTab(2, OpenSkillsTab);
        WireMainWndTab(3, OpenActionsTab);
        WireMainWndTab(4, OpenClanTab);
        WireMainWndTab(5, OpenQuestTab);
        SelectMainTab(1, updateTitle: true);
        ShowMainPanel(1);
    }
}
