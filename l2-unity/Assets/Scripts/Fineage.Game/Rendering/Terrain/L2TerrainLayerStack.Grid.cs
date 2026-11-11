using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public sealed partial class L2TerrainLayerStack
{
    void EnsureNativeVertexGrid()
    {
        if (_filter == null || _filter.sharedMesh == null)
            return;
        Mesh src = _filter.sharedMesh;
        const int oldSide = 255;
        const int newSide = 256;
        if (src.vertexCount != oldSide * oldSide)
            return;

        Vector3[] oldPos = src.vertices;
        Vector3[] oldNrm = src.normals;
        Vector2[] oldUv = src.uv;
        int[] oldTris = src.triangles;
        bool hasNrm = oldNrm != null && oldNrm.Length == oldPos.Length;
        bool hasUv = oldUv != null && oldUv.Length == oldPos.Length;
        const float step = 1f / 254f;
        var cell = new int[newSide * newSide];
        for (int i = 0; i < cell.Length; i++)
            cell[i] = -1;
        for (int i = 0; i < oldPos.Length; i++)
        {
            int gx = Mathf.Clamp(Mathf.RoundToInt(oldPos[i].x / step), 0, oldSide - 1);
            int gz = Mathf.Clamp(Mathf.RoundToInt(oldPos[i].z / step), 0, oldSide - 1);
            cell[gz * newSide + gx] = i;
        }

        var pos = new List<Vector3>(oldPos);
        var nrm = new List<Vector3>(hasNrm ? oldNrm : new Vector3[oldPos.Length]);
        var uv = new List<Vector2>(hasUv ? oldUv : new Vector2[oldPos.Length]);
        if (!hasNrm)
        {
            for (int i = 0; i < nrm.Count; i++)
                nrm[i] = Vector3.up;
        }

        for (int z = 0; z < newSide; z++)
        {
            for (int x = 0; x < newSide; x++)
            {
                int slot = z * newSide + x;
                if (cell[slot] >= 0 || (x < oldSide && z < oldSide))
                    continue;
                int x0 = Mathf.Min(x, oldSide - 1);
                int z0 = Mathf.Min(z, oldSide - 1);
                int x1 = Mathf.Max(0, x0 - 1);
                int z1 = Mathf.Max(0, z0 - 1);
                int ia = cell[z0 * newSide + x0];
                int ib = cell[z0 * newSide + x1];
                int ic = cell[z1 * newSide + x0];
                if (ia < 0)
                    continue;
                Vector3 p = oldPos[ia];
                if (x == oldSide && ib >= 0)
                    p += oldPos[ia] - oldPos[ib];
                if (z == oldSide && ic >= 0)
                    p += oldPos[ia] - oldPos[ic];
                if (x == oldSide && z == oldSide && ib >= 0 && ic >= 0)
                    p = oldPos[ia] + (oldPos[ia] - oldPos[ib]) + (oldPos[ia] - oldPos[ic]);
                cell[slot] = pos.Count;
                pos.Add(p);
                nrm.Add(Vector3.up);
                uv.Add(new Vector2(x / 254f, z / 254f));
            }
        }

        var tris = new List<int>(oldTris);
        for (int z = 0; z < oldSide; z++)
        {
            for (int x = 0; x < oldSide; x++)
            {
                if (x < oldSide - 1 && z < oldSide - 1)
                    continue;
                int a = cell[z * newSide + x];
                int b = cell[z * newSide + (x + 1)];
                int c = cell[(z + 1) * newSide + x];
                int d = cell[(z + 1) * newSide + (x + 1)];
                if (a < 0 || b < 0 || c < 0 || d < 0)
                    continue;
                tris.Add(a);
                tris.Add(c);
                tris.Add(b);
                tris.Add(b);
                tris.Add(c);
                tris.Add(d);
            }
        }

        var mesh = new Mesh { name = src.name + "_256" };
        mesh.indexFormat = IndexFormat.UInt32;
        mesh.SetVertices(pos);
        mesh.SetNormals(nrm);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        _filter.mesh = mesh;
    }
}
