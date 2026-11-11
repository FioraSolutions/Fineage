#if (UNITY_EDITOR) 
using System.Collections.Generic;
using UnityEngine;

public partial class DecoToMesh
{
    private static float GetGroundHeight(Vector3 pos) {
        RaycastHit hit;
        if (Physics.Raycast(pos + Vector3.up * 200.0f, Vector3.down, out hit, 200f, 1 << 9)) {
            return hit.point.y;
        }

        return -1;
    }
}
#endif
