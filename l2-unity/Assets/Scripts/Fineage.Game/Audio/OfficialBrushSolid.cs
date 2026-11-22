using System.Collections.Generic;
using UnityEngine;

/// <summary>Point-in-solid test for official Unreal brush polys, in L2/UE units.</summary>
public static class OfficialBrushSolid
{
    public readonly struct Poly
    {
        public readonly Vector3 Normal;
        public readonly Vector3[] Vertices;

        public Poly(Vector3 normal, Vector3[] vertices)
        {
            Normal = normal;
            Vertices = vertices;
        }
    }

    public static bool Contains(Vector3 point, Vector3 min, Vector3 max, IReadOnlyList<Poly> polys)
    {
        if (polys == null || polys.Count == 0)
            return false;
        if (point.x < min.x || point.x > max.x
            || point.y < min.y || point.y > max.y
            || point.z < min.z || point.z > max.z)
            return false;

        int outward = 0;
        int inward = 0;
        Vector3 interior = (min + max) * 0.5f;
        for (int i = 0; i < polys.Count; i++)
        {
            Vector3 n = PlaneNormal(polys[i]);
            Vector3 on = polys[i].Vertices != null && polys[i].Vertices.Length > 0
                ? polys[i].Vertices[0]
                : interior;
            float side = Vector3.Dot(n, interior - on);
            if (side < 0f) outward++;
            else if (side > 0f) inward++;
        }

        bool normalsOutward = outward >= inward;
        for (int i = 0; i < polys.Count; i++)
        {
            Vector3 n = PlaneNormal(polys[i]);
            Vector3 on = polys[i].Vertices != null && polys[i].Vertices.Length > 0
                ? polys[i].Vertices[0]
                : min;
            float side = Vector3.Dot(n, point - on);
            if (normalsOutward)
            {
                if (side > 0.01f)
                    return false;
            }
            else if (side < -0.01f)
                return false;
        }
        return true;
    }

    static Vector3 PlaneNormal(Poly poly)
    {
        if (poly.Normal.sqrMagnitude > 0.0001f)
            return poly.Normal;
        if (poly.Vertices == null || poly.Vertices.Length < 3)
            return Vector3.up;
        Vector3 a = poly.Vertices[1] - poly.Vertices[0];
        Vector3 b = poly.Vertices[2] - poly.Vertices[0];
        Vector3 n = Vector3.Cross(a, b);
        return n.sqrMagnitude > 0.0001f ? n.normalized : Vector3.up;
    }
}
