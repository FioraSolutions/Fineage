/// <summary>
/// Character lobby / selection incoming handler access via LoginPacketServices and queue fallbacks (#142 sub-slice 51).
/// Per-packet Apply* encapsulation completed in sub-slice 75.
/// Henna apply lives on <see cref="HennaIncomingPacketServices"/> (sub-slice 99).
/// Char create ok apply lives on <see cref="CharCreateOkIncomingPacketServices"/> (sub-slice 100).
/// Char create fail apply lives on <see cref="CharCreateFailIncomingPacketServices"/> (sub-slice 101).
/// Char template apply lives on <see cref="CharTemplateIncomingPacketServices"/> (sub-slice 102).
/// Char info apply lives on <see cref="CharInfoIncomingPacketServices"/> (sub-slice 103).
/// Char selection info apply lives on <see cref="CharSelectionInfoIncomingPacketServices"/> (sub-slice 104).
/// </summary>
public static class CharacterIncomingPacketServices
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
