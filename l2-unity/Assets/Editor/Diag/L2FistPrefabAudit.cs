#if UNITY_EDITOR
// Compare fist weapon prefabs: how many meshes do they carry, where are those meshes
// relative to the prefab pivot, and what happens when the same prefab is mounted on
// both hands. S-grade fists ship a left and a right mesh inside one prefab.
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class L2FistPrefabAudit
{
    const string Anim = "Assets/Resources/Data/Animations/LineageWeapons/";
    const string OutFile = "/tmp/fist-prefab-audit.txt";

    static readonly string[] Models =
    {
        "fist_blade_m00_wp",
        "demon_splinter_m00_wp",
        "great_pata_m00_wp",
        "demon_fangs_m00_wp",
        "the_fist_of_hero_m00_wp",
    };

    public static void RunCli()
    {
        var sb = new StringBuilder();
        foreach (string name in Models)
        {
            var pf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + name + ".prefab");
            if (pf == null) { sb.AppendLine(name + ": MISSING"); continue; }
            var go = (GameObject)PrefabUtility.InstantiatePrefab(pf);
            try
            {
                sb.AppendLine("=== " + name);
                int renderers = 0;
                foreach (var r in go.GetComponentsInChildren<Renderer>(true))
                {
                    Mesh mesh = null;
                    if (r is MeshRenderer)
                    {
                        var mf = r.GetComponent<MeshFilter>();
                        mesh = mf != null ? mf.sharedMesh : null;
                    }
                    else if (r is SkinnedMeshRenderer smr) mesh = smr.sharedMesh;
                    if (mesh == null) continue;
                    renderers++;
                    Bounds b = mesh.bounds;
                    sb.AppendLine($"   renderer {r.name} enabled={r.enabled} active={r.gameObject.activeSelf}");
                    sb.AppendLine($"      localPos={V(r.transform.localPosition)} worldPos={V(r.transform.position)}");
                    sb.AppendLine($"      mesh bounds centre={V(b.center)} size={V(b.size)} min={V(b.min)} max={V(b.max)}");
                    sb.AppendLine($"      world centre={V(r.transform.TransformPoint(b.center))}");
                }
                sb.AppendLine($"   total mesh renderers: {renderers}");
            }
            finally { Object.DestroyImmediate(go); }
        }

        // Now show what the equip code produces for the S-grade model: two full copies.
        var rigPf = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Resources/Data/Animations/Fighter/MFighter/MFighter_anim_player.prefab");
        if (rigPf != null)
        {
            var rig = (GameObject)PrefabUtility.InstantiatePrefab(rigPf);
            try
            {
                var wr = Find(rig, "Weapon_R_Bone");
                var wl = Find(rig, "Weapon_L_Bone");
                var pf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "demon_splinter_m00_wp.prefab");
                if (wr != null && wl != null && pf != null)
                {
                    var right = (GameObject)PrefabUtility.InstantiatePrefab(pf);
                    right.transform.SetParent(wr, false);
                    var left = (GameObject)PrefabUtility.InstantiatePrefab(pf);
                    left.transform.SetParent(wl, false);
                    left.transform.localScale = new Vector3(-1f, 1f, 1f);

                    int rightCount = right.GetComponentsInChildren<Renderer>(true).Length;
                    int leftCount = left.GetComponentsInChildren<Renderer>(true).Length;
                    sb.AppendLine($"=== mounted on MFighter: right hand renders {rightCount} meshes, left hand renders {leftCount} meshes");
                    sb.AppendLine("   (a single-mesh fist should render exactly 1 per hand)");
                }
            }
            finally { Object.DestroyImmediate(rig); }
        }

        File.WriteAllText(OutFile, sb.ToString());
        Debug.Log(sb.ToString());
        EditorApplication.Exit(0);
    }

    static Transform Find(GameObject root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    static string V(Vector3 v) => "(" + v.x.ToString("F4") + "," + v.y.ToString("F4") + "," + v.z.ToString("F4") + ")";
}
#endif