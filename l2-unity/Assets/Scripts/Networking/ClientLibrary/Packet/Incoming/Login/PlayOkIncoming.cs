[IncomingLoginPacket(LoginServerPacketType.PlayOk)]
public sealed class PlayOkIncoming : IncomingWirePacket<PlayOkDto>
{
    protected override void OnParsed(PlayOkDto packet)
    {
        IncomingPacketServices.Game.PlayKey1 = packet.PlayOk1;
        IncomingPacketServices.Game.PlayKey2 = packet.PlayOk2;
    }

    public override void Apply(PlayOkDto packet)
    {
        LobbyFlowLog.Info(
            "PlayOk Apply playKey1=" + packet.PlayOk1 + " playKey2=" + packet.PlayOk2 +
            " state=" + (IncomingPacketServices.Manager != null ? IncomingPacketServices.Manager.GameState.ToString() : "null"));

        IncomingPacketServices.Manager.OnLoginServerPlayOk();

        if (IncomingPacketServices.Manager.GameState != GameState.READY_TO_CONNECT)
        {
            LobbyFlowLog.Warn("PlayOk skip Game.Connect — GameState=" + IncomingPacketServices.Manager.GameState);
            return;
        }
        if (IncomingPacketServices.Manager.IsSwitchingServer)
        {
            LobbyFlowLog.Warn("PlayOk skip Game.Connect — already switching server");
            return;
        }

        IncomingPacketServices.Manager.IsSwitchingServer = true;
        IncomingPacketServices.Login.Disconnect();
        LobbyFlowLog.Info("PlayOk → Login.Disconnect + Game.Connect");
        IncomingPacketServices.Game.Connect();
    }
}
