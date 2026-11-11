using UnityEngine;

public static partial class ItemDropPose
{
    public static Vector3 TumbleWorldAxis(int dropType, Vector3 throwDir, int slot)
    {
        Vector3 flat = new Vector3(throwDir.x, 0f, throwDir.z);
        if (flat.sqrMagnitude < 0.0001f)
            flat = Vector3.forward;

        Vector3 axis = dropType == 2
            ? flat.normalized
            : Vector3.Cross(Vector3.up, flat.normalized);
        if (axis.sqrMagnitude < 1e-8f)
            axis = Vector3.right;
        axis.Normalize();
        if ((slot & 1) != 0)
            axis = -axis;
        return axis;
    }

    public static float GetTumbleDegPerSec(int slot)
    {
        // LIVE RotationRate 540016 uu/s ≈ 2966°/s. Our arc is 0.55s, so that is
        // ~4.5 flips (a smear). Two somersaults stay readable.
        float sign = (slot & 1) == 0 ? -1f : 1f;
        return sign * 720f * 1.3f * 1.3f / ItemDropPresentationIds.WeaponThrowArcSeconds;
    }
}
