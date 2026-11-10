using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Interlude <c>DetailStatusWnd</c> (256×335) hosted under MainWnd tab 1 (Status).
/// Coords/textures from <c>interface.xdat</c> + L2UI_CH3.PlayerStatusWnd.
/// </summary>
public class CharacterInfoWindow : L2PopupWindow
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

    static CharacterInfoWindow _instance;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static CharacterInfoWindow Instance => _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireHudCorePacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void Start()
    {
        WireHudCorePacketServices();
    }

    void WireHudCorePacketServices()
    {
        HudCorePacketServices.Bind(characterInfo: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/CharacterInfoWindow");
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        VisualElement dragArea = GetElementByClass("drag-area");
        dragArea.AddManipulator(new DragManipulator(dragArea, _windowEle));

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterCloseWindowEventByName("Close-Window-Button");
        RegisterClickWindowEvent(_windowEle, dragArea);

        ApplyInterludeTextures();
    }

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
    }

    /// <summary>
    /// UITK <c>resource()</c> often fails for Interlude sprites — stamp via Resources.Load.
    /// </summary>
    void ApplyInterludeTextures()
    {
        Paint("Left-Frame-Element", "Data/UI/Frame/Interlude/FrameBackLeft");
        Paint("Center-Frame-Element", "Data/UI/Frame/Interlude/FrameBackMid");
        Paint("Right-Frame-Element", "Data/UI/Frame/Interlude/FrameBackRight");
        Paint("Minimize-Window-Button", "Data/UI/Frame/Interlude/FrameMiniBtn");
        Paint("Close-Window-Button", "Data/UI/Frame/Interlude/FrameCloseBtn");

        Paint("MyInfoBack", "Data/UI/Status/Interlude/Status_Interlude_MyInfo_Back");
        Paint("MainWndTabBack", "Data/UI/Status/Interlude/Status_Interlude_MainWndTabBack");

        for (int i = 1; i <= 5; i++)
        {
            string btnPath = i == 1
                ? "Data/UI/Status/Interlude/Status_Interlude_MainWndTabBtn1On"
                : $"Data/UI/Status/Interlude/Status_Interlude_MainWndTabBtn{i}";
            Paint($"MainWndTab{i}", btnPath);
            Paint($"MainWndTabIcon{i}", $"Data/UI/Status/Interlude/Status_Interlude_MainWndTabIcon{i}");
        }

        // myinfo_back troughs are blanked; paint empty tracks + fills (Interlude ps_*).
        Paint("HpBg", "Data/UI/Status/Interlude/Status_Interlude_Ps_HpBar_Back");
        Paint("MpBg", "Data/UI/Status/Interlude/Status_Interlude_Ps_MpBar_Back");
        Paint("CpBgB", "Data/UI/Status/Interlude/Status_Interlude_Ps_CpBar_Back");
        Paint("ExpBg", "Data/UI/Status/Interlude/Status_Interlude_Ps_ExpBar_Back");
        ClearBg("WeightBg"); // weight trough + bag stay on myinfo_back
        Paint("HpGauge", "Data/UI/Status/Interlude/Status_Interlude_Ps_HpBar");
        Paint("MpGauge", "Data/UI/Status/Interlude/Status_Interlude_Ps_MpBar");
        Paint("CpGaugeB", "Data/UI/Status/Interlude/Status_Interlude_Ps_CpBar");
        Paint("ExpGauge", "Data/UI/Status/Interlude/Status_Interlude_Ps_ExpBar");
        Paint("WeightGauge", "Data/UI/Status/Interlude/Status_Interlude_Ps_WeightBar1");
    }

    void ClearBg(string elementName)
    {
        VisualElement ele = _windowEle?.Q(elementName);
        if (ele != null)
            ele.style.backgroundImage = StyleKeyword.None;
    }

    void Paint(string elementName, string resourcesPath)
    {
        VisualElement ele = _windowEle?.Q(elementName);
        if (ele == null)
            return;
        L2SlotIconView.TrySetResourcesBackground(ele, resourcesPath);
        // Stretch the 8px Interlude strip across the trough; ScaleToFit letterboxes a gap on the right.
        ele.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
    }

    public void UpdateValues()
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null)
            return;

        StartCoroutine(UpdateWithDelayInterlude(player));
    }

    IEnumerator UpdateWithDelayInterlude(PlayerEntity player)
    {
        yield return new WaitForEndOfFrame();

        var stats = (PlayerStats)player.Stats;
        var status = (PlayerStatus)player.Status;

        UpdatePlayerInterlude(player.Identity, stats);
        UpdateStatsInterlude(stats);
        UpdateCombatValuesInterlude(stats);
        UpdateBarsInterlude(status, stats);
        UpdateSocialInterlude(stats);
    }

    void UpdatePlayerInterlude(EntityIdentity identity, PlayerStats stats)
    {
        if (_nameLabel != null)
            _nameLabel.text = identity.Name;

        if (_clanLabel != null)
            _clanLabel.text = "No Clan";

        // Interlude txtLvName: "14 Dwarven Fighter"
        if (_levelLabel != null)
            _levelLabel.text = $"{stats.Level} {LobbyCharacterText.ClassLabel(identity.PlayerClass)}";

        if (_statusLabel != null)
            _statusLabel.text = "Vagabond";
    }

    void UpdateStatsInterlude(PlayerStats stats)
    {
        Set(_strLabel, stats.Str);
        Set(_intLabel, stats.Int);
        Set(_dexLabel, stats.Dex);
        Set(_witLabel, stats.Wit);
        Set(_conLabel, stats.Con);
        Set(_menLabel, stats.Men);
    }

    void UpdateCombatValuesInterlude(PlayerStats stats)
    {
        // Interlude 5+5 — no M.Acc / M.Crit / separate P.Eva column beyond Evasion.
        Set(_patkLabel, stats.PAtk);
        Set(_pdefLabel, stats.PDef);
        Set(_paccLabel, stats.PAccuracy);
        Set(_pcritLabel, stats.PCritical);
        Set(_patkspdLabel, stats.PAtkSpd);

        Set(_matkLabel, stats.MAtk);
        Set(_mdefLabel, stats.MDef);
        Set(_pevaLabel, stats.PEvasion);
        Set(_speedLabel, stats.Speed);
        Set(_castspeedLabel, stats.MAtkSpd);
    }

    void UpdateSocialInterlude(PlayerStats stats)
    {
        if (_karmaLabel != null)
            _karmaLabel.text = stats.Karma.ToString();
        if (_pvpLabel != null)
            _pvpLabel.text = $"{stats.PvpKills} / {stats.PkKills}";
        if (_recLabel != null)
            _recLabel.text = "0";
        if (_recLeftLabel != null)
            _recLeftLabel.text = "0";
    }

    void UpdateBarsInterlude(PlayerStatus status, PlayerStats stats)
    {
        int hpMax = PlayerResourceCaps.EffectiveMax(stats.MaxHp, status.GetHp());
        int hpCur = PlayerResourceCaps.DisplayCurrent(status.GetHp(), hpMax);
        int mpMax = PlayerResourceCaps.EffectiveMax(stats.MaxMp, status.GetMp());
        int mpCur = PlayerResourceCaps.DisplayCurrent(status.GetMp(), mpMax);
        int cpMax = PlayerResourceCaps.EffectiveMax(stats.MaxCp, status.Cp);
        int cpCur = PlayerResourceCaps.DisplayCurrent(status.Cp, cpMax);

        if (_hpLabel != null)
            _hpLabel.text = $"{hpCur}/{hpMax}";
        if (_mpLabel != null)
            _mpLabel.text = $"{mpCur}/{mpMax}";
        if (_cpLabel != null)
            _cpLabel.text = $"{cpCur}/{cpMax}";
        if (_spLabel != null)
            _spLabel.text = ((long)stats.Sp).ToString();

        float expRatio = LevelExpRatio(stats);
        if (_expLabel != null)
            _expLabel.text = $"{expRatio * 100f:0.00}%";

        float weightRatio = stats.MaxWeight > 0 ? (float)stats.CurrWeight / stats.MaxWeight : 0f;
        if (_weightLabel != null)
            _weightLabel.text = $"{weightRatio * 100f:0.00}%";

        L2ResourceBarView.SetFillFixed(
            _hpBar, _hpBarBg, PlayerResourceCaps.BarFillRatio(status.GetHp(), stats.MaxHp), BarWidth);
        L2ResourceBarView.SetFillFixed(
            _mpBar, _mpBarBg, PlayerResourceCaps.BarFillRatio(status.GetMp(), stats.MaxMp), BarWidth);
        L2ResourceBarView.SetFillFixed(
            _cpBar, _cpBarBg, PlayerResourceCaps.BarFillRatio(status.Cp, stats.MaxCp), BarWidth);

        if (expRatio <= 0.0001f)
            ClearFill(_expBar);
        else
        {
            Paint("ExpGauge", "Data/UI/Status/Interlude/Status_Interlude_Ps_ExpBar");
            L2ResourceBarView.SetFillFixed(_expBar, _expBarBg, expRatio, BarWidth);
        }

        if (weightRatio <= 0.0001f)
            ClearFill(_weightBar);
        else
        {
            ApplyWeightBarSprite(weightRatio);
            L2ResourceBarView.SetFillFixed(_weightBar, _weightBarBg, weightRatio, WeightBarWidth);
        }
    }

    static void ClearFill(VisualElement bar)
    {
        if (bar == null)
            return;
        bar.style.width = 0;
        bar.style.backgroundImage = StyleKeyword.None;
    }

    /// <summary>Same within-level formula as <see cref="StatusWindow"/> (0..1).</summary>
    static float LevelExpRatio(PlayerStats stats)
    {
        int lvl = stats.Level;
        long cur = LevelServer.GetExp(lvl);
        long next = LevelServer.GetExp(lvl + 1);
        if (cur < 0 || next <= cur)
            return 0f;
        double r = (stats.Exp - cur) / (double)(next - cur);
        if (double.IsNaN(r))
            return 0f;
        return Mathf.Clamp01((float)r);
    }

    void ApplyWeightBarSprite(float ratio)
    {
        if (_weightBar == null)
            return;

        int tier = 1;
        if (ratio >= 0.8f) tier = 5;
        else if (ratio >= 0.66f) tier = 4;
        else if (ratio >= 0.5f) tier = 3;
        else if (ratio >= 0.33f) tier = 2;

        L2SlotIconView.TrySetResourcesBackground(
            _weightBar,
            $"Data/UI/Status/Interlude/Status_Interlude_Ps_WeightBar{tier}");
    }

    static void Set(Label label, int value)
    {
        if (label != null)
            label.text = value.ToString();
    }
}
