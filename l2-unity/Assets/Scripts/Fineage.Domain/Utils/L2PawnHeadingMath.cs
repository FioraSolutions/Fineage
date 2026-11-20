using System;

/// <summary>
/// Engine-free Interlude pawn heading math (APawn::physicsRotation / fixedTurn).
/// RotationRate.Yaw uses 65536 heading units per full circle; live dump 0xFDE8.
/// </summary>
public static class L2PawnHeadingMath
{
    public const int FullCircle = 65536;
    public const int RotationRateYaw = 0xFDE8;

    public static float DegreesPerSecond(int headingUnitsPerSecond)
    {
        return headingUnitsPerSecond * (360f / FullCircle);
    }

    public static float DeltaAngleDegrees(float currentYaw, float desiredYaw)
    {
        float delta = (desiredYaw - currentYaw) % 360f;
        if (delta > 180f)
            delta -= 360f;
        if (delta < -180f)
            delta += 360f;
        return delta;
    }

    // APawn::fixedTurn: step toward desired yaw by at most maxStepDegrees.
    public static float StepDegrees(float currentYaw, float desiredYaw, float maxStepDegrees)
    {
        float delta = DeltaAngleDegrees(currentYaw, desiredYaw);
        float abs = Math.Abs(delta);
        if (abs <= maxStepDegrees || maxStepDegrees < 0f)
            return desiredYaw;
        return currentYaw + Math.Sign(delta) * maxStepDegrees;
    }
}