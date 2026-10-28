/// <summary>
/// Server position validation apply hub (#142 batch slice 162).
/// </summary>
public static class PositionApplyIncomingPacketServices
{
    public static PositionValidationController PositionValidation =>
        IncomingPacketLocatorApplyIncomingPacketServices.PositionValidation;
}
