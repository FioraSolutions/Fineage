/// <summary>
/// PlayOk wire packet handler entry (#142 sub-slice 112).
/// Splits play-ok parse/apply out of <see cref="LoginServerIncomingPacketServices"/> (sub-slice 58 encapsulation).
/// </summary>
public static class PlayOkIncomingPacketServices
{
    public static LoginClient Login => LoginPacketServices.LoginClient;

    public static GameClient Game => LoginPacketServices.Game;

    public static GameManager Manager => LoginPacketServices.Manager;

    public static void OnPlayOkParsed(PlayOkDto packet)
    {
        Game.PlayKey1 = packet.PlayOk1;
        Game.PlayKey2 = packet.PlayOk2;
    }

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
