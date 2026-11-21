using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects and repairs LineageWeapons meshes that were exported as a single-sided sheet
/// (cardboard) instead of a closed shell. Rebaked weapons carry paired opposite-facing
/// triangles; legacy Blender 3.6 exports do not.
/// </summary>
public static class WeaponMeshTopology
{
    /// <summary>
    /// Fraction of triangle normals that must have an opposing partner in the same mesh.
    /// Measured: rebaked fist/axe meshes &gt;= ~0.89; cardboard fists &lt; ~0.25.
    /// Unity re-import (weld/optimize) can shave ~1–2% off the FBX-on-disk ratio.
    /// </summary>
    public const float MinimumOppositeNormalPairRatio = 0.84f;

    /// <summary>Shell offset as a fraction of the mesh's largest axis extent.</summary>
    public const float SolidifyExtentFactor = 0.035f;

    public const float MinimumShellMeters = 0.0015f;

    public struct Report
    {
        public string AssetName;
        public int VertexCount;
        public int TriangleCount;
        public Vector3 BoundsSize;
        public float ThicknessRatio;
        public float OppositeNormalPairRatio;
        public int ZeroLengthNormals;
        public int FlippedNormals;
        public bool IsIntentionallyFlat;
        public bool IsDegenerate;

        public string FailureReason =>
            IsDegenerate
                ? "oppRatio=" + OppositeNormalPairRatio.ToString("F3")
                  + " thickRatio=" + ThicknessRatio.ToString("F3")
                  + " verts=" + VertexCount
                  + " tris=" + TriangleCount
                : string.Empty;
    }

    public static bool IsIntentionallyFlatAsset(string meshName)
    {
        if (string.IsNullOrEmpty(meshName))
            return false;
        return meshName.EndsWith("_et", StringComparison.Ordinal)
            || meshName.EndsWith("_sh", StringComparison.Ordinal)
            || meshName.Contains("fishing_rod", StringComparison.Ordinal);
    }

    public static Report Analyze(Mesh mesh, string assetName = null)
    {
        var report = new Report { AssetName = assetName ?? (mesh != null ? mesh.name : null) };
        if (mesh == null)
        {
            report.IsDegenerate = true;
            return report;
        }

        report.IsIntentionallyFlat = IsIntentionallyFlatAsset(assetName ?? mesh.name);
        report.VertexCount = mesh.vertexCount;
        report.TriangleCount = mesh.triangles.Length / 3;
        report.BoundsSize = mesh.bounds.size;
        float longest = Mathf.Max(report.BoundsSize.x, Mathf.Max(report.BoundsSize.y, report.BoundsSize.z));
        float shortest = Mathf.Min(report.BoundsSize.x, Mathf.Min(report.BoundsSize.y, report.BoundsSize.z));
        report.ThicknessRatio = longest > 1e-6f ? shortest / longest : 0f;

        mesh.GetNormals(reportNormalsScratch);
        for (int i = 0; i < reportNormalsScratch.Count; i++)
        {
            if (reportNormalsScratch[i].sqrMagnitude < 1e-10f)
                report.ZeroLengthNormals++;
        }

        report.OppositeNormalPairRatio = ComputeOppositeNormalPairRatio(mesh);
        report.FlippedNormals = CountInconsistentWinding(mesh);
        report.IsDegenerate = !report.IsIntentionallyFlat
            && report.OppositeNormalPairRatio < MinimumOppositeNormalPairRatio;
        return report;
    }

    /// <summary>
    /// Builds a thickened closed shell (front + offset back + reversed winding).
    /// Returns null when the mesh already satisfies <see cref="MinimumOppositeNormalPairRatio"/>.
    /// </summary>
    public static Mesh BuildSolidifiedMesh(Mesh source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        Report before = Analyze(source, source.name);
        if (!before.IsDegenerate)
            return null;

        Vector3[] vertices = source.vertices;
        int[] triangles = source.triangles;
        if (vertices == null || vertices.Length == 0 || triangles == null || triangles.Length < 3)
            throw new InvalidOperationException(
                "Weapon mesh '" + source.name + "' has no geometry; re-export from LineageWeapons.ukx.");

        Vector3[] avgNormals = ComputeAverageNormals(vertices, triangles);
        float longest = Mathf.Max(source.bounds.size.x, Mathf.Max(source.bounds.size.y, source.bounds.size.z));
        float shell = Mathf.Max(longest * SolidifyExtentFactor, MinimumShellMeters);

        int baseCount = vertices.Length;
        var newVertices = new Vector3[baseCount * 2];
        Array.Copy(vertices, newVertices, baseCount);
        for (int i = 0; i < baseCount; i++)
            newVertices[i + baseCount] = vertices[i] + avgNormals[i] * shell;

        int triCount = triangles.Length / 3;
        var newTris = new int[triCount * 3 * 2];
        for (int t = 0; t < triCount; t++)
        {
            int o = t * 3;
            int a = triangles[o];
            int b = triangles[o + 1];
            int c = triangles[o + 2];
            int front = t * 6;
            newTris[front] = a;
            newTris[front + 1] = b;
            newTris[front + 2] = c;
            int back = front + 3;
            newTris[back] = a + baseCount;
            newTris[back + 1] = c + baseCount;
            newTris[back + 2] = b + baseCount;
        }

        var mesh = new Mesh { name = source.name + "_solidified" };
        mesh.vertices = newVertices;
        mesh.triangles = newTris;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        Report after = Analyze(mesh, source.name);
        if (after.IsDegenerate)
        {
            throw new InvalidOperationException(
                "Solidify did not repair weapon mesh '" + source.name + "': " + after.FailureReason);
        }

        return mesh;
    }

    public static float ComputeOppositeNormalPairRatio(Mesh mesh)
    {
        if (mesh == null)
            return 0f;
        int[] tris = mesh.triangles;
        Vector3[] verts = mesh.vertices;
        int triCount = tris.Length / 3;
        if (triCount == 0)
            return 0f;

        triNormalsScratch.Clear();
        for (int t = 0; t < triCount; t++)
        {
            int o = t * 3;
            Vector3 n = ComputeTriangleNormal(verts, tris[o], tris[o + 1], tris[o + 2]);
            if (n.sqrMagnitude > 1e-12f)
                triNormalsScratch.Add(n.normalized);
        }

        int unmatched = 0;
        var used = new bool[triNormalsScratch.Count];
        for (int i = 0; i < triNormalsScratch.Count; i++)
        {
            if (used[i])
                continue;
            bool found = false;
            Vector3 n = triNormalsScratch[i];
            for (int j = i + 1; j < triNormalsScratch.Count; j++)
            {
                if (used[j])
                    continue;
                if (Vector3.Dot(n, triNormalsScratch[j]) < -0.85f)
                {
                    used[i] = true;
                    used[j] = true;
                    found = true;
                    break;
                }
            }

            if (!found)
                unmatched++;
        }

        return triNormalsScratch.Count > 0
            ? 1f - (float)unmatched / triNormalsScratch.Count
            : 0f;
    }

    static int CountInconsistentWinding(Mesh mesh)
    {
        mesh.GetNormals(reportNormalsScratch);
        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;
        int bad = 0;
        for (int t = 0; t < tris.Length / 3; t++)
        {
            int o = t * 3;
            Vector3 face = ComputeTriangleNormal(verts, tris[o], tris[o + 1], tris[o + 2]);
            if (face.sqrMagnitude < 1e-12f)
                continue;
            Vector3 vertexNormal = reportNormalsScratch[tris[o]]
                + reportNormalsScratch[tris[o + 1]]
                + reportNormalsScratch[tris[o + 2]];
            if (vertexNormal.sqrMagnitude < 1e-12f)
                continue;
            if (Vector3.Dot(face.normalized, vertexNormal.normalized) < -0.2f)
                bad++;
        }

        return bad;
    }

    static Vector3[] ComputeAverageNormals(Vector3[] vertices, int[] triangles)
    {
        var sums = new Vector3[vertices.Length];
        var counts = new int[vertices.Length];
        for (int t = 0; t < triangles.Length / 3; t++)
        {
            int o = t * 3;
            int a = triangles[o];
            int b = triangles[o + 1];
            int c = triangles[o + 2];
            Vector3 n = ComputeTriangleNormal(vertices, a, b, c);
            if (n.sqrMagnitude < 1e-12f)
                continue;
            n.Normalize();
            sums[a] += n;
            sums[b] += n;
            sums[c] += n;
            counts[a]++;
            counts[b]++;
            counts[c]++;
        }

        var avg = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            if (counts[i] == 0)
                avg[i] = Vector3.forward;
            else
            {
                avg[i] = sums[i] / counts[i];
                if (avg[i].sqrMagnitude < 1e-12f)
                    avg[i] = Vector3.forward;
                else
                    avg[i].Normalize();
            }
        }

        return avg;
    }

    static Vector3 ComputeTriangleNormal(Vector3[] verts, int a, int b, int c)
    {
        Vector3 ab = verts[b] - verts[a];
        Vector3 ac = verts[c] - verts[a];
        return Vector3.Cross(ab, ac);
    }

    static readonly List<Vector3> reportNormalsScratch = new List<Vector3>(256);
    static readonly List<Vector3> triNormalsScratch = new List<Vector3>(512);
}