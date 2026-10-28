/// <summary>
/// Char selected wire parse hook (#142 batch slice 147; login hub slice 177).
/// Splits parse out of <see cref="CharSelectedIncomingPacketServices"/> (sub-slice 75).
/// </summary>
public static class CharSelectedParsedApplyIncomingPacketServices
{
    public static GameClient Game => CharacterApplyIncomingPacketServices.Game;

    public static void OnCharSelectedParsed(CharSelectedDto packet)
    {
        Game.PlayerInfo = packet.PlayeInfo;
        Game.SetDataPreparationCompleted(false);
    }
}
