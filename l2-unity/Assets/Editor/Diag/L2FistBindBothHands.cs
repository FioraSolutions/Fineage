#if UNITY_EDITOR
// Apply the original bind pose to both hands on the orc fighter and the human
// fighter, photograph from the front and the side, and also verify the A-grade
// fist still looks right with its own bind pose.
using System.IO;
using UnityEditor;
using UnityEngine;

public static class L2FistBindBothHands
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const string OutDir = "/tmp/fineage-fist-both";

    struct Bind
    {
        public string model;
        public Quaternion rot;
        public Vector3 pos;
    }

    static readonly Bind[] Binds =
    {
        new Bind { model = "demon_splinter_m00_wp", rot = new Quaternion(-0.1075f, 0.2438f, 0.1606f, 0.9504f), pos = new Vector3(15.170f, -1.284f, -4.038f) },
        new Bind { model = "fist_blade_m00_wp", rot = new Quaternion(-0.6533f, 0.2706f, 0.2706f, 0.6533f), pos = new Vector3(-3.811f, -0.720f, -0.017f) },
        new Bind { model = "great_pata_m00_wp", rot = new Quaternion(0.9936f, 0f, 0f, 0.1132f), pos = new Vector3(-0.013f, -0.536f, 1.920f) },
    };

    public static void RunCli()
    {
        Directory.CreateDirectory(OutDir);

        foreach (string race in new[] { "Orc/MOrc", "Fighter/MFighter" })
        {
            string raceName = race.Split('/')[1];
            var rigPf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + race + "/" + raceName + "_anim_player.prefab");
            if (rigPf == null) continue;
            var rig = (GameObject)PrefabUtility.InstantiatePrefab(rigPf);
            foreach (string part in new[] { "u", "l", "g", "b" })
            {
                var pf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + race + "/" + raceName + "_m000_" + part + ".prefab");
                if (pf == null) continue;
                var piece = (GameObject)PrefabUtility.InstantiatePrefab(pf);
                piece.transform.SetParent(rig.transform.parent != null ? rig.transform.parent : rig.transform, false);
            }
            rig.AddComponent<SkinnedMeshSync>().SyncMesh();

            Transform wr = Find(rig, "Weapon_R_Bone");
            Transform wl = Find(rig, "Weapon_L_Bone");
            Transform hand = Find(rig, "Bip01_R_Hand");
            Transform pelvis = Find(rig, "bip01");

            foreach (Bind bind in Binds)
            {
                var pfW = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "LineageWeapons/" + bind.model + ".prefab");
                if (pfW == null) continue;

                foreach (bool left in new[] { false, true })
                {
                    var w = (GameObject)PrefabUtility.InstantiatePrefab(pfW);
                    w.transform.SetParent(left ? wl : wr, false);
                    Vector3 lossy = w.transform.lossyScale;
                    float sx = 1f / lossy.x;
                    w.transform.localScale = new Vector3(left ? -sx : sx, 1f / lossy.y, 1f / lossy.z);
                    w.transform.localRotation = bind.rot;
                    w.transform.localPosition = bind.pos * 0.019f;
                }

                Shoot(hand.position, raceName + "_" + bind.model + "_side", new Vector3(0.30f, 0.02f, -0.42f), 30f);
                Shoot(pelvis.position, raceName + "_" + bind.model + "_body", new Vector3(0.25f, 0.05f, -0.85f), 40f);

                foreach (Transform t in new[] { wr, wl })
                {
                    for (int i = t.childCount - 1; i >= 0; i--)
                        Object.DestroyImmediate(t.GetChild(i).gameObject);
                }
            }
            Object.DestroyImmediate(rig);
        }

        EditorApplication.Exit(0);
    }

    static Transform Find(GameObject root, string name)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    static void Shoot(Vector3 focus, string label, Vector3 offset, float fov)
    {
        var camGo = new GameObject("cam");
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.12f, 0.12f, 0.14f, 1f);
        cam.fieldOfView = fov;
        cam.nearClipPlane = 0.005f;
        cam.farClipPlane = 50f;
        cam.transform.position = focus + offset;
        cam.transform.LookAt(focus);
        var lightGo = new GameObject("light");
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.5f;
        lightGo.transform.rotation = Quaternion.Euler(25f, 150f, 0f);
        const int size = 560;
        var rt = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;
        var tex = new Texture2D(size, size, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        File.WriteAllBytes(Path.Combine(OutDir, label + ".png"), tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        rt.Release();
        Object.DestroyImmediate(rt);
        Object.DestroyImmediate(camGo);
        Object.DestroyImmediate(lightGo);
    }
}
#endif