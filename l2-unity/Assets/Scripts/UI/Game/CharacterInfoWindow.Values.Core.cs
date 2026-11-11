using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System;

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
}
