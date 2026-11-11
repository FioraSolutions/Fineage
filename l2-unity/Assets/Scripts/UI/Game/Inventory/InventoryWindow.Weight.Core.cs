public partial class InventoryWindow
{
    public override void ShowWindow()
    {
        base.ShowWindow();
        RefreshWeightFromPlayer();
    }

    void RefreshWeightFromPlayer()
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player?.Stats is PlayerStats stats)
        {
            UpdateStats(stats);
            return;
        }

        PlayerInfoInterlude info = ZoneCommands.Transport != null
            ? ZoneCommands.Transport.PlayerInfo
            : default;
        if (info.Stats != null)
            UpdateStats(info.Stats);
    }
}
