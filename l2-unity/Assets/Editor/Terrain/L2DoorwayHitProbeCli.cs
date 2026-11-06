#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Batch probe what seals Elven Village doorways (mesh/mat/brush).
/// Unity -batchmode -nographics -quit -projectPath l2-unity -executeMethod L2DoorwayHitProbeCli.RunCli
/// </summary>
public static class L2DoorwayHitProbeCli
{
    public static void RunCli()
    {
        bool ok = Probe("21_19");
        EditorApplication.Exit(ok ? 0 : 1);
    }

    [MenuItem("Shnok/[Debug] Probe Elven doorway hits (21_19)")]
    public static void Menu() { Probe("21_19"); }

    static bool Probe(string tile)
    {
        string scenePath = "Assets/Resources/Scenes/" + tile + ".unity";
        if (!File.Exists(scenePath))
        {
            Debug.LogError("[DoorwayProbe] missing " + scenePath);
            return false;
        }

        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        Physics.SyncTransforms();

        var sb = new StringBuilder();

        // Prefab Unity positions for O_E_GDS_Ent (already ConvertPosToUnity)
        Vector3 gdsE = new Vector3(982.0953f, -50.628574f, 876.64764f);
        Vector3 gdsW = new Vector3(982.0953f, -50.628574f, 851.3905f);
        Vector3 gdIn = new Vector3(996.8381f, -48.799885f, 864.1907f);

        sb.AppendLine("[DoorwayProbe] === GDS / GD mesh inventory ===");
        bool entCollider = false;
        bool mainCollider = false;
        foreach (var r in Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (r == null) continue;
            string path = PathOf(r.transform);
            if (path.IndexOf("GDS_Ent", System.StringComparison.OrdinalIgnoreCase) < 0
                && path.IndexOf("GDS_Main", System.StringComparison.OrdinalIgnoreCase) < 0
                && path.IndexOf("O_E_GD_IN", System.StringComparison.OrdinalIgnoreCase) < 0
                && path.IndexOf("O_E_GD.", System.StringComparison.OrdinalIgnoreCase) < 0
                && !r.gameObject.name.EndsWith("O_E_GD"))
                continue;

            var mf = r.GetComponent<MeshFilter>();
            Mesh mesh = mf != null ? mf.sharedMesh : null;
            bool hasCol = r.GetComponent<Collider>() != null;
            if (path.IndexOf("GDS_Ent", System.StringComparison.OrdinalIgnoreCase) >= 0)
                entCollider |= hasCol;
            if (path.IndexOf("GDS_Main", System.StringComparison.OrdinalIgnoreCase) >= 0)
                mainCollider |= hasCol;
            sb.AppendLine(
                "REN " + r.gameObject.name
                + " path=" + path
                + " center=" + r.bounds.center
                + " size=" + r.bounds.size
                + " subMeshes=" + (mesh != null ? mesh.subMeshCount : -1)
                + " verts=" + (mesh != null ? mesh.vertexCount : -1)
                + " layer=" + LayerMask.LayerToName(r.gameObject.layer)
                + " collider=" + hasCol);
            var mats = r.sharedMaterials;
            if (mats != null)
            {
                for (int i = 0; i < mats.Length; i++)
                {
                    var m = mats[i];
                    if (m == null)
                    {
                        sb.AppendLine("  mat[" + i + "]=NULL");
                        continue;
                    }
                    Texture baseMap = m.HasProperty("_BaseMap") ? m.GetTexture("_BaseMap") : null;
                    if (baseMap == null && m.HasProperty("_MainTex"))
                        baseMap = m.GetTexture("_MainTex");
                    Color c = m.HasProperty("_BaseColor") ? m.GetColor("_BaseColor")
                        : (m.HasProperty("_Color") ? m.GetColor("_Color") : Color.white);
                    sb.AppendLine(
                        "  mat[" + i + "]=" + m.name
                        + " shader=" + (m.shader != null ? m.shader.name : "?")
                        + " tex=" + (baseMap != null ? baseMap.name : "NULL")
                        + " color=" + c
                        + " queue=" + m.renderQueue
                        + " alphaClip=" + (m.HasProperty("_AlphaClip") ? m.GetFloat("_AlphaClip") : -1));
                }
            }
        }

        sb.AppendLine("[DoorwayProbe] === rays into entrances ===");
        // Stand south of facade (smaller Z) and fire +Z into building; also try ±X.
        // Eye height ~1.5 above ground (y ~ -50.6 + 1.5)
        float eyeY = -49.1f;
        AppendRay(sb, "GDS_E_+Z", new Vector3(gdsE.x, eyeY, gdsE.z - 8f), Vector3.forward, 20f);
        AppendRay(sb, "GDS_E_-Z", new Vector3(gdsE.x, eyeY, gdsE.z + 8f), Vector3.back, 20f);
        AppendRay(sb, "GDS_E_+X", new Vector3(gdsE.x - 8f, eyeY, gdsE.z), Vector3.right, 20f);
        AppendRay(sb, "GDS_E_-X", new Vector3(gdsE.x + 8f, eyeY, gdsE.z), Vector3.left, 20f);
        AppendRay(sb, "GDS_W_+Z", new Vector3(gdsW.x, eyeY, gdsW.z - 8f), Vector3.forward, 20f);
        AppendRay(sb, "GD_IN_+Z", new Vector3(gdIn.x, eyeY, gdIn.z - 8f), Vector3.forward, 20f);

        // Sweep a vertical fan at GDS_E doorway center
        sb.AppendLine("[DoorwayProbe] === vertical sweep GDS_E ===");
        for (float dy = 0f; dy <= 6f; dy += 0.5f)
        {
            Vector3 o = new Vector3(gdsE.x, gdsE.y + dy, gdsE.z - 6f);
            AppendRay(sb, "sweep_y" + dy.ToString("F1"), o, Vector3.forward, 14f);
        }

        // Standing-height capsule ≈ CharacterController on terrace Y~-48
        sb.AppendLine("[DoorwayProbe] === standing capsule / floor gate ===");
        Vector3 foot = new Vector3(982.10f, -48.0f, 870.65f);
        Vector3 p0 = foot + Vector3.up * 0.35f;
        Vector3 p1 = foot + Vector3.up * 1.55f;
        float radius = 0.28f;
        bool prev = Physics.queriesHitBackfaces;
        Physics.queriesHitBackfaces = true;
        bool blocked = Physics.CapsuleCast(
            p0, p1, radius, Vector3.forward, out RaycastHit capHit, 14f, ~0, QueryTriggerInteraction.Ignore);
        Physics.queriesHitBackfaces = prev;
        sb.AppendLine(
            "CAPSULE_INTO_GUILD blocked=" + blocked
            + (blocked
                ? (" col=" + capHit.collider.name + " d=" + capHit.distance.ToString("F2") + " pt=" + capHit.point)
                : " (clear)"));

        Vector3[] floorSamples =
        {
            new Vector3(982.10f, -45.0f, 870.0f), // terrace south of entrance
            new Vector3(982.10f, -45.0f, 874.0f), // doorway threshold (may be punched)
            new Vector3(982.10f, -45.0f, 878.5f), // inside Main east
            new Vector3(975.00f, -45.0f, 864.0f), // courtyard
            new Vector3(982.10f, -45.0f, 854.0f), // west approach
        };
        bool terraceOk = false;
        bool prevFloor = Physics.queriesHitBackfaces;
        Physics.queriesHitBackfaces = true;
        foreach (var sample in floorSamples)
        {
            RaycastHit[] downs = Physics.RaycastAll(sample, Vector3.down, 20f, ~0, QueryTriggerInteraction.Ignore);
            System.Array.Sort(downs, (a, b) => a.distance.CompareTo(b.distance));
            if (downs.Length == 0)
            {
                sb.AppendLine("FLOOR_DOWN @" + sample + " hit=False");
                continue;
            }
            for (int i = 0; i < Mathf.Min(downs.Length, 3); i++)
            {
                var fl = downs[i];
                sb.AppendLine(
                    "FLOOR_DOWN @" + sample
                    + " #" + i
                    + " col=" + fl.collider.name
                    + " y=" + fl.point.y.ToString("F2")
                    + " path=" + PathOf(fl.collider.transform));
                if (fl.point.y > -52f && fl.point.y < -46f
                    && (fl.collider.name.IndexOf("GDS_Main", System.StringComparison.OrdinalIgnoreCase) >= 0
                        || PathOf(fl.collider.transform).IndexOf("GDS_Main", System.StringComparison.OrdinalIgnoreCase) >= 0))
                    terraceOk = true;
            }
        }
        Physics.queriesHitBackfaces = prevFloor;
        sb.AppendLine("GATE EntCollider=" + entCollider + " MainCollider=" + mainCollider
            + " capsuleClear=" + (!blocked)
            + " terraceFloorY~-48=" + terraceOk);

        Debug.Log(sb.ToString());
        File.WriteAllText("/tmp/fineage-doorway-probe.txt", sb.ToString());
        File.WriteAllText(
            "/Users/nikitarakitin/Projects/Fineage/.work/doorway_reset_verify.done",
            sb.ToString());
        return true;
    }

    static void AppendRay(StringBuilder sb, string label, Vector3 origin, Vector3 dir, float dist)
    {
        bool prev = Physics.queriesHitBackfaces;
        Physics.queriesHitBackfaces = true;
        RaycastHit[] hits;
        try
        {
            hits = Physics.RaycastAll(origin, dir.normalized, dist, ~0, QueryTriggerInteraction.Ignore);
        }
        finally
        {
            Physics.queriesHitBackfaces = prev;
        }

        sb.AppendLine("RAY " + label + " from=" + origin + " dir=" + dir.normalized);
        if (hits == null || hits.Length == 0)
        {
            sb.AppendLine("  (no hits)");
            return;
        }
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        int n = Mathf.Min(hits.Length, 6);
        for (int i = 0; i < n; i++)
        {
            var h = hits[i];
            var c = h.collider;
            Renderer r = null;
            if (c != null)
            {
                r = c.GetComponent<Renderer>();
                if (r == null) r = c.GetComponentInParent<Renderer>();
            }
            string mats = "-";
            if (r != null && r.sharedMaterials != null)
            {
                var parts = new List<string>();
                foreach (var m in r.sharedMaterials)
                    parts.Add(m != null ? m.name : "null");
                mats = string.Join(",", parts);
            }
            sb.AppendLine(
                "  #" + i
                + " d=" + h.distance.ToString("F2")
                + " layer=" + (c != null ? LayerMask.LayerToName(c.gameObject.layer) : "?")
                + " col=" + (c != null ? c.name : "?")
                + " path=" + (c != null ? PathOf(c.transform) : "?")
                + " mats=" + mats
                + " pt=" + h.point);
        }
    }

    static string PathOf(Transform t)
    {
        string p = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            p = t.name + "/" + p;
        }
        return p;
    }
}
#endif
