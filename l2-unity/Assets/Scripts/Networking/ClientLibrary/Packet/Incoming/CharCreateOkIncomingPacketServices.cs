/// <summary>
/// Character create ok wire packet handler entry (#142 sub-slice 100).
/// Splits unwired char-create-ok apply out of <see cref="CharacterIncomingPacketServices"/> until lobby wiring lands.
/// </summary>
public static class CharCreateOkIncomingPacketServices
{
    public static void ApplyCharCreateOk(CharCreateOkDto packet)
    {
    }
}
