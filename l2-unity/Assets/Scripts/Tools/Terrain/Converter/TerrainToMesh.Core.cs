#if (UNITY_EDITOR) 
using System.Collections.Generic;
using UnityEngine;

public partial class TerrainToMesh : MonoBehaviour
{
    void Start() {
        // BuildMeshBase();
        // ConvertTerrain();
        // AssetDatabase.ExportPackage("Assets/Data/Maps/17_25/TerrainData", "TerrainPackage.unitypackage");
    }

    public void BuildMeshBase() {
        Mesh planeMesh = new Mesh();
        SetVertices(planeMesh, meshSubdivisions);
        SetTriangles(planeMesh, meshSubdivisions);
        SetUVs(planeMesh, meshSubdivisions);

        var meshFilter = dest.AddComponent<MeshFilter>();
        meshFilter.mesh = planeMesh;
        var meshRenderer = dest.AddComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        meshRenderer.material = material;

        Vector3 meshScale = new Vector3(terrain.terrainData.size.x, 1, terrain.terrainData.size.z);
        dest.transform.localScale = meshScale;
        dest.transform.position = terrain.transform.position;
    }

    private void SetVertices(Mesh mesh, int subdivisions) {
        Vector3[] vertices = new Vector3[(subdivisions + 1) * (subdivisions + 1)];
        float step = 1f / subdivisions;
        for(int i = 0, z = 0; z <= subdivisions; z++) {
            for(int x = 0; x <= subdivisions; x++) {
                vertices[i] = new Vector3(x * step, 0, z * step);
                i++;
            }
        }
        mesh.vertices = vertices;
    }
}
#endif