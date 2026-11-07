using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>Play Mode diagnostic: what colliders are under / overlapping the player (stuck-in-pit / pop-up reports).</summary>
public static class FineageColliderProbe
{
    [MenuItem("Fineage/Debug/Probe colliders at player %#k")]
    static void Probe()
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null) { Debug.LogWarning("[ColliderProbe] no Player"); return; }
        Vector3 p = player.transform.position;
        var sb = new System.Text.StringBuilder("[ColliderProbe] player " + p.ToString("F2") + "\n-- ray down from +60:\n");
        foreach (var h in Physics.RaycastAll(p + Vector3.up * 60f, Vector3.down, 120f, ~0, QueryTriggerInteraction.Collide).OrderBy(h => h.distance))
            sb.AppendLine(string.Format("  y={0:F2} {1} [{2}] layer={3} trigger={4} normal={5}", h.point.y, Path(h.collider.transform),
                h.collider.GetType().Name, LayerMask.LayerToName(h.collider.gameObject.layer), h.collider.isTrigger, h.normal.ToString("F2")));
        var cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            Vector3 c = player.transform.TransformPoint(cc.center);
            float half = Mathf.Max(0, cc.height * 0.5f - cc.radius);
            sb.AppendLine("-- overlapping CC (r=" + cc.radius + " h=" + cc.height + " step=" + cc.stepOffset + " skin=" + cc.skinWidth + " grounded=" + cc.isGrounded + "):");
            foreach (var col in Physics.OverlapCapsule(c + Vector3.up * half, c - Vector3.up * half, cc.radius + 0.05f, ~0, QueryTriggerInteraction.Collide))
                if (col != cc) sb.AppendLine("  " + Path(col.transform) + " [" + col.GetType().Name + "] layer=" + LayerMask.LayerToName(col.gameObject.layer));
        }
        Debug.Log(sb.ToString());
    }

    static string Path(Transform t)
    {
        string s = t.name;
        for (int i = 0; i < 3 && t.parent != null; i++) { t = t.parent; s = t.name + "/" + s; }
        return s;
    }
}
