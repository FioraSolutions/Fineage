#if UNITY_EDITOR
// Measure a weapon in the hand's own frame instead of eyeballing a render.
//
// For every fist weapon and for a 1H sword (the known-good control) this prints, in hand
// space, where the mesh centroid sits, which way its longest axis points, and how far the
// mesh reaches past the knuckles. The socket frame is the hand bone: +Z along the fingers
// (wrist -> middle finger), so a weapon worn on the knuckles must have a positive forward
// reach and its blades must extend along the fingers.
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class L2FistHandFrame
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const string OutFile = "/tmp/fist-hand-frame.txt";

    struct Weapon
    {
        public string model;
        public Quaternion bindRot;
        public Vector3 bindPos;
    }

    static readonly Weapon[] Weapons =
    {
        new Weapon { model = "fist_blade_m00_wp",       bindRot = new Quaternion(-0.653281f,  0.270598f,  0.270598f, 0.653282f), bindPos = new Vector3( -3.8108f, -0.7197f, -0.0168f) },
        new Weapon { model = "the_fist_of_hero_m00_wp", bindRot = new Quaternion(-0.653281f, -0.270598f, -0.270598f, 0.653281f), bindPos = new Vector3( -1.2588f,  6.8440f,  1.4054f) },
        new Weapon { model = "great_pata_m00_wp",       bindRot = new Quaternion( 0.993572f,  0.000000f, -0.000000f, 0.113203f), bindPos = new Vector3( -0.0130f, -0.5358f,  1.9198f) },
        new Weapon { model = "bellion_cestus_m00_wp",   bindRot = new Quaternion(-0.195090f,  0.000000f,  0.000000f, 0.980785f), bindPos = new Vector3( -0.0042f, -0.3011f,  1.9745f) },
        new Weapon { model = "demon_splinter_m00_wp",   bindRot = new Quaternion(-0.107474f,  0.243822f,  0.160617f, 0.950370f), bindPos = new Vector3( 15.1705f, -1.2838f, -4.0376f) },
        new Weapon { model = "demon_fangs_m00_wp",      bindRot = new Quaternion( 0.353553f,  0.612372f,  0.353553f, 0.612372f), bindPos = new Vector3( 18.6110f, -0.0042f, -0.0052f) },
        new Weapon { model = "arthro_nail_m00_wp",      bindRot = new Quaternion( 0.500000f, -0.500000f, -0.500000f, 0.500000f), bindPos = new Vector3( -4.8344f, -4.3130f,  1.0601f) },
        new Weapon { model = "foxs_nail_m00_wp",        bindRot = new Quaternion( 0.500000f, -0.500000f, -0.500000f, 0.500000f), bindPos = new Vector3( -6.3558f,  0.6583f,  0.3984f) },
        new Weapon { model = "chakram_m00_wp",          bindRot = new Quaternion( 0.707107f,  0.707107f,  0.000000f, 0.000000f), bindPos = new Vector3( -0.0118f, -4.2048f,  0.0023f) },
        new Weapon { model = "dragon_grinder_m00_wp",   bindRot = new Quaternion( 1.000000f,  0.000000f,  0.000000f, 0.000000f), bindPos = new Vector3(  2.8105f,  0.0263f, -5.5381f) },
        // known-good control: a 1H sword the user says sits right today
        new Weapon { model = "sword_of_damascus_m00_wp", bindRot = new Quaternion( 0.707107f, -0.707107f, 0.000000f, 0.000000f), bindPos = new Vector3( -5.6050f, -0.0270f, -2.7540f) },
    };

    public static void RunCli()
    {
        var sb = new StringBuilder();

        var rigPf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "Fighter/MFighter/MFighter_anim_player.prefab");
        var rig = (GameObject)PrefabUtility.InstantiatePrefab(rigPf);
        foreach (string part in new[] { "u", "l", "g", "b" })
        {
            var pf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "Fighter/MFighter_m000_" + part + ".prefab");
            if (pf == null) continue;
            var piece = (GameObject)PrefabUtility.InstantiatePrefab(pf);
            piece.transform.SetParent(rig.transform.parent != null ? rig.transform.parent : rig.transform, false);
        }
        rig.AddComponent<SkinnedMeshSync>().SyncMesh();

        Transform wr = Find(rig, "Weapon_R_Bone");
        Transform hand = Find(rig, "Bip01_R_Hand");
        Transform finger = Find(rig, "Bip01_R_Finger1") ?? Find(rig, "Bip01_R_Finger11");

        Vector3 fwd = (finger.position - hand.position).normalized;
        sb.AppendLine("hand world pos      " + V(hand.position));
        sb.AppendLine("socket world pos    " + V(wr.position) + "   offset from hand " + V(wr.position - hand.position));
        sb.AppendLine("finger direction    " + V(fwd) + "   (hand forward)");
        sb.AppendLine("socket rotation     " + Q(wr.rotation));
        sb.AppendLine("socket fwd axis (socket.rotation * +Z) " + V(wr.rotation * Vector3.forward));
        sb.AppendLine("dot(fingerDir, socket.rotation*+Z) = " + Vector3.Dot(fwd, wr.rotation * Vector3.forward).ToString("F3"));
        sb.AppendLine();

        foreach (Weapon w in Weapons)
        {
            var pf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "LineageWeapons/" + w.model + ".prefab");
            if (pf == null) { sb.AppendLine(w.model + ": missing"); continue; }
            sb.AppendLine("=== " + w.model);

            Evaluate(sb, pf, wr, "identity", null, null);
            Evaluate(sb, pf, wr, "bind    ", w.bindRot, w.bindPos * 0.019f);
            Quaternion invRot = Quaternion.Inverse(w.bindRot);
            Evaluate(sb, pf, wr, "invBind ", invRot, -(invRot * (w.bindPos * 0.019f)));
            sb.AppendLine();
        }

        File.WriteAllText(OutFile, sb.ToString());
        Debug.Log(sb.ToString());
        EditorApplication.Exit(0);
    }

    static void Evaluate(StringBuilder sb, GameObject prefab, Transform socket, string label, Quaternion? rot, Vector3? pos)
    {
        for (int i = socket.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(socket.GetChild(i).gameObject);

        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.transform.SetParent(socket, false);
        Vector3 lossy = go.transform.lossyScale;
        go.transform.localScale = new Vector3(1f / lossy.x, 1f / lossy.y, 1f / lossy.z);
        if (rot.HasValue) go.transform.localRotation = rot.Value;
        if (pos.HasValue) go.transform.localPosition = pos.Value;

        // Gather world-space vertices of every renderer under the prefab.
        Vector3 centroid = Vector3.zero;
        int count = 0;
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        var renderers = go.GetComponentsInChildren<Renderer>(true);
        for (int r = 0; r < renderers.Length; r++)
        {
            MeshFilter mf = renderers[r].GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) continue;
            Mesh mesh = mf.sharedMesh;
            Vector3[] verts = mesh.vertices;
            Transform t = mf.transform;
            for (int i = 0; i < verts.Length; i++)
            {
                Vector3 wp = t.TransformPoint(verts[i]);
                centroid += wp; count++;
                min = Vector3.Min(min, wp);
                max = Vector3.Max(max, wp);
            }
        }
        if (count == 0) { sb.AppendLine("   " + label + ": no mesh"); return; }
        centroid /= count;

        Vector3 size = max - min;
        Vector3 centre = (min + max) * 0.5f;

        // longest axis of the mesh in world space, then expressed in socket space
        Vector3 axis = size.x >= size.y && size.x >= size.z ? Vector3.right
                     : size.z >= size.y ? Vector3.forward : Vector3.up;
        Vector3 axisWorld = axis;
        Vector3 axisLocal = socket.InverseTransformDirection(axisWorld);

        // forward reach: how far the mesh extends beyond the socket along the finger
        Transform handT = socket.parent;
        Vector3 fwdW = handT != null ? (handT.rotation * Vector3.forward) : Vector3.forward;
        Vector3 relCentroid = socket.InverseTransformPoint(centroid);
        Vector3 relCentre = socket.InverseTransformPoint(centre);

        sb.AppendLine($"   {label} verts={count} centroid(socket)={V(relCentroid)} boundsCentre(socket)={V(relCentre)}");
        sb.AppendLine($"            size(socket)={V(size)}  longestAxis(socket)={V(axisLocal)}");
    }

    static Transform Find(GameObject root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    static string V(Vector3 v) => "(" + v.x.ToString("F3") + "," + v.y.ToString("F3") + "," + v.z.ToString("F3") + ")";
    static string Q(Quaternion q) => "(" + q.x.ToString("F3") + "," + q.y.ToString("F3") + "," + q.z.ToString("F3") + "," + q.w.ToString("F3") + ")";
}
#endif
