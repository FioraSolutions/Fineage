#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Scene diagnostic for sealed Elven Village doorways: ray from Scene camera
/// (or Main Camera) and log which layer/object blocks the passage.
/// </summary>
public static class L2DoorwayRaycastDiag
{
    [MenuItem("Shnok/[Debug] Raycast doorway blocker (Scene view)")]
    public static void Menu()
    {
        Camera cam = SceneView.lastActiveSceneView != null
            ? SceneView.lastActiveSceneView.camera
            : Camera.main;
        if (cam == null)
        {
            Debug.LogError("[DoorwayDiag] no Scene/Main camera");
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        bool prev = Physics.queriesHitBackfaces;
        Physics.queriesHitBackfaces = true;
        RaycastHit[] hits;
        try
        {
            hits = Physics.RaycastAll(ray, 80f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        }
        finally
        {
            Physics.queriesHitBackfaces = prev;
        }

        if (hits == null || hits.Length == 0)
        {
            Debug.Log("[DoorwayDiag] no hits along Scene camera forward (80u)");
            return;
        }

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("[DoorwayDiag] hits from " + cam.name + " forward:");
        int n = Mathf.Min(hits.Length, 12);
        for (int i = 0; i < n; i++)
        {
            RaycastHit h = hits[i];
            Collider c = h.collider;
            string layer = c != null ? LayerMask.LayerToName(c.gameObject.layer) : "?";
            string path = c != null ? ObjectPath(c.transform) : "?";
            sb.AppendLine(
                "  #" + i + " dist=" + h.distance.ToString("F1")
                + " layer=" + layer
                + " name=" + (c != null ? c.name : "?")
                + " path=" + path
                + " point=" + h.point);
        }
        Debug.Log(sb.ToString());
    }

    static string ObjectPath(Transform t)
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
