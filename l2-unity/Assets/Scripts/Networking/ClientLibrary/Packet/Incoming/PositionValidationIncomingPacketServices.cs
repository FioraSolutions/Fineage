/// <summary>
/// Server position validation wire packet handler access via UiIncomingPacketServices (#142 sub-slice 74).
/// </summary>
public static class PositionValidationIncomingPacketServices
{
    public static void ApplyValidateLocation(ValidateLocationDto packet) =>
        UiIncomingPacketServices.PositionValidation.AddValidateLocation(packet);
}
