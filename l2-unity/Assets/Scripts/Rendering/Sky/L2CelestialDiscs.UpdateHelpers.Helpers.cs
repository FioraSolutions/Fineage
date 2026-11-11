using System;
using Object = UnityEngine.Object;

public partial class L2CelestialDiscs
{
    float ResolveSkyDistance(Camera cam)
    {
        float far = cam != null ? cam.farClipPlane : 500f;
        // Keep the quad inside the far plane; corners of a large billboard sit farther than the center.
        float autoFar = Mathf.Clamp(far * 0.88f, 80f, 4000f);
        if (_forceDistanceMeters > 80f)
        {
            return Mathf.Min(_forceDistanceMeters, far * 0.95f);
        }

        return autoFar;
    }

    static string FormatClock(float hours)
    {
        float t = hours % 24f;
        if (t < 0f)
        {
            t += 24f;
        }

        int sec = Mathf.Clamp((int)(t * 3600f), 0, 86399);
        return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}", sec / 3600, (sec % 3600) / 60, sec % 60);
    }
}
