using System;

/// <summary>
/// Position validation apply hub (#142 batch slice 162; UI hub slice 167).
/// </summary>
public static class PositionApplyIncomingPacketServices
{
    public static PositionValidationController PositionValidation =>
        UiApplyIncomingPacketServices.PositionValidation;
}
