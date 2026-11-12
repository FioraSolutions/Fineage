#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

public partial class L2BrushBuilder
{
    static GameObject createProbuilderMesh(string csgOper, PolyData polyData, int index) {

        Material material = GetMaterialForTexture(polyData.texture);

        Vector3 adjustedNormal = VectorUtils.ConvertToUnityUnscaled(polyData.normal);
        Vector3 adjustedU = VectorUtils.ConvertToUnityUnscaled(polyData.textureU);
        Vector3 adjustedV = VectorUtils.ConvertToUnityUnscaled(polyData.textureV);

        Quaternion rt = Quaternion.FromToRotation(Vector3.forward, adjustedNormal);
        Vector3 rotatedU = rt * adjustedU;
        Vector3 rotatedV = rt * adjustedV;
        if(adjustedNormal.y < 0) {
            rotatedU.x = -rotatedU.x;
        } else {
            rotatedU.y = -rotatedU.y;
        }

        // Create Vertices
        List<Vertex> vertexList = new List<Vertex>();
        foreach(var v in polyData.vertices) {
            Vertex vertex = new Vertex();
            vertex.position = v;
            vertexList.Add(vertex);
        }

        // Create faces
        Face[] faces = new Face[1];
        Face face = new Face(GenerateTris(csgOper, polyData));

        AutoUnwrapSettings aus = new AutoUnwrapSettings {
            anchor = AutoUnwrapSettings.Anchor.UpperLeft,
            scale = Vector2.one,
            offset = Vector2.zero,
            rotation = 0,
            fill = AutoUnwrapSettings.Fill.Tile,
            useWorldSpace = false
        };
        face.uv = aus;
        face.manualUV = true;
        faces[0] = face;

        // Create empty sharedVertices and sharedTextures lists
        List<SharedVertex> sharedVertices = new List<SharedVertex>();
        List<SharedVertex> sharedTextures = new List<SharedVertex>();

        // Create Mesh
        ProBuilderMesh pbm = ProBuilderMesh.Create(
            vertexList,
            faces,
            sharedVertices,
            sharedTextures
        );

        List<Vector4> uvs = new List<Vector4>();
        for(int i = 0; i < vertexList.Count; i++) {
            uvs.Add(new Vector4(rotatedU.x, rotatedU.y, rotatedV.x, rotatedV.y));
        }
        pbm.SetUVs(0, uvs);
        pbm.Refresh();

        Material[] sharedMaterials = pbm.GetComponent<Renderer>().sharedMaterials;
        sharedMaterials[0] = material;
        pbm.GetComponent<Renderer>().sharedMaterials = sharedMaterials;

        pbm.ToMesh();
        pbm.Refresh();
        pbm.RebuildWithPositionsAndFaces(polyData.vertices, faces);

        pbm.gameObject.transform.name = index.ToString();
        return pbm.gameObject;
    }

    private static int[] GenerateTris(string csgOper, PolyData polyData) {
        int[] triangles;

        if(csgOper == "CSG_Subtract") {
            if(polyData.vertices.Length == 4) {
                triangles = new int[6]
                {
                    2, 1, 0, // First triangle (top-left, top-right, bottom-left)
                    0, 3, 2  // Second triangle (bottom-left, top-right, bottom-right)
                };
            } else {
                triangles = new int[3]
                {
                    2, 1, 0, // First triangle (top-left, top-right, bottom-left)
                };
            }
        } else {
            if(polyData.vertices.Length == 4) {
                triangles = new int[6]
                {
                    0, 1, 2, // First triangle (top-left, top-right, bottom-left)
                    2, 3, 0  // Second triangle (bottom-left, top-right, bottom-right)
                };
            } else {
                triangles = new int[3]
                {
                    0, 1, 2, // First triangle (top-left, top-right, bottom-left)
                };
            }
        }

        return triangles;
    }

    private static Material GetMaterialForTexture(string texture) {
        string materialPath = TextureUtils.GetMaterialPath(texture);

        Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if(material == null) {
            material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Prefab/Red.mat");
            // Debug.LogError("Missing material for " + texture);
        }

        return material;
    }
}
#endif
