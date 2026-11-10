using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CharacterInfoWindow
{
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
