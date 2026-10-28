/// <summary>
/// Char selected wire apply hook (#142 batch slice 147; locator queue slice 255).
/// Splits apply out of <see cref="CharSelectedIncomingPacketServices"/> (sub-slice 75).
/// </summary>
public static class CharSelectedApplyIncomingPacketServices
{
    public static GameClient Game => LocatorApplyIncomingPacketServices.Game;

    public static GameManager Manager => LocatorApplyIncomingPacketServices.Manager;

    public static void ApplyCharSelected(CharSelectedDto packet)
    {
        Game.PlayerInfo = packet.PlayeInfo;
        Manager.OnCharacterSelect();
    }
}
