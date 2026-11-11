using System;
using UnityEngine;

public static partial class L2StaticMeshVertexDatabase
{
    // Matches ModelImporter.globalScale on the original StaticMeshes FBX assets.
    public const float StaticMeshFbxScale = 0.019f;

    [Serializable]
    public sealed class MapData
    {
        public int meshCount;
        public int exported;
        public MeshData[] meshes;
    }

    [Serializable]
    public sealed class MeshData
    {
        public string mesh;
        public int vertexCount;
        public string positions;
        public string normals;
        public string uv0;
        public string error;

        public Vector3[] DecodePositions()
        {
            return DecodeVectors(positions, vertexCount, true);
        }

        public Vector3[] DecodeNormals()
        {
            return DecodeVectors(normals, vertexCount, false);
        }

        public Vector2[] DecodeUv0()
        {
            byte[] bytes = Convert.FromBase64String(uv0 ?? "");
            if (bytes.Length != vertexCount * 8)
                return Array.Empty<Vector2>();
            var result = new Vector2[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                int p = i * 8;
                result[i] = new Vector2(
                    ReadSingleLittleEndian(bytes, p),
                    ReadSingleLittleEndian(bytes, p + 4));
            }
            return result;
        }
    }



}
