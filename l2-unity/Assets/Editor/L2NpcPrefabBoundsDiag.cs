#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class L2NpcPrefabBoundsDiag
{
    const string OutPath = "Assets/Editor/L2NpcPrefabBoundsDiag.out.txt";

    static readonly string[] Paths =
    {
        "Assets/Resources/Data/Animations/LineageNPCs/a_teleporter_FElf_m00/a_teleporter_FElf_m00.prefab",
        "Assets/Resources/Data/Animations/LineageNPCs/a_teleporter_FHuman_m00/a_teleporter_FHuman_m00.prefab",
        "Assets/Resources/Data/Animations/LineageNPCs/a_casino_MHuman_m00/a_casino_MHuman_m00.prefab",
        "Assets/Resources/Data/Animations/LineageNPCs/a_common_people_FElf_m00/a_common_people_FElf_m00.prefab",
    };

    public static void RunCli()
    {
        Debug.Log(Run());
        EditorApplication.Exit(0);
    }

    public static string Run()
    {
        var lines = new System.Collections.Generic.List<string>();
        foreach (string path in Paths)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                lines.Add("MISSING " + path);
                continue;
            }

            var go = Object.Instantiate(prefab);
            go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            Bounds? union = null;
            foreach (var r in go.GetComponentsInChildren<Renderer>(true))
            {
                if (r == null) continue;
                Bounds b = r.bounds;
                if (union == null) union = b;
                else
                {
                    Bounds u = union.Value;
                    u.Encapsulate(b.min);
                    u.Encapsulate(b.max);
                    union = u;
                }
            }

            string fbxHint = path.Contains("FHuman")
                ? "Assets/Resources/Data/Animations/LineageNPCs/a_teleporter_FHuman_m00/Model/a_teleporter_FHuman.fbx"
                : path.Contains("common_people_FElf")
                    ? "Assets/Resources/Data/Animations/LineageNPCs/a_common_people_FElf_m00/Model/a_common_people_FElf.fbx"
                    : path.Replace("/" + Path.GetFileName(path),
                        "/Model/" + Path.GetFileNameWithoutExtension(path) + ".fbx");
            var imp = AssetImporter.GetAtPath(fbxHint) as ModelImporter;
            string gs = imp != null ? imp.globalScale.ToString("0.####") : "?";
            string line = prefab.name
                + " localScale=" + go.transform.localScale.ToString("F4")
                + " globalScale=" + gs
                + " size=" + (union.HasValue ? union.Value.size.ToString("F4") : "none");
            lines.Add(line);
            Object.DestroyImmediate(go);
        }

        string text = string.Join("\n", lines);
        File.WriteAllText(OutPath, text + "\n");
        AssetDatabase.Refresh();
        return text;
    }
}
#endif
