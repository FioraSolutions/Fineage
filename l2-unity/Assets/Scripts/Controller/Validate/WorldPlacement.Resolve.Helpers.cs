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

        float sink = groundPos.y - minY;
        if (sink <= 0.001f || sink > 5f)
            return go.transform.position;

        Vector3 p = go.transform.position;
        p.y += sink;
        ApplyPose(go, p, go.transform.rotation);
        return p;
    }
}
