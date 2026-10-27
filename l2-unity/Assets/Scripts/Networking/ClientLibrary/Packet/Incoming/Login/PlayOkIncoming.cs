[IncomingLoginPacket(LoginServerPacketType.PlayOk)]
public sealed class PlayOkIncoming : IncomingWirePacket<PlayOkDto>
{
    protected override void OnParsed(PlayOkDto packet)
    {
        LoginPacketServices.Game.PlayKey1 = packet.PlayOk1;
        LoginPacketServices.Game.PlayKey2 = packet.PlayOk2;
    }

    public override void Apply(PlayOkDto packet)
    {
        LobbyFlowLog.Info(
            "PlayOk Apply playKey1=" + packet.PlayOk1 + " playKey2=" + packet.PlayOk2 +
            " state=" + (LoginPacketServices.Manager != null ? LoginPacketServices.Manager.GameState.ToString() : "null"));

        LoginPacketServices.Manager.OnLoginServerPlayOk();

        if (LoginPacketServices.Manager.GameState != GameState.READY_TO_CONNECT)
        {
            LobbyFlowLog.Warn("PlayOk skip Game.Connect — GameState=" + LoginPacketServices.Manager.GameState);
            return;
        }
        if (LoginPacketServices.Manager.IsSwitchingServer)
        {
            LobbyFlowLog.Warn("PlayOk skip Game.Connect — already switching server");
            return;
        }

        LoginPacketServices.Manager.IsSwitchingServer = true;
        LoginPacketServices.LoginClient.Disconnect();
        LobbyFlowLog.Info("PlayOk → Login.Disconnect + Game.Connect");
        LoginPacketServices.Game.Connect();
    }
}
