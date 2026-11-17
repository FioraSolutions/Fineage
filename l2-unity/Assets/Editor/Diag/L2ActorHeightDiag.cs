#if UNITY_EDITOR
// World-space height of every NPC/monster prefab, to find the ones that still stand
// at room scale after the meter migration.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class L2ActorHeightDiag
{
    const string OutPath = "/tmp/fineage-actor-heights.txt";

    public static void RunCli()
    {
        var lines = new List<string>();
        string[] roots =
        {
            "Assets/Resources/Data/Animations/LineageNPCs",
            "Assets/Resources/Data/Animations/LineageNPCs4",
            "Assets/Resources/Data/Animations/LineageMonsters",
        };

        foreach (string root in roots)
        {
            if (!Directory.Exists(root))
                continue;
            foreach (string path in Directory.GetFiles(root, "*.prefab", SearchOption.AllDirectories))
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                    continue;

                // Only the root actor prefab: its name matches the folder.
                if (Path.GetFileNameWithoutExtension(path) != Path.GetFileName(Path.GetDirectoryName(path)))
                    continue;

                var go = Object.Instantiate(prefab);
                try
                {
                    Bounds? union = null;
                    foreach (Renderer r in go.GetComponentsInChildren<Renderer>(true))
                    {
                        if (r == null || !r.enabled)
                            continue;
                        Bounds b = r.bounds;
                        if (union == null)
                            union = b;
                        else
                        {
                            Bounds u = union.Value;
                            u.Encapsulate(b.min);
                            u.Encapsulate(b.max);
                            union = u;
                        }
                    }

                    if (union == null)
                        continue;

                    lines.Add($"{union.Value.size.y:F2}\t{union.Value.min.y:F2}\t{go.transform.localScale.x:F2}\t{prefab.name}");
                }
                finally
                {
                    Object.DestroyImmediate(go);
                }
            }
        }

        lines.Sort();
        File.WriteAllText(OutPath, string.Join("\n", lines) + "\n");
        Debug.Log("[L2ActorHeightDiag] wrote " + lines.Count + " rows to " + OutPath);
        EditorApplication.Exit(0);
    }
}
#endif
