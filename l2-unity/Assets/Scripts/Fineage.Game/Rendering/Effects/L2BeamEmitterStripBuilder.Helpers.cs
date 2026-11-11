using System.Collections.Generic;
using UnityEngine;

public partial class L2BeamEmitterStripBuilder : MonoBehaviour
{
    public static int SegmentCount(int highFrequencyPoints)
    {
        int points = Mathf.Clamp(highFrequencyPoints, 2, MaxSegments + 1);
        return points - 1;
    }

    public static void AssignStripToFilters(Component host, Mesh mesh)
    {
        if (host == null || mesh == null)
        {
            return;
        }

        MeshFilter[] filters = host.GetComponentsInChildren<MeshFilter>(true);
        for (int i = 0; i < filters.Length; i++)
        {
            if (filters[i] != null)
            {
                filters[i].sharedMesh = mesh;
            }
        }
    }
}
