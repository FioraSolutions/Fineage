/// <summary>
/// Apply facade (#142 batch slice 151; locator queue slice 259). Re-homes from <see cref="PositionValidationIncomingPacketServices"/>.
///
/// Server position validation wire packet handler access via LocatorApply (#142 sub-slice 74; UI hub slice 182).
/// </summary>
public static class PositionValidationApplyIncomingPacketServices
{
    public static PositionValidationController PositionValidation =>
        LocatorApplyIncomingPacketServices.PositionValidation;

    public static void ApplyValidateLocation(ValidateLocationDto packet) =>
        PositionValidation.AddValidateLocation(packet);
}
