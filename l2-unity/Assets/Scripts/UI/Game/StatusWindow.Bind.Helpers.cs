using System;

public partial class StatusWindow
{
    private void SendSelfTargetAction(PlayerEntity player)
    {
        if (player.Identity == null || !ZoneCommands.IsConnected)
        {
            return;
        }

        var position = player.transform.position;
        var l2jpos = VectorUtils.ConvertPosUnityToL2j(position);

        ZoneCommands.Send(new ClickActionCommand(
            player.Identity.Id,
            (int)l2jpos.x,
            (int)l2jpos.y,
            (int)l2jpos.z,
            0));
    }

    void FixedUpdate()
    {
        if(DomainPacketServices.LocalPlayer == null) { 
            return; 
        }

        if(!(DomainPacketServices.LocalPlayer.Status is PlayerStatus)) {
            // Debug.LogWarning("Player status is not of type playerstatus");
            return;
        }

        PlayerStatus status = (PlayerStatus)DomainPacketServices.LocalPlayer.Status;
        PlayerStats stats = (PlayerStats)DomainPacketServices.LocalPlayer.Stats;

        if (_levelLabel != null) {
            _levelLabel.text = stats.Level.ToString();
        }

        if(_nameLabel != null) {
            _nameLabel.text = DomainPacketServices.LocalPlayer.Identity.Name;
        }

        if(_CPTextLabel != null) {
            int cpMax = PlayerResourceCaps.EffectiveMax(stats.MaxCp, status.Cp);
            int cpCur = PlayerResourceCaps.DisplayCurrent(status.Cp, cpMax);
            _CPTextLabel.text = cpCur + "/" + cpMax;
        }

        if(_HPTextLabel != null) {
            int hpMax = PlayerResourceCaps.EffectiveMax(stats.MaxHp, status.GetHp());
            int hpCur = PlayerResourceCaps.DisplayCurrent(status.GetHp(), hpMax);
            _HPTextLabel.text = hpCur + "/" + hpMax;
        }

        if(_MPTextLabel != null) {
            int mpMax = PlayerResourceCaps.EffectiveMax(stats.MaxMp, status.GetMp());
            int mpCur = PlayerResourceCaps.DisplayCurrent(status.GetMp(), mpMax);
            _MPTextLabel.text = mpCur + "/" + mpMax;
        }

        L2ResourceBarView.SetFill(
            _CPBar,
            _CPBarBG,
            PlayerResourceCaps.BarFillRatio(status.Cp, stats.MaxCp));
        L2ResourceBarView.SetFill(
            _HPBar,
            _HPBarBG,
            PlayerResourceCaps.BarFillRatio(status.GetHp(), stats.MaxHp));
        L2ResourceBarView.SetFill(
            _MPBar,
            _MPBarBG,
            PlayerResourceCaps.BarFillRatio(status.GetMp(), stats.MaxMp));

        float expRatio = LevelExpRatio(stats);
        if (_EXPTextLabel != null) {
            _EXPTextLabel.text = (expRatio * 100f).ToString("0.00") + "%";
        }
        L2ResourceBarView.SetFill(_EXPBar, _EXPBarBG, expRatio);
    }

    static float LevelExpRatio(PlayerStats stats)
    {
        int lvl = stats.Level;
        long cur = LevelServer.GetExp(lvl);
        long next = LevelServer.GetExp(lvl + 1);
        if (cur < 0 || next <= cur)
            return 0f;
        double r = (stats.Exp - cur) / (double)(next - cur);
        if (double.IsNaN(r)) return 0f;
        return Mathf.Clamp01((float)r);
    }
}
