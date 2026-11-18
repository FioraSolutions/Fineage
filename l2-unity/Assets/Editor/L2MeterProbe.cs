#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Meter pilot probe: dump every bone's world position + lossyScale and the skinned vertex
/// positions (after <see cref="SkinnedMeshRenderer.BakeMesh"/>) for an FBX under a given
/// (globalScale, useFileScale) pair. Lets a candidate import contract be compared
/// numerically against the legacy one instead of by eye.
///
/// Batch:
///   L2_METER_DUMP_DIR=Assets/L2ImportTmp L2_METER_DUMP_CONFIG=1,1 \
///   Unity -batchmode -quit -projectPath l2-unity -executeMethod L2MeterProbe.DumpCli
/// Env:
///   L2_METER_DUMP_DIR     folder to scan (default Assets/L2ImportTmp)
///   L2_METER_DUMP_CONFIG  "globalScale,useFileScale(0|1)" (default 1,1)
///   L2_METER_DUMP_OUT     json output path (default /tmp/fineage-meter-dump.json)
/// </summary>
public static class L2MeterProbe
{
    const string ResultPath = "/tmp/fineage-meter-probe.txt";
    const int VertexSamples = 32;

    [MenuItem("L2/Meter Contract/Dump FBX Bone Worlds")]
    public static void DumpMenu()
    {
        Debug.Log("[MeterProbe] wrote " + Dump());
    }

    public static void DumpCli()
    {
        try
        {
            Debug.Log("[MeterProbe] wrote " + Dump());
            EditorApplication.Exit(0);
        }
        catch (Exception error)
        {
            File.AppendAllText(ResultPath, error.ToString() + "\n");
            Debug.LogException(error);
            EditorApplication.Exit(1);
        }
    }

    static string Dump()
    {        string dir = Environment.GetEnvironmentVariable("L2_METER_DUMP_DIR") ?? "Assets/L2ImportTmp";
        string outPath = Environment.GetEnvironmentVariable("L2_METER_DUMP_OUT") ?? "/tmp/fineage-meter-dump.json";
        string[] parts = (Environment.GetEnvironmentVariable("L2_METER_DUMP_CONFIG") ?? "1,1").Split(',');
        float globalScale = float.Parse(parts[0], CultureInfo.InvariantCulture);
        bool useFileScale = parts[1].Trim() != "0";

        var sb = new StringBuilder();
        sb.Append("{\"globalScale\":").Append(N(globalScale));
        sb.Append(",\"useFileScale\":").Append(useFileScale ? "true" : "false");
        sb.Append(",\"files\":{");
        bool firstFile = true;
        foreach (string path in AssetDatabase.FindAssets("t:Model", new[] { dir })
                     .Select(AssetDatabase.GUIDToAssetPath)
                     .Where(p => p.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
                     .OrderBy(p => p, StringComparer.Ordinal))
        {
            if (!(AssetImporter.GetAtPath(path) is ModelImporter importer))
                continue;
            importer.globalScale = globalScale;
            importer.useFileScale = useFileScale;
            importer.importAnimation = false;
            importer.animationType = ModelImporterAnimationType.Generic;
            importer.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
            importer.SaveAndReimport();

            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null)
                continue;
            var instance = UnityEngine.Object.Instantiate(asset);
            try
            {
                sb.Append(firstFile ? "\n" : ",\n");
                firstFile = false;
                sb.Append('"').Append(Path.GetFileName(path)).Append("\":{");
                sb.Append("\"bones\":{");
                bool firstBone = true;
                foreach (Transform t in instance.GetComponentsInChildren<Transform>(true))
                {
                    sb.Append(firstBone ? "\n" : ",\n");
                    firstBone = false;
                    sb.Append('"').Append(t.name).Append("\":{");
                    sb.Append("\"w\":").Append(V(t.position)).Append(',');
                    sb.Append("\"s\":").Append(N(t.lossyScale.x)).Append('}');
                }
                sb.Append("},\"skins\":[");
                bool firstSkin = true;
                foreach (SkinnedMeshRenderer skin in instance.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                {
                    if (skin.sharedMesh == null)
                        continue;
                    var baked = new Mesh();
                    skin.BakeMesh(baked);
                    Vector3[] vertices = baked.vertices;
                    // BakeMesh already returns skinned vertices in the renderer's space; the
                    // renderer transform carries the unit, so TransformPoint is wrong here.
                    Vector3[] world = vertices;
                    sb.Append(firstSkin ? "\n" : ",\n");
                    firstSkin = false;
                    sb.Append('{').Append("\"node\":").Append(J(skin.name));
                    sb.Append(",\"boneCount\":").Append(skin.bones.Length);
                    sb.Append(",\"bounds\":").Append(V(skin.bounds.size)).Append(',');
                    sb.Append("\"center\":").Append(V(skin.bounds.center)).Append(',');
                    int step = Mathf.Max(1, world.Length / VertexSamples);
                    sb.Append("\"v\":[");
                    bool firstVertex = true;
                    for (int i = 0; i < world.Length; i += step)
                    {
                        sb.Append(firstVertex ? "" : ",").Append(V(world[i]));
                        firstVertex = false;
                    }
                    sb.Append(']');
                    // Deformation probe: rotate one bone 30 degrees and re-bake, so a wrong
                    // bind matrix shows up even when the rest pose matches.
                    int boneIndex = Array.FindIndex(skin.bones, b => b != null && b.name == "Bip01_R_UpperArm");
                    if (boneIndex >= 0 && boneIndex < skin.bones.Length)
                    {
                        Transform bone = skin.bones[boneIndex];
                        Quaternion was = bone.localRotation;
                        bone.localRotation = was * Quaternion.Euler(0f, 0f, 30f);
                        var posed = new Mesh();
                        skin.BakeMesh(posed);
                        Vector3[] posedWorld = posed.vertices;
                        sb.Append(",\"posed\":[");
                        firstVertex = true;
                        for (int i = 0; i < posedWorld.Length; i += step)
                        {
                            sb.Append(firstVertex ? "" : ",").Append(V(posedWorld[i]));
                            firstVertex = false;
                        }
                        sb.Append(']');
                        bone.localRotation = was;
                        UnityEngine.Object.DestroyImmediate(posed);
                    }
                    sb.Append('}');
                    UnityEngine.Object.DestroyImmediate(baked);
                }
                sb.Append("]}");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }
        sb.Append("}}\n");
        File.WriteAllText(outPath, sb.ToString());
        return outPath;
    }

    static string N(float value) => value.ToString("R", CultureInfo.InvariantCulture);

    static string V(Vector3 v) => "[" + N(v.x) + "," + N(v.y) + "," + N(v.z) + "]";

    static string J(string value) => "\"" + value + "\"";
}
#endif
