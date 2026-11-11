using UnityEngine;

public static partial class L2FxRibbonGetPoint
{
    public static bool TryGetMeshBladeEnds(Transform searchRoot, out Vector3 tipWorld, out Vector3 hiltWorld)
    {
        tipWorld = default;
        hiltWorld = default;
        if (searchRoot == null)
        {
            return false;
        }

        Transform meshXf = null;
        Bounds localBounds = default;
        bool found = false;

        MeshFilter mf = searchRoot.GetComponentInChildren<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            meshXf = mf.transform;
            localBounds = mf.sharedMesh.bounds;
            found = true;
        }
        else
        {
            SkinnedMeshRenderer smr = searchRoot.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null && smr.sharedMesh != null)
            {
                meshXf = smr.transform;
                localBounds = smr.sharedMesh.bounds;
                found = true;
            }
        }

        if (!found || meshXf == null)
        {
            return false;
        }

        Vector3 size = localBounds.size;
        Vector3 minLocal;
        Vector3 maxLocal;
        if (size.x >= size.y && size.x >= size.z)
        {
            minLocal = new Vector3(localBounds.min.x, localBounds.center.y, localBounds.center.z);
            maxLocal = new Vector3(localBounds.max.x, localBounds.center.y, localBounds.center.z);
        }
        else if (size.z >= size.x && size.z >= size.y)
        {
            minLocal = new Vector3(localBounds.center.x, localBounds.center.y, localBounds.min.z);
            maxLocal = new Vector3(localBounds.center.x, localBounds.center.y, localBounds.max.z);
        }
        else
        {
            minLocal = new Vector3(localBounds.center.x, localBounds.min.y, localBounds.center.z);
            maxLocal = new Vector3(localBounds.center.x, localBounds.max.y, localBounds.center.z);
        }

        // Convention: max = tip, min = hilt (matches SwordSetup max→tip naming intent).
        tipWorld = meshXf.TransformPoint(maxLocal);
        hiltWorld = meshXf.TransformPoint(minLocal);
        return (tipWorld - hiltWorld).sqrMagnitude > 1e-10f;
    }

    public static Edges StabilizeEdgePolarity(in Edges current, in Edges reference)
    {
        Vector3 curAxis = current.A2 - current.A3;
        Vector3 refAxis = reference.A2 - reference.A3;
        if (curAxis.sqrMagnitude < 1e-12f || refAxis.sqrMagnitude < 1e-12f)
        {
            return current;
        }

        if (Vector3.Dot(curAxis, refAxis) >= 0f)
        {
            return current;
        }

        Edges swapped;
        swapped.A2 = current.A3;
        swapped.A3 = current.A2;
        swapped.A4 = current.A4;
        return swapped;
    }

    public static Edges StabilizeEdgeEndsByProximity(in Edges current, in Edges reference)
    {
        float keep =
            Vector3.Distance(current.A2, reference.A2) +
            Vector3.Distance(current.A3, reference.A3);
        float swap =
            Vector3.Distance(current.A2, reference.A3) +
            Vector3.Distance(current.A3, reference.A2);

        if (swap + 1e-5f < keep)
        {
            Edges swapped;
            swapped.A2 = current.A3;
            swapped.A3 = current.A2;
            swapped.A4 = current.A4;
            return swapped;
        }

        return current;
    }
}
