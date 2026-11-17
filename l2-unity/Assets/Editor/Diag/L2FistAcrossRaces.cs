#if UNITY_EDITOR
// Mount the same fist weapon on several races and photograph the right fist from the
// same relative angle. If every race holds it the same wrong way, the weapon prefab or
// its mount convention is at fault; if only the orc is wrong, it is the orc socket.
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class L2FistAcrossRaces
{
    const string Anim = "Assets/Resources/Data/Animations/";
    const string OutDir = "/tmp/fineage-fist-races";

    static readonly (string family, string race, string bodyDir)[] Races =
    {
        ("Fighter", "MFighter", "Fighter"),
        ("Elf", "MElf", "Elf"),
        ("Dwarf", "MDwarf", "Dwarf"),
        ("Orc", "MOrc", "Orc"),
        ("Orc", "FOrc", "Orc"),
    };

    public static void RunCli()
    {
        Directory.CreateDirectory(OutDir);
        var sb = new StringBuilder();

        foreach (var (family, race, bodyDir) in Races)
        {
            string rigPath = Anim + family + "/" + race + "/" + race + "_anim_player.prefab";
            var rigPf = AssetDatabase.LoadAssetAtPath<GameObject>(rigPath);
            if (rigPf == null) { sb.AppendLine(race + ": no rig"); continue; }

            var actor = new GameObject("actor_" + race);
            var rig = (GameObject)PrefabUtility.InstantiatePrefab(rigPf);
            rig.transform.SetParent(actor.transform, false);

            foreach (string part in new[] { "u", "l", "g", "b" })
            {
                var pf = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + bodyDir + "/" + race + "_m000_" + part + ".prefab");
                if (pf == null) continue;
                var piece = (GameObject)PrefabUtility.InstantiatePrefab(pf);
                piece.transform.SetParent(actor.transform, false);
            }

            var sync = rig.AddComponent<SkinnedMeshSync>();
            sync.SyncMesh();

            Transform wr = Find(rig, "Weapon_R_Bone");
            Transform hand = Find(rig, "Bip01_R_Hand");
            if (wr == null || hand == null) { sb.AppendLine(race + ": bones missing"); Object.DestroyImmediate(actor); continue; }

            var wp = AssetDatabase.LoadAssetAtPath<GameObject>(Anim + "LineageWeapons/fist_blade_m00_wp.prefab");
            var weapon = (GameObject)PrefabUtility.InstantiatePrefab(wp);
            weapon.transform.SetParent(wr, false);
            Vector3 lossy = weapon.transform.lossyScale;
            weapon.transform.localScale = new Vector3(1f / lossy.x, 1f / lossy.y, 1f / lossy.z);

            // measure: hand direction (wrist -> middle finger) and claw direction
            Transform finger = Find(rig, "Bip01_R_Finger1") ?? Find(rig, "Bip01_R_Finger11");
            Vector3 handDir = finger != null ? (finger.position - hand.position).normalized : Vector3.zero;
            Vector3 socketToWeapon = (weapon.transform.position - hand.position);
            sb.AppendLine($"{race}: handWorld={V(hand.position)} socketWorld={V(wr.position)} socketOffset={V(wr.position - hand.position)}");
            sb.AppendLine($"   hand->finger dir={V(handDir)}  weapon localY world={V(weapon.transform.up)}  dot={Vector3.Dot(handDir, weapon.transform.up):F3}");
            sb.AppendLine($"   socket localPos={V(wr.localPosition)}  hand localPos={V(hand.localPosition)}");

            // camera framed on the hand from outside the body
            Vector3 focus = hand.position;
            Vector3 camOffset = new Vector3(0.35f, 0.05f, -0.45f);
            Shoot(focus, race + "_hand", camOffset, 30f);
            Object.DestroyImmediate(actor);
        }

        File.WriteAllText(Path.Combine(OutDir, "report.txt"), sb.ToString());
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

        const int size = 640;
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