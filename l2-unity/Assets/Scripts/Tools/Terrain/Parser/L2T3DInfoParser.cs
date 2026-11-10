#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class L2T3DInfoParser {
    public static L2TerrainInfo terrainInfo;
    public static string t3dPath;

    public static L2TerrainInfo LoadMetadata(string mapName) {
        t3dPath = StaticMeshUtils.GetT3DPath(mapName);
        if (!File.Exists(t3dPath)) {
            // Debug.Log("No t3d for map " + mapName + " at path " + t3dPath);
            return null;
        }

        terrainInfo = new L2TerrainInfo();

        ParseStaticMeshInfo(null);
        ParseTerrainInfo(mapName);

        return terrainInfo;
    }

    public static L2TerrainInfo LoadStaticMeshInfo(string filePath) {
        terrainInfo = new L2TerrainInfo();

        ParseStaticMeshInfo(filePath);

        return terrainInfo;
    }

    private static void ParseStaticMeshInfo(string givenpath) {
        terrainInfo.staticMeshes = new List<L2StaticMesh>();

        string path = t3dPath;
        if (givenpath != null && givenpath.Length != 0) {
            // Debug.LogWarning("Reading staticmeshactors from file " + givenpath);
            path = givenpath;
        }

        using (StreamReader reader = new StreamReader(path)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                line = line.Trim();
                L2StaticMesh mesh = new L2StaticMesh();
                if (line.StartsWith("Begin Actor Class=StaticMeshActor Name=")) {
                    while ((line = reader.ReadLine()) != null && !line.Contains("End Actor")) {
                        line = line.Trim();

                        if (line.StartsWith("StaticMesh=StaticMesh'")) {
                            mesh.staticMesh = line.Replace("StaticMesh=StaticMesh'", "").Replace("'", "");
                        }

                        if (line.StartsWith("SwayRotationOrig=")) {
                            mesh.eulerAngles = L2MetaDataUtils.ParseRotation(line);
                        }

                        if (line.StartsWith("Location=")) {
                            Vector3 pos = L2MetaDataUtils.ParseVector3(line);
                            mesh.x = pos.x;
                            mesh.y = pos.y;
                            mesh.z = pos.z;
                        }

                        if (line.StartsWith("DrawScale=")) {
                            string[] parts = line.Split("=");
                            mesh.scaleX = float.Parse(parts[1]);
                            mesh.scaleY = float.Parse(parts[1]);
                            mesh.scaleZ = float.Parse(parts[1]);
                        }

                        if (line.StartsWith("DrawScale3D=")) {
                            mesh.drawScale3D = L2MetaDataUtils.ParseVector3(line);
                            mesh.hasDrawScale3D = true;
                        }
                    }
                    terrainInfo.staticMeshes.Add(mesh);
                }
            }
        }

        // Debug.Log($"Loaded {terrainInfo.staticMeshes.Count} staticmeshes data.");
    }

}

#endif