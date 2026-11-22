#if (UNITY_EDITOR) 
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;
using System.Text.RegularExpressions;

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

        if (line.IndexOf("Texture=None", StringComparison.OrdinalIgnoreCase) >= 0
            || line.IndexOf("Texture=none", StringComparison.OrdinalIgnoreCase) >= 0) {
            return null;
        }

        if (!textureInfo.Contains("Texture")) {
            return null;
        }

        layer.diffuseTextureRef = textureInfo;
        layer.texture = TextureUtils.LoadTexture2DFromInfo(textureInfo, L2TerrainGeneratorTool.UV_TEXTURE_SIZE);
        if (layer.texture == null) {
            string path = TextureUtils.GetTexturePath(textureInfo);
            Debug.LogError(
                "Terrain UV layer diffuse texture missing: ref=" + textureInfo.Trim()
                + " assetPath=" + path);
        }
        if (alphaMapInfo != string.Empty) {
            layer.alphaMap = TextureUtils.LoadTexture2DFromInfo(alphaMapInfo, L2TerrainGeneratorTool.UV_LAYER_ALPHAMAP_SIZE);
            if (layer.alphaMap == null) {
                string path = TextureUtils.GetTexturePath(alphaMapInfo);
                Debug.LogError(
                    "Terrain UV layer alphamap missing: ref=" + alphaMapInfo.Trim()
                    + " assetPath=" + path);
            }
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

        var up = System.Text.RegularExpressions.Regex.Match(line, @"UPan=([-0-9.]+)");
        var vp = System.Text.RegularExpressions.Regex.Match(line, @"VPan=([-0-9.]+)");
        if (up.Success) {
            layer.uOffset += float.Parse(up.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
        }
        if (vp.Success) {
            layer.vOffset += float.Parse(vp.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
        }

        var tr = System.Text.RegularExpressions.Regex.Match(line, @"TextureRotation=([-0-9.]+)");
        if (tr.Success) {
            layer.textureRotation = float.Parse(tr.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
        }

        var ax = System.Text.RegularExpressions.Regex.Match(line, @"TextureMapAxis=([A-Za-z0-9_]+)");
        if (ax.Success) {
            string a = ax.Groups[1].Value;
            if (a == "TEXMAPAXIS_XZ" || a == "1") {
                layer.textureMapAxis = 1;
            } else if (a == "TEXMAPAXIS_YZ" || a == "2") {
                layer.textureMapAxis = 2;
            } else {
                layer.textureMapAxis = 0;
            }
        }

        var ua = System.Text.RegularExpressions.Regex.Match(line, @"bUseAlpha=(True|False|true|false|1|0)");
        if (ua.Success) {
            string v = ua.Groups[1].Value;
            layer.useAlpha = v == "True" || v == "true" || v == "1";
        }

        var sc = System.Text.RegularExpressions.Regex.Match(line, @"Scale=\(X=([-0-9.]+),Y=([-0-9.]+),Z=([-0-9.]+)\)");
        if (sc.Success) {
            layer.layerScale = new Vector3(
                float.Parse(sc.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture),
                float.Parse(sc.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture),
                float.Parse(sc.Groups[3].Value, System.Globalization.CultureInfo.InvariantCulture));
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
        ParseDecoRange(line, "FadeoutRadius", out float fadeMin, out float fadeMax);
        layer.fadeMinUu = fadeMin;
        layer.fadeMaxUu = fadeMax;
        ParseDecoRange(line, "DensityMultiplier", out float dMin, out float dMax);
        layer.densityMin = dMin;
        layer.densityMax = dMax;
        layer.maxPerQuad = ParseDecoInt(line, "MaxPerQuad");
        layer.seed = ParseDecoInt(line, "Seed");
        layer.alignToTerrain = ParseDecoInt(line, "AlignToTerrain") != 0;
        layer.randomYaw = ParseDecoInt(line, "RandomYaw", 1) != 0;
        if (layer.densityMap == null && !string.IsNullOrEmpty(densityMap) && !densityMap.Contains("None"))
            Debug.LogError("[Interlude deco] missing density map " + densityMap);
        if (layer.staticMesh == null && !string.IsNullOrEmpty(staticMesh) && !staticMesh.Contains("None"))
            Debug.LogError("[Interlude deco] missing StaticMesh " + staticMesh);
        return layer;
    }

    static void ParseDecoRange(string line, string key, out float min, out float max)
    {
        min = 0f;
        max = 0f;
        var m = System.Text.RegularExpressions.Regex.Match(line, key + @"=\(Min=([-0-9.]+),Max=([-0-9.]+)\)");
        if (m.Success)
        {
            float.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out min);
            float.TryParse(m.Groups[2].Value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out max);
        }
    }

    /// <summary>
    /// Official TerrainInfo layer 0 is the base and has no AlphaMap. Every later
    /// used layer must name an AlphaMap; a missing field is an official-data gap
    /// and must not be papered over with a generated mask.
    /// </summary>
    public static void ValidateOfficialUvLayers(string mapId, IList<L2TerrainLayer> layers)
    {
        if (layers == null)
            return;
        string map = string.IsNullOrEmpty(mapId) ? "(unknown map)" : mapId;
        for (int i = 1; i < layers.Count; i++)
        {
            L2TerrainLayer layer = layers[i];
            if (layer == null)
                continue;
            bool hasTexture = layer.texture != null
                || (!string.IsNullOrEmpty(layer.diffuseTextureRef)
                    && layer.diffuseTextureRef.IndexOf("Texture=None", StringComparison.OrdinalIgnoreCase) < 0);
            if (hasTexture && layer.alphaMap == null)
            {
                string texRef = layer.diffuseTextureRef;
                if (!string.IsNullOrEmpty(texRef))
                    texRef = texRef.Trim().TrimStart('(');
                Debug.LogError(
                    "[Interlude terrain] " + map + " official layer " + i
                    + " has " + (string.IsNullOrEmpty(texRef) ? "a Texture" : texRef)
                    + " but no AlphaMap field; the layer cannot be painted without inventing a mask.");
            }
        }
    }

    static int ParseDecoInt(string line, string key, int fallback = 0)
    {
        var m = System.Text.RegularExpressions.Regex.Match(line, key + @"=([-0-9]+)");
        if (m.Success && int.TryParse(m.Groups[1].Value, out int v))
            return v;
        return fallback;
    }
}
#endif
