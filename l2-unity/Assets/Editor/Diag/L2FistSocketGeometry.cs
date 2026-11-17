#if UNITY_EDITOR
// Where does a fist weapon sit relative to the hand it is supposed to grip?
// Reports the socket bone and the actual hand bone in one frame, the weapon mesh
// bounds in socket-local space, and how far the weapon centre is from the palm.
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class L2FistSocketGeometry
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const string OutFile = "/tmp/fist-geometry.txt";

    public static void RunCli()
    {
        var sb = new StringBuilder();

        // 1) weapon prefab local mesh bounds and pivot
        var wp = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "LineageWeapons/fist_blade_m00_wp.prefab");
        if (wp != null)
        {
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(wp);
            try
            {
                sb.AppendLine("=== fist_blade_m00_wp prefab (local space, pivot = origin) ===");
                foreach (var t in inst.GetComponentsInChildren<Transform>(true))
                {
                    sb.AppendLine($"  node {t.name}  pos={V(t.localPosition)} rot={Q(t.localRotation)} scale={V(t.localScale)}");
                    var mf = t.GetComponent<MeshFilter>();
                    if (mf != null && mf.sharedMesh != null)
                    {
                        Bounds b = mf.sharedMesh.bounds;
                        sb.AppendLine($"     mesh bounds center={V(b.center)} size={V(b.size)} min={V(b.min)} max={V(b.max)}");
                    }
                    var smr = t.GetComponent<SkinnedMeshRenderer>();
                    if (smr != null && smr.sharedMesh != null)
                    {
                        Bounds b = smr.sharedMesh.bounds;
                        sb.AppendLine($"     skinned mesh bounds center={V(b.center)} size={V(b.size)}");
                        sb.AppendLine($"     bones={smr.bones.Length} rootBone={(smr.rootBone != null ? smr.rootBone.name : "-")}");
                    }
                }
            }
            finally { Object.DestroyImmediate(inst); }
        }

        // 2) compare the hand bone and the weapon socket for several races
        string[] races = { "Fighter/MFighter", "Elf/MElf", "Orc/MOrc", "Orc/FOrc", "Dwarf/MDwarf" };
        foreach (string rr in races)
        {
            string[] p = rr.Split('/');
            string family = p[0], race = p[1];
            string bodyPath = Anim + family + "/" + race + "_m000_u.prefab";
            var body = AssetDatabase.LoadAssetAtPath<GameObject>(bodyPath);
            if (body == null) { sb.AppendLine(race + ": body missing"); continue; }

            var go = (GameObject)PrefabUtility.InstantiatePrefab(body);
            try
            {
                sb.AppendLine($"=== {race} body bind ===");
                Transform wr = Find(go, "Weapon_R_Bone");
                Transform hand = Find(go, "Bip01_R_Hand");
                if (wr == null || hand == null) { sb.AppendLine("   bones missing"); continue; }

                sb.AppendLine($"   hand  pos={V(hand.position)}  lossy={V(hand.lossyScale)}");
                sb.AppendLine($"   WR    pos={V(wr.position)}");
                Vector3 local = hand.InverseTransformPoint(wr.position);
                sb.AppendLine($"   socket in hand-local space: {V(local)}  (dist={local.magnitude:F4} m)");
                sb.AppendLine($"   socket localPos under parent={V(wr.localPosition)}");
                sb.AppendLine($"   socket world rot={Q(wr.rotation)}");
                sb.AppendLine($"   hand   world rot={Q(hand.rotation)}");

                // the actual hand mesh vertices that carry the palm
                var smr = go.GetComponentInChildren<SkinnedMeshRenderer>();
                if (smr != null && smr.sharedMesh != null)
                {
                    Mesh mesh = smr.sharedMesh;
                    Transform[] bones = smr.bones;
                    var weights = mesh.boneWeights;
                    int handIdx = -1, socketIdx = -1;
                    for (int i = 0; i < bones.Length; i++)
                    {
                        if (bones[i] == hand) handIdx = i;
                        if (bones[i] == wr) socketIdx = i;
                    }
                    sb.AppendLine($"   mesh: {weights.Length} weights; handIdx={handIdx} socketIdx={socketIdx}");
                    if (socketIdx >= 0)
                    {
                        float wsum = 0f; int cnt = 0;
                        Bounds bound = new Bounds();
                        bool first = true;
                        Vector3[] verts = mesh.vertices;
                        for (int i = 0; i < weights.Length; i++)
                        {
                            var w = weights[i];
                            float ws = (w.boneIndex0 == socketIdx ? w.weight0 : 0f)
                                     + (w.boneIndex1 == socketIdx ? w.weight1 : 0f)
                                     + (w.boneIndex2 == socketIdx ? w.weight2 : 0f)
                                     + (w.boneIndex3 == socketIdx ? w.weight3 : 0f);
                            if (ws <= 0.001f) continue;
                            cnt++; wsum += ws;
                            if (first) { bound = new Bounds(verts[i], Vector3.zero); first = false; }
                            else bound.Encapsulate(verts[i]);
                        }
                        sb.AppendLine($"   verts weighted to Weapon_R_Bone: {cnt} (wsum={wsum:F1}) bounds={V(bound.center)} size={V(bound.size)}");
                    }
                }
            }
            finally { Object.DestroyImmediate(go); }
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
    static string Q(Quaternion q) => "(" + q.x.ToString("F3") + "," + q.y.ToString("F3") + "," + q.z.ToString("F3") + "," + q.w.ToString("F3") + ")";
}
#endif