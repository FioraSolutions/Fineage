/// <summary>
/// Char selected wire packet handler entry (#142 sub-slice 105).
/// Splits char-selected parse/apply out of <see cref="CharacterIncomingPacketServices"/> (sub-slice 75 encapsulation).
/// </summary>
public static class CharSelectedIncomingPacketServices
{
    public static GameClient Game => LoginPacketServices.Game;

    public static GameManager Manager => LoginPacketServices.Manager;

    public static void OnCharSelectedParsed(CharSelectedDto packet)
    {
        Game.PlayerInfo = packet.PlayeInfo;
        Game.SetDataPreparationCompleted(false);
    }

    public static void ApplyCharSelected(CharSelectedDto packet)
    {
        Game.PlayerInfo = packet.PlayeInfo;
        Manager.OnCharacterSelect();
    }
}
