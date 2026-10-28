/// <summary>
/// PlayOk wire apply hook (#142 batch slice 146).
/// Splits apply out of <see cref="PlayOkIncomingPacketServices"/> (sub-slice 58).
/// </summary>
public static class PlayOkApplyIncomingPacketServices
{
    public static LoginClient Login => LoginSessionApplyIncomingPacketServices.Login;

    public static GameClient Game => LoginSessionApplyIncomingPacketServices.Game;

    public static GameManager Manager => LoginSessionApplyIncomingPacketServices.Manager;

    public static void ApplyPlayOk(PlayOkDto packet)
    {
        LobbyFlowLog.Info(
            "PlayOk Apply playKey1=" + packet.PlayOk1 + " playKey2=" + packet.PlayOk2 +
            " state=" + (Manager != null ? Manager.GameState.ToString() : "null"));

        Manager.OnLoginServerPlayOk();

        if (Manager.GameState != GameState.READY_TO_CONNECT)
        {
            LobbyFlowLog.Warn("PlayOk skip Game.Connect — GameState=" + Manager.GameState);
            return;
        }
        if (Manager.IsSwitchingServer)
        {
            LobbyFlowLog.Warn("PlayOk skip Game.Connect — already switching server");
            return;
        }

        Manager.IsSwitchingServer = true;
        Login.Disconnect();
        LobbyFlowLog.Info("PlayOk → Login.Disconnect + Game.Connect");
        Game.Connect();
    }
}
