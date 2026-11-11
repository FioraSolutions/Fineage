#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class L2TerrainInfoParser
{
    public static L2TerrainLayer ParseL2TerrainLayer(string line) {
        L2TerrainLayer layer = new L2TerrainLayer();

        int equalsIndex = line.IndexOf('=');
        string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);
        string[] valueParts = valueString.Split(',');

        if (valueParts.Length < 4) {
            // Debug.LogError("Invalid layer line: " + line);
            return null;
        }

        string textureInfo = string.Empty;
        string alphaMapInfo = string.Empty;
        string uScaleInfo = string.Empty;
        string vScaleInfo = string.Empty;

        for (int i = 0; i < valueParts.Length; i++) {
            if (valueParts[i].Contains("Texture=")) {
                textureInfo = valueParts[i].Trim();
            }
            if (valueParts[i].Contains("AlphaMap=")) {
                alphaMapInfo = valueParts[i].Trim();
            }
            if (valueParts[i].Contains("UScale=")) {
                uScaleInfo = valueParts[i].Trim();
            }
            if (valueParts[i].Contains("VScale=")) {
                vScaleInfo = valueParts[i].Trim();
            }
        }

        if (!textureInfo.Contains("Texture")) {
            return null;
        }

        layer.texture = TextureUtils.LoadTexture2DFromInfo(textureInfo, L2TerrainGeneratorTool.UV_TEXTURE_SIZE);
        if (alphaMapInfo != string.Empty) {
            layer.alphaMap = TextureUtils.LoadTexture2DFromInfo(alphaMapInfo, L2TerrainGeneratorTool.UV_LAYER_ALPHAMAP_SIZE);
        }

        // Parse uScale and vScale
        float uScale = L2MetaDataUtils.ParseFloatFromInfo(uScaleInfo);
        float vScale = L2MetaDataUtils.ParseFloatFromInfo(vScaleInfo);

        layer.uScale = uScale;
        layer.vScale = vScale;

        // TerrainMatrix WPlane X/Y = UV offset of the tile matrix (TI bake stores offset/scale in _Layer_N_UV.zw)
        var wm = System.Text.RegularExpressions.Regex.Match(line, @"WPlane=\(W=[-0-9.]+,X=([-0-9.]+),Y=([-0-9.]+)");
        if (wm.Success) {
            layer.uOffset = float.Parse(wm.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
            layer.vOffset = float.Parse(wm.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
        }

        return layer;
    }

    public static L2DecoLayer ParseL2DecoLayer(string line) {
        L2DecoLayer layer = new L2DecoLayer();

        string showOnTerrain = string.Empty;
        string densityMap = string.Empty;
        string staticMesh = string.Empty;
        string scaleMultiplier = string.Empty;

        int equalsIndex = line.IndexOf('=');
        string valueString = line.Substring(equalsIndex + 1, line.Length - equalsIndex - 2);
        string[] valueParts = valueString.Split(',');

        for (int i = 0; i < valueParts.Length; i++) {
            if (valueParts[i].Contains("ShowOnTerrain=")) {
                showOnTerrain = valueParts[i].Trim();
            }
            if (valueParts[i].Contains("DensityMap=")) {
                densityMap = valueParts[i].Trim();
            }
            if (valueParts[i].Contains("StaticMesh=")) {
                staticMesh = valueParts[i].Trim();
            }
            if (valueParts[i].Contains("ScaleMultiplier=")) {
                int index = line.IndexOf("ScaleMultiplier=");
                string fromIndex = line.Substring(index);
                int argEndIndex = fromIndex.IndexOf("))");
                scaleMultiplier = fromIndex.Substring(0, argEndIndex + 2);
            }
        }

        layer.showOnTerrain = L2MetaDataUtils.ParseBoolFromInfo(showOnTerrain);
        layer.densityMapPath = TextureUtils.GetTexturePath(densityMap);
        layer.densityMap = TextureUtils.LoadTexture2DFromInfo(densityMap, L2TerrainGeneratorTool.DECO_LAYER_ALPHAMAP_SIZE);
        layer = UpdateDecoLayerScale(layer, scaleMultiplier);
        layer.staticMesh = StaticMeshUtils.LoadMeshFromInfo(staticMesh);

        return layer;
    }
}
#endif
