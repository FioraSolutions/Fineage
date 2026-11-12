#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class L2T3DInfoParser
{
    public static void ParseTerrainInfo(string mapName) {
        terrainInfo.mapName = mapName;
        terrainInfo.uvLayers = new List<L2TerrainLayer>();
        terrainInfo.decoLayers = new List<L2DecoLayer>();

        using (StreamReader reader = new StreamReader(t3dPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                line = line.Trim();
                if (line.StartsWith("Begin Actor Class=TerrainInfo Name=TerrainInfo0")) {
                    while ((line = reader.ReadLine()) != null && !line.Contains("End Actor")) {
                        line = line.Trim();
                        if (line.StartsWith("TerrainMap=")) {
                            terrainInfo.terrainMapPath = TextureUtils.GetHeightMapPath(line);
                        } else if (line.StartsWith("GeneratedSectorCounter=")) {
                            terrainInfo.generatedSectorCounter = 256;
                        } else if (line.StartsWith("TerrainScale=")) {
                            terrainInfo.terrainScale = L2MetaDataUtils.ParseVector3(line);
                        } else if (line.StartsWith("Location=")) {
                            terrainInfo.location = L2MetaDataUtils.ParseVector3(line);
                        } else if (line.StartsWith("Layer")) {
                            L2TerrainLayer layer = L2TerrainInfoParser.ParseL2TerrainLayer(line);
                            if (layer != null) {
                                terrainInfo.uvLayers.Add(layer);
                            }
                        } else if (line.StartsWith("DecoLayers")) {
                            L2DecoLayer layer = L2TerrainInfoParser.ParseL2DecoLayer(line);
                            if (layer != null) {
                                terrainInfo.decoLayers.Add(layer);
                            }
                        }
                    }
                }
            }
        }

        // Debug.Log($"Loaded {terrainInfo.uvLayers.Count} uv layers data.");
        // Debug.Log($"Loaded {terrainInfo.decoLayers.Count} deco layers data.");
    }

    public static List<Brush> ParseBrushInfo(string t3dPath) {
        if (terrainInfo == null) {
            terrainInfo = new L2TerrainInfo();
        }
        terrainInfo.brushes = new List<Brush>();

        using (StreamReader reader = new StreamReader(t3dPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                line = line.Trim();
                if (line.StartsWith("Begin Actor Class=Brush Name=")) {
                    Brush brush = new Brush();
                    brush.model = new Model();
                    brush.model.name = line.Replace("Begin Actor Class=Brush Name=", "");
                    brush.name = line.Replace("Begin Actor Class=Brush Name=", "");
                    brush.model.poly = new Poly();
                    brush.model.poly.name = "Poly";
                    brush.polyFlags = L2MetaDataUtils.ParsePolyFlags(0);

                    List<PolyData> polydatas = new List<PolyData>();
                    // Debug.Log("New Brush " + brush.name);

                    int polyIndex = 0;
                    while ((line = reader.ReadLine()) != null && !line.Contains("End Actor")) {
                        line = line.Trim();

                        if (line.StartsWith("CsgOper=")) {
                            brush.csgOper = line.Replace("CsgOper=", "");
                        } else if (line.StartsWith("Location=")) {
                            brush.position = L2MetaDataUtils.ParseVector3(line);
                        } else if (line.StartsWith("PrePivot=")) {
                            brush.prePivot = L2MetaDataUtils.ParseVector3(line);
                        } else if (line.StartsWith("Begin Polygon")) {
                            PolyData polydata = new PolyData();
                            polydata.polyIndex = polyIndex++;

                            
                            string[] polyInfos = line.Replace("Begin Polygon", "").Trim().Split(" ");

                            foreach (var polyInfo in polyInfos) {
                                if (polyInfo.Contains("Texture=")) {
                                    polydata.texture = polyInfo.Replace("Texture=", "");
                                }
                                if (polyInfo.Contains("Flags=")) {
                                    string flags = polyInfo.Replace("Flags=", "");
                                    polydata.polyFlags = L2MetaDataUtils.ParsePolyFlags(int.Parse(flags));
                                }
                            }

                            List<Vector3> vertices = new List<Vector3>();
                            while ((line = reader.ReadLine()) != null && !line.Contains("End Polygon")) {
                                line = line.Trim();
                                if (line.Contains("Origin")) {
                                    polydata.origin = L2MetaDataUtils.ParseVector3Poly(line.Replace("Origin", "").Trim());
                                }
                                if (line.Contains("TextureU")) {
                                    polydata.textureU = L2MetaDataUtils.ParseVector3Poly(line.Replace("TextureU", "").Trim());
                                }
                                if (line.Contains("TextureV")) {
                                    polydata.textureV = L2MetaDataUtils.ParseVector3Poly(line.Replace("TextureV", "").Trim());
                                }
                                if (line.Contains("Normal")) {
                                    polydata.normal = L2MetaDataUtils.ParseVector3Poly(line.Replace("Normal", "").Trim());
                                }
                                if (line.Contains("Vertex")) {
                                    vertices.Add(L2MetaDataUtils.ParseVector3Poly(line.Replace("Vertex", "").Trim()));
                                }
                            }

                            // Debug.Log("Poly Verticle count " + vertices.Count);

                            polydata.vertices = vertices.ToArray();

                            if (polydata.polyFlags == null || polydata.polyFlags.Length == 0) {
                                polydata.polyFlags = L2MetaDataUtils.ParsePolyFlags(0);
                            }

                            polydatas.Add(polydata);

                            brush.model.poly.polyData = polydatas.ToArray();
                            brush.model.poly.polyCount = polydatas.Count;
                        }
                    }

                    // Debug.Log(brush);
                    // Debug.Log(brush.model.poly);
                    // Debug.Log("Brush poly count " + brush.model.poly.polyCount);
                    terrainInfo.brushes.Add(brush);
                }
            }
        }

        // Debug.Log($"Loaded {terrainInfo.brushes.Count} brushe(s).");

        return terrainInfo.brushes;
    }
}
#endif
