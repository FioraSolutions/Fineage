/// <summary>
/// Position validation apply hub (#142 batch slice 162; position orchestration slice 196).
/// </summary>
public static class PositionApplyIncomingPacketServices
{
    public static PositionValidationController PositionValidation =>
        LocatorApplyIncomingPacketServices.PositionValidation;
}
