/// <summary>
/// Apply facade (#142 batch slice 151). Re-homes from <see cref="PositionValidationIncomingPacketServices"/>.
/// 
/// Server position validation wire packet handler access via UiApplyIncomingPacketServices (#142 sub-slice 74).
/// </summary>
public static class PositionValidationApplyIncomingPacketServices
{
    public static void ApplyValidateLocation(ValidateLocationDto packet) =>
        UiApplyIncomingPacketServices.PositionValidation.AddValidateLocation(packet);
}
