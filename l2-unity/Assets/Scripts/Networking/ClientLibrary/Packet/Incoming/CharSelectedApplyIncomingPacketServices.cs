/// <summary>
/// Char selected wire apply hook (#142 batch slice 147).
/// Splits apply out of <see cref="CharSelectedIncomingPacketServices"/> (sub-slice 75).
/// </summary>
public static class CharSelectedApplyIncomingPacketServices
{
    public static GameClient Game => LoginSessionApplyIncomingPacketServices.Game;

    public static GameManager Manager => LoginSessionApplyIncomingPacketServices.Manager;

    public static void ApplyCharSelected(CharSelectedDto packet)
    {
        Game.PlayerInfo = packet.PlayeInfo;
        Manager.OnCharacterSelect();
    }
}
