using System.Globalization;
using UnityEngine;

public static partial class L2CelestialLut
{
    static Sample Lerp(Sample a, Sample b, float u, float t, int indexA, int indexB)
    {
        return new Sample
        {
            tHours = t,
            phase = u < 0.5f ? a.phase : b.phase,
            isDay = u < 0.5f ? a.isDay : b.isDay,
            sunDirUnity = Vector3.Slerp(SafeDir(a.sunDirUnity), SafeDir(b.sunDirUnity), u),
            moonDirUnity = Vector3.Slerp(SafeDir(a.moonDirUnity), SafeDir(b.moonDirUnity), u),
            sunOffsetUnity = Vector3.Lerp(a.sunOffsetUnity, b.sunOffsetUnity, u),
            moonOffsetUnity = Vector3.Lerp(a.moonOffsetUnity, b.moonOffsetUnity, u),
            sunElevDeg = Mathf.Lerp(a.sunElevDeg, b.sunElevDeg, u),
            moonElevDeg = Mathf.Lerp(a.moonElevDeg, b.moonElevDeg, u),
            sunDrawScale = Mathf.Lerp(a.sunDrawScale, b.sunDrawScale, u),
            moonDrawScale = Mathf.Lerp(a.moonDrawScale, b.moonDrawScale, u),
            sunScale = Mathf.Lerp(a.sunScale, b.sunScale, u),
            moonScale = Mathf.Lerp(a.moonScale, b.moonScale, u),
            moonColor = Color.Lerp(a.moonColor, b.moonColor, u),
            sampleA = indexA,
            sampleB = indexB,
            lerpU = u
        };
    }

    static Vector3 DirUeToUnity(Vector3 ue)
    {
        Vector3 u = VectorUtils.ConvertToUnityUnscaled(ue);
        return u.sqrMagnitude > 1e-8f ? u.normalized : Vector3.forward;
    }

    static Vector3 SafeDir(Vector3 d)
    {
        return d.sqrMagnitude > 1e-8f ? d.normalized : Vector3.forward;
    }

    static float Parse(string s) => float.Parse(s, CultureInfo.InvariantCulture);
}
