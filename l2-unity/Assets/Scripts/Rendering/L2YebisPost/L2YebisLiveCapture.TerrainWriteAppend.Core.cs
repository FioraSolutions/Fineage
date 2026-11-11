using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public static partial class L2YebisLiveCapture
{
    static void AppendHit(
        StringBuilder text,
        Camera cam,
        RaycastHit ground,
        Vector4 origin,
        Vector4 ambient,
        Vector4 hsv,
        Texture2D intensityTex,
        float fogEnd,
        float fogScale,
        float fogEnable)
    {
        Vector3 amb = new Vector3(ambient.x, ambient.y, ambient.z);
        Vector3 plane = new Vector3(hsv.x, hsv.y, hsv.z);
        float viewAbsZ = Mathf.Abs(cam.worldToCameraMatrix.MultiplyPoint(ground.point).z);
        float fogF = fogEnable > 0.5f ? Mathf.Clamp01((fogEnd - viewAbsZ) * fogScale) : 1f;
        text.AppendLine("hit_world " + Fmt(ground.point) + " viewAbsZ " + viewAbsZ.ToString("0.###", CultureInfo.InvariantCulture) +
            " fog_f " + fogF.ToString("0.###", CultureInfo.InvariantCulture));
        AppendPoint(text, "hit", ground.point, Vector3.zero, origin, amb, plane, intensityTex);

        MeshCollider meshCollider = ground.collider as MeshCollider;
        Mesh mesh = meshCollider != null ? meshCollider.sharedMesh : null;
        if (mesh == null || ground.triangleIndex < 0)
        {
            text.AppendLine("triangle=unavailable");
            return;
        }

        int[] tris;
        try
        {
            tris = mesh.triangles;
        }
        catch (Exception)
        {
            text.AppendLine("triangle=unreadable");
            return;
        }
        int tri = ground.triangleIndex * 3;
        if (tri + 2 >= tris.Length)
        {
            text.AppendLine("triangle=out_of_range");
            return;
        }

        Vector3[] vertices;
        Color32[] colors = null;
        try
        {
            vertices = mesh.vertices;
            if (mesh.HasVertexAttribute(VertexAttribute.Color))
            {
                colors = mesh.colors32;
            }
        }
        catch (Exception)
        {
            text.AppendLine("triangle=unreadable");
            return;
        }
        Vector3 bary = ground.barycentricCoordinate;
        Vector3 interpolated = Vector3.zero;
        for (int v = 0; v < 3; v++)
        {
            int index = tris[tri + v];
            Vector3 world = meshCollider.transform.TransformPoint(vertices[index]);
            Vector3 meshColor = Vector3.zero;
            if (colors != null && index < colors.Length)
            {
                meshColor = new Vector3(colors[index].r / 255f, colors[index].g / 255f, colors[index].b / 255f);
            }

            Vector3 color0 = TerrainShaderColor0(meshColor, amb, plane, Intensity01(world, origin, intensityTex));
            interpolated += color0 * bary[v];
            text.AppendLine(
                "v" + v + " world " + Fmt(world) +
                " mesh " + (colors != null && index < colors.Length
                    ? colors[index].r + " " + colors[index].g + " " + colors[index].b
                    : "none") +
                " shader_color0 " + Fmt(color0));
            AppendPoint(text, "v" + v, world, meshColor, origin, amb, plane, intensityTex);
        }

        text.AppendLine("pixel_color0 " + Fmt(interpolated));
    }
}
