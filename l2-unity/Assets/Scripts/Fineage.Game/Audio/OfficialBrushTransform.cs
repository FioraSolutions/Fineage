using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unreal brush local-to-world transform used by official volume dumps (#1728).
/// Matches tools/interlude-extract/write_official_music_volumes.py: world =
/// location + RotateUe(scale * drawScale3D * (local - prepivot)).
/// Rotation is a UE rotator in 65536-unit circle, applied roll (X), pitch (Y), yaw (Z).
/// Coordinates stay in L2/UE units.
/// </summary>
public static class OfficialBrushTransform
{
    const float TwoPiOver65536 = (2f * Mathf.PI) / 65536f;

    public static Vector3 ToWorld(
        Vector3 local,
        Vector3 location,
        Vector3 prepivot,
        Vector3 scale,
        Vector3 drawScale3d,
        Vector3 rotationPitchYawRoll)
    {
        Vector3 v = local - prepivot;
        v = new Vector3(
            v.x * scale.x * drawScale3d.x,
            v.y * scale.y * drawScale3d.y,
            v.z * scale.z * drawScale3d.z);
        v = RotateUe(v, rotationPitchYawRoll);
        return location + v;
    }

    public static Vector3 RotateUe(Vector3 v, Vector3 pitchYawRoll)
    {
        float pitch = pitchYawRoll.x * TwoPiOver65536;
        float yaw = pitchYawRoll.y * TwoPiOver65536;
        float roll = pitchYawRoll.z * TwoPiOver65536;
        if (pitch == 0f && yaw == 0f && roll == 0f)
            return v;

        float x = v.x, y = v.y, z = v.z;
        if (roll != 0f)
        {
            float c = Mathf.Cos(roll), s = Mathf.Sin(roll);
            float ny = y * c - z * s;
            float nz = y * s + z * c;
            y = ny;
            z = nz;
        }
        if (pitch != 0f)
        {
            float c = Mathf.Cos(pitch), s = Mathf.Sin(pitch);
            float nx = x * c + z * s;
            float nz = -x * s + z * c;
            x = nx;
            z = nz;
        }
        if (yaw != 0f)
        {
            float c = Mathf.Cos(yaw), s = Mathf.Sin(yaw);
            float nx = x * c - y * s;
            float ny = x * s + y * c;
            x = nx;
            y = ny;
        }
        return new Vector3(x, y, z);
    }

    public static Vector3 RotateNormalUe(Vector3 normal, Vector3 scale, Vector3 drawScale3d, Vector3 rotationPitchYawRoll)
    {
        Vector3 n = new Vector3(
            normal.x * scale.x * drawScale3d.x,
            normal.y * scale.y * drawScale3d.y,
            normal.z * scale.z * drawScale3d.z);
        n = RotateUe(n, rotationPitchYawRoll);
        return n.sqrMagnitude > 0.0001f ? n.normalized : Vector3.forward;
    }

    /// <summary>UE Z is up. Faces whose world normal points mostly +Z are water-surface candidates.</summary>
    public static bool IsTopFace(Vector3 worldNormal, float threshold = 0.5f)
    {
        return worldNormal.z >= threshold;
    }

    /// <summary>
    /// Append a fan-triangulated polygon in UE units. Returns false if the poly has fewer than 3 vertices.
    /// </summary>
    public static bool AppendPolygon(IReadOnlyList<Vector3> worldVerts, List<Vector3> verts, List<int> tris)
    {
        if (worldVerts == null || worldVerts.Count < 3)
            return false;
        int i0 = verts.Count;
        for (int i = 0; i < worldVerts.Count; i++)
            verts.Add(worldVerts[i]);
        for (int i = 1; i < worldVerts.Count - 1; i++)
        {
            tris.Add(i0);
            tris.Add(i0 + i);
            tris.Add(i0 + i + 1);
        }
        return true;
    }
}
