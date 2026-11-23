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
        terrainInfo = new L2TerrainInfo();
        bool hasT3d = File.Exists(t3dPath);

        if (hasT3d) {
            ParseStaticMeshInfo(null);
            ParseTerrainInfo(mapName);
        } else {
            terrainInfo.staticMeshes = new List<L2StaticMesh>();
        }

        if (!hasT3d || terrainInfo.quadVisibilityBitmap == null) {
            string info0 = L2InterludeMapPaths.TerrainInfo0AbsolutePath(mapName);
            if (File.Exists(info0)) {
                L2TerrainInfo fromTxt = L2TerrainInfoParser.ParseTerrainInfo0(mapName);
                if (fromTxt != null) {
                    if (!hasT3d) {
                        terrainInfo = fromTxt;
                        if (terrainInfo.staticMeshes == null) {
                            terrainInfo.staticMeshes = new List<L2StaticMesh>();
                        }
                    } else if (fromTxt.quadVisibilityBitmap != null) {
                        terrainInfo.quadVisibilityBitmap = fromTxt.quadVisibilityBitmap;
                    }
                }
            } else if (!hasT3d) {
                Debug.LogError("[Interlude terrain] no t3d or TerrainInfo0.txt for " + mapName
                    + " at " + t3dPath + " / " + info0);
                return null;
            }
        }

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
                    if (string.IsNullOrEmpty(mesh.staticMesh))
                    {
                        Debug.LogError("[StaticMesh] actor in " + path + " has no StaticMesh= reference");
                        continue;
                    }
                    terrainInfo.staticMeshes.Add(mesh);
                }
            }
        }

        // Debug.Log($"Loaded {terrainInfo.staticMeshes.Count} staticmeshes data.");
    }

}

#endif