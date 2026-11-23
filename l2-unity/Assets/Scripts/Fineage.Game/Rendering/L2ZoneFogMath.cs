using UnityEngine;

/// <summary>
/// Distance-fog amount and the eye-depth reconstruction Hidden/L2/ZoneFog uses.
/// The shader constants are meters: UU * <see cref="VectorUtils.L2UuToUnity"/>.
/// </summary>
public static class L2ZoneFogMath
{
    /// <summary>
    /// Fog amount in [0,1]. 0 keeps the pixel, 1 is DistanceFogColor.
    /// eye, start, and end are Unreal units. Both sides are scaled by the same
    /// UU-to-meters factor the world and <see cref="L2ZoneRenderTable.DistanceFogGlobals"/> use.
    /// </summary>
    public static float FogAmountUu(float eyeUu, float startUu, float endUu, float uuToUnity)
    {
        return FogAmountMeters(eyeUu * uuToUnity, startUu * uuToUnity, endUu * uuToUnity);
    }

    /// <summary>
    /// Same amount the shader writes: saturate((eyeMeters - startMeters) * scale)
    /// with scale = 1 / (endMeters - startMeters).
    /// </summary>
    public static float FogAmountMeters(float eyeMeters, float startMeters, float endMeters)
    {
        float range = endMeters - startMeters;
        if (range <= 1e-6f)
            return 0f;
        float t = (eyeMeters - startMeters) / range;
        if (float.IsNaN(t) || t <= 0f)
            return 0f;
        if (t >= 1f)
            return 1f;
        return t;
    }

    /// <summary>
    /// Unity _ZBufferParams. Reversed-Z is Metal, D3D11, and modern GL:
    /// raw 1 is the near plane, raw 0 is the far plane.
    /// </summary>
    public static Vector4 ZBufferParams(float near, float far, bool reversedZ)
    {
        float invFar = 1f / far;
        if (reversedZ)
        {
            float x = far / near - 1f;
            const float y = 1f;
            return new Vector4(x, y, x * invFar, y * invFar);
        }

        float xn = 1f - far / near;
        float yn = far / near;
        return new Vector4(xn, yn, xn * invFar, yn * invFar);
    }

    /// <summary>LinearEyeDepth(raw, _ZBufferParams). A zero param vector is non-finite.</summary>
    public static float LinearEyeDepth(float raw, Vector4 zBufferParams)
    {
        return 1f / (zBufferParams.z * raw + zBufferParams.w);
    }

    /// <summary>Cleared depth. Reversed-Z platforms store the far plane as 0.</summary>
    public static bool IsFarPlaneRaw(float raw, bool reversedZ)
    {
        return reversedZ ? raw <= 1e-6f : raw >= 1f - 1e-6f;
    }

    /// <summary>
    /// A sample the fog pass may lerp. The far clear and a non-finite reconstruction
    /// (missing depth, or _ZBufferParams left at 0) are not world pixels.
    /// </summary>
    public static bool EyeDepthUsable(float eyeMeters, float farClip, bool farPlaneRaw)
    {
        if (farPlaneRaw)
            return false;
        if (float.IsNaN(eyeMeters) || float.IsInfinity(eyeMeters))
            return false;
        return eyeMeters > 0f && eyeMeters < farClip * 0.999f;
    }
}
