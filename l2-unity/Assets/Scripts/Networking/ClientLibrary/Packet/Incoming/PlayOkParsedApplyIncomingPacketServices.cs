/// <summary>
/// PlayOk wire parse hook (#142 batch slice 146).
/// Splits parse out of <see cref="PlayOkIncomingPacketServices"/> (sub-slice 58).
/// </summary>
public static class PlayOkParsedApplyIncomingPacketServices
{
    public static GameClient Game => LoginPacketServices.Game;

    public static void OnPlayOkParsed(PlayOkDto packet)
    {
        Game.PlayKey1 = packet.PlayOk1;
        Game.PlayKey2 = packet.PlayOk2;
    }
}
