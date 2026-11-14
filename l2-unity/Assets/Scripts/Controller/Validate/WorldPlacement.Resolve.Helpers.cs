using UnityEngine;

public static partial class WorldPlacement
{
    public static bool TryResolveMeshY(
        Vector3 position,
        LayerMask? groundMask,
        out float y)
    {
        y = position.y;
        LayerMask mask = WalkableMask(groundMask);

        if (TryClosestHit(position, position.y + DefaultStartAbove, DefaultStartAbove + DefaultMaxDistance, mask, 0f, out y))
            return true;
        if (TryClosestHit(position, HighOriginY, HighMaxDistance, mask, 0f, out y))
            return true;
        if (TryClosestHit(position, HighOriginY, HighMaxDistance, mask, 0.4f, out y))
            return true;
        return false;
    }

    public static Vector3 SettleFeet(GameObject go, Vector3 groundPos)
    {
        if (go == null)
            return groundPos;

        SkinnedMeshRenderer[] skins = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        if (skins == null || skins.Length == 0)
            return groundPos;

        bool has = false;
        float minY = 0f;
        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i] == null)
                continue;
            float sy = skins[i].bounds.min.y;
            if (!has || sy < minY)
            {
                minY = sy;
                has = true;
            }
        }

        if (!has)
            return groundPos;

        // A staff or cape that hangs below the foot bone is not the sole. Lifting until
        // that tip touches the stone leaves the feet in the air (Gludin gatekeeper).
        float footY;
        if (TryLowestFootY(go, out footY) && minY < footY - 0.2f)
            minY = footY;

        // Positive sink: mesh feet below ground → lift. Negative: feet above ground → lower
        // (aracne / import pivots often sit slightly above the snap point).
        float sink = groundPos.y - minY;
        if (sink > 5f || sink < -2f || Mathf.Abs(sink) <= 0.001f)
            return go.transform.position;

        Vector3 p = go.transform.position;
        p.y += sink;
        ApplyPose(go, p, go.transform.rotation);
        return p;
    }

    static bool TryLowestFootY(GameObject go, out float y)
    {
        y = 0f;
        Transform[] bones = go.GetComponentsInChildren<Transform>(true);
        bool found = false;
        for (int i = 0; i < bones.Length; i++)
        {
            string n = bones[i].name;
            if (string.IsNullOrEmpty(n))
                continue;
            if (n.IndexOf("Nub", System.StringComparison.Ordinal) >= 0)
                continue;
            bool foot = n.IndexOf("Toe0", System.StringComparison.Ordinal) >= 0
                || n.IndexOf("Foot", System.StringComparison.Ordinal) >= 0;
            if (!foot)
                continue;
            float by = bones[i].position.y;
            if (!found || by < y)
            {
                y = by;
                found = true;
            }
        }

        return found;
    }
}
