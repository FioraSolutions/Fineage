#if (UNITY_EDITOR) 
using System.Collections.Generic;
using UnityEngine;

public partial class TerrainToMesh : MonoBehaviour
{
    private void SetTriangles(Mesh mesh, int subdivisions) {
        var triangles = new List<int>(subdivisions * subdivisions * 6);
        int vert = 0;
        for(int z = 0; z < subdivisions; z++) {
            for(int x = 0; x < subdivisions; x++) {
                if (!applyQuadVisibilityHoles || MeshQuadVisible(x, z, subdivisions)) {
                    triangles.Add(vert + 0);
                    triangles.Add(vert + subdivisions + 1);
                    triangles.Add(vert + 1);
                    triangles.Add(vert + 1);
                    triangles.Add(vert + subdivisions + 1);
                    triangles.Add(vert + subdivisions + 2);
                }
                vert++;
            }
            vert++; // Move to the next row
        }
        mesh.triangles = triangles.ToArray();
    }

    /// <summary>
    /// Official mask is 256×256. When the mesh is coarser, hide a mesh quad if any official cell it covers is hidden.
    /// </summary>
    private bool MeshQuadVisible(int x, int z, int subdivisions) {
        int[] words = quadVisibilityBitmap;
        if (words == null) {
            return true;
        }

        int n = L2TerrainInfoParser.QuadVisibilitySize;
        if (subdivisions == n) {
            return L2TerrainInfoParser.IsUnityQuadVisible(words, x, z);
        }

        int x0 = x * n / subdivisions;
        int x1 = (x + 1) * n / subdivisions;
        int z0 = z * n / subdivisions;
        int z1 = (z + 1) * n / subdivisions;
        for (int zz = z0; zz < z1; zz++) {
            for (int xx = x0; xx < x1; xx++) {
                if (!L2TerrainInfoParser.IsUnityQuadVisible(words, xx, zz)) {
                    return false;
                }
            }
        }

        return true;
    }

    private void SetUVs(Mesh mesh, int subdivisions) {
        Vector2[] uvs = new Vector2[mesh.vertices.Length];
        for(int i = 0, z = 0; z <= subdivisions; z++) {
            for(int x = 0; x <= subdivisions; x++) {
                uvs[i] = new Vector2((float)x / subdivisions, (float)z / subdivisions);
                i++;
            }
        }
        mesh.uv = uvs;
    }

    public void ConvertTerrain(bool optimize) {
        Vector3 terrainSize = terrain.terrainData.size;
        if (!(terrainSize.x > 0f && terrainSize.y > 0f && terrainSize.z > 0f)) {
            throw new System.InvalidOperationException(
                "[Interlude terrain] terrain '" + terrain.name + "' has size " + terrainSize +
                "; SampleHeight would return NaN. Check GeneratedSectorCounter in its TerrainInfo0.txt.");
        }

        // get the bounds of the terrain
        var bounds = terrain.terrainData.bounds;

        var mf = dest.GetComponent<MeshFilter>();
        var mesh = mf.mesh;

        List<Vector3> newVerts = new List<Vector3>();
        foreach(var vert in mesh.vertices) {
            var wPos = dest.transform.localToWorldMatrix * vert;
            var newVert = vert;
            newVert.y = terrain.SampleHeight(wPos);
            if (float.IsNaN(newVert.y) || float.IsInfinity(newVert.y)) {
                throw new System.InvalidOperationException(
                    "[Interlude terrain] terrain '" + terrain.name + "' sampled height " + newVert.y + " at " + wPos + ".");
            }
            newVerts.Add(newVert);
        }
        mesh.SetVertices(newVerts.ToArray());
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        if(optimize) {
            mesh.Optimize();
        }
    }
}
#endif