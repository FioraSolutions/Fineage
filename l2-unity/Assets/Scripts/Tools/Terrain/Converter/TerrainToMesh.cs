#if (UNITY_EDITOR) 
using System.Collections.Generic;
using UnityEngine;

public partial class TerrainToMesh : MonoBehaviour
{
    public Terrain terrain;
    public GameObject dest;
    public int meshSubdivisions = 255;

    // Start is called before the first frame update



    /*private void SetTriangles(Mesh mesh, int subdivisions) {
        int[] triangles = new int[subdivisions * subdivisions * 6];
        int vert = 0;
        int tris = 0;
        for(int z = 0; z < subdivisions; z++) {
            for(int x = 0; x < subdivisions; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + subdivisions + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + subdivisions + 1;
                triangles[tris + 5] = vert + subdivisions + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }
        mesh.triangles = triangles;
    }*/




}
#endif