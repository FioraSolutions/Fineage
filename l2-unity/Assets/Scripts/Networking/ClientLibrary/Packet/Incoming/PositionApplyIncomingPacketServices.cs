/// <summary>
/// Position validation apply hub (#142 batch slice 162; locator slice 170).
/// </summary>
public static class PositionApplyIncomingPacketServices
{
    public static PositionValidationController PositionValidation =>
        LocatorApplyIncomingPacketServices.PositionValidation;
}
