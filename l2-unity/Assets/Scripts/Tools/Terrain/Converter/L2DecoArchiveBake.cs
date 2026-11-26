#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Writes <c>XX_YY_Deco.bytes</c>: official DecoLayer inputs, not expanded instances.
/// </summary>
public static class L2DecoArchiveBake
{
    public const string FileSuffix = "_Deco.bytes";

    public static bool WriteForMap(string mapName)
    {
        if (string.IsNullOrEmpty(mapName))
        {
            Debug.LogError("[Interlude deco] missing tile id");
            return false;
        }

        L2TerrainInfo info = L2T3DInfoParser.LoadMetadata(mapName);
        if (info == null)
        {
            Debug.LogError("[Interlude deco] " + mapName + ": TerrainInfo metadata missing");
            return false;
        }

        L2DecoTileFile tile = BuildTile(mapName, info);
        L2DecoArchiveResult result = L2DecoLayerArchive.Write(tile);
        for (int i = 0; i < result.Errors.Count; i++)
            Debug.LogError(result.Errors[i]);
        if (result.Bytes == null)
            return false;

        string dir = Path.Combine("Assets", "Resources", "Data", "Maps", mapName);
        Directory.CreateDirectory(dir);
        string assetPath = Path.Combine(dir, mapName + FileSuffix).Replace('\\', '/');
        File.WriteAllBytes(assetPath, result.Bytes);
        AssetDatabase.ImportAsset(assetPath);
        Debug.Log("[Interlude deco] " + mapName + " wrote " + result.Bytes.Length + " bytes → " + assetPath
            + " (" + (tile.Layers == null ? 0 : tile.Layers.Length) + " layers)");
        return result.Errors.Count == 0;
    }

    public static L2DecoTileFile BuildTile(string mapName, L2TerrainInfo info)
    {
        var tile = new L2DecoTileFile();
        tile.TileId = mapName;
        tile.OriginX = info.location.x;
        tile.OriginY = info.location.y;
        tile.OriginZ = info.location.z;
        tile.ScaleX = info.terrainScale.x;
        tile.ScaleY = info.terrainScale.y;
        tile.ScaleZ = info.terrainScale.z;
        tile.QuadsX = L2TerrainInfoParser.QuadVisibilitySize;
        tile.QuadsY = L2TerrainInfoParser.QuadVisibilitySize;
        tile.SectorQuads = L2DecoTileFile.Ue2TerrainSectorSizeDefault;
        tile.DecoLayerOffsetUu = 0f;
        tile.VisibilityWords = info.quadVisibilityBitmap;
        if (info.generatedSectorCounter > 0 && info.generatedSectorCounter != tile.QuadsX)
        {
            Debug.LogError("[Interlude deco] " + mapName + ": GeneratedSectorCounter "
                + info.generatedSectorCounter + " != QuadVisibility " + tile.QuadsX);
        }

        var layers = new List<L2DecoLayerInput>();
        if (info.decoLayers != null)
        {
            for (int i = 0; i < info.decoLayers.Count; i++)
                layers.Add(BuildLayer(mapName, i, info.decoLayers[i]));
        }

        tile.Layers = layers.ToArray();
        return tile;
    }

    static L2DecoLayerInput BuildLayer(string mapName, int index, L2DecoLayer source)
    {
        var layer = new L2DecoLayerInput();
        if (source == null)
            return layer;
        layer.ShowOnTerrain = source.showOnTerrain;
        layer.HasShowOnTerrain = source.hasShowOnTerrain;
        layer.AlignToTerrain = source.alignToTerrain;
        layer.HasAlign = source.hasAlign;
        layer.RandomYaw = source.randomYaw;
        layer.HasRandomYaw = source.hasRandomYaw;
        layer.ShowOnInvisibleTerrain = source.showOnInvisibleTerrain;
        layer.HasShowOnInvisible = source.hasShowOnInvisible;
        layer.LitDirectional = source.litDirectional;
        layer.DisregardTerrainLighting = source.disregardTerrainLighting;
        layer.ForceRender = source.forceRender;
        layer.DrawOrder = (byte)Mathf.Clamp(source.drawOrder, 0, 255);
        layer.HasDrawOrder = source.hasDrawOrder;
        layer.Seed = source.seed;
        layer.HasSeed = source.hasSeed;
        layer.MaxPerQuad = source.hasMaxPerQuad ? source.maxPerQuad : -1;
        layer.HasMaxPerQuad = source.hasMaxPerQuad;
        layer.FadeMinUu = source.fadeMinUu;
        layer.FadeMaxUu = source.fadeMaxUu;
        layer.HasFade = source.hasFade;
        layer.DensityMin = source.densityMin;
        layer.DensityMax = source.densityMax;
        layer.HasDensity = source.hasDensity;
        layer.ScaleXMin = source.scaleXMin;
        layer.ScaleXMax = source.scaleXMax;
        layer.ScaleYMin = source.scaleYMin;
        layer.ScaleYMax = source.scaleYMax;
        layer.ScaleZMin = source.scaleZMin;
        layer.ScaleZMax = source.scaleZMax;
        layer.HasScale = source.hasScale;
        layer.MeshRef = source.staticMeshRef ?? string.Empty;
        layer.DensityRef = source.densityMapRef ?? string.Empty;
        layer.ScaleRef = source.scaleMapRef ?? string.Empty;
        layer.ColorRef = source.colorMapRef ?? string.Empty;
        layer.MeshResourcePath = ResolveMeshResource(mapName, index, layer.MeshRef);

        if (!string.IsNullOrEmpty(source.densityMapPath))
            ReadAlpha(mapName, index, "DensityMap", source.densityMapPath, out layer.DensityWidth, out layer.DensityHeight, out layer.DensityAlpha);
        if (!string.IsNullOrEmpty(layer.ScaleRef))
        {
            string path = TextureUtils.GetTexturePath(layer.ScaleRef);
            ReadAlpha(mapName, index, "ScaleMap", path, out layer.ScaleWidth, out layer.ScaleHeight, out layer.ScaleAlpha);
        }
        if (!string.IsNullOrEmpty(layer.ColorRef))
        {
            string path = TextureUtils.GetTexturePath(layer.ColorRef);
            ReadRgba(mapName, index, path, out layer.ColorWidth, out layer.ColorHeight, out layer.ColorRgba);
        }

        return layer;
    }

    static string ResolveMeshResource(string mapName, int index, string meshRef)
    {
        if (string.IsNullOrEmpty(meshRef) || meshRef.IndexOf("None", StringComparison.Ordinal) >= 0)
            return string.Empty;
        string fbx = StaticMeshUtils.GetMeshPath(meshRef);
        string prefab = Path.Combine(
            Path.GetDirectoryName(fbx) ?? string.Empty,
            "Prefab",
            Path.GetFileNameWithoutExtension(fbx) + ".prefab").Replace('\\', '/');
        string chosen = File.Exists(AbsoluteAsset(prefab)) ? prefab : fbx;
        if (!File.Exists(AbsoluteAsset(chosen)))
        {
            Debug.LogError("[Interlude deco] " + mapName + " layer " + index + ": StaticMesh file missing " + chosen + " (" + meshRef + ")");
            return string.Empty;
        }

        string rel = chosen.Replace('\\', '/');
        const string prefix = "Assets/Resources/";
        if (!rel.StartsWith(prefix, StringComparison.Ordinal))
        {
            Debug.LogError("[Interlude deco] " + mapName + " layer " + index + ": StaticMesh is outside Resources " + rel);
            return string.Empty;
        }

        string resource = rel.Substring(prefix.Length);
        int dot = resource.LastIndexOf('.');
        if (dot > 0)
            resource = resource.Substring(0, dot);
        return resource;
    }

    static void ReadAlpha(string mapName, int index, string kind, string assetPath, out int width, out int height, out byte[] alpha)
    {
        width = 0;
        height = 0;
        alpha = null;
        if (!TryLoadPixels(mapName, index, kind, assetPath, out Color32[] pixels, out width, out height))
            return;
        alpha = new byte[width * height];
        for (int y = 0; y < height; y++)
        {
            int srcY = height - 1 - y;
            for (int x = 0; x < width; x++)
                alpha[y * width + x] = pixels[srcY * width + x].a;
        }
    }

    static void ReadRgba(string mapName, int index, string assetPath, out int width, out int height, out byte[] rgba)
    {
        width = 0;
        height = 0;
        rgba = null;
        if (!TryLoadPixels(mapName, index, "ColorMap", assetPath, out Color32[] pixels, out width, out height))
            return;
        rgba = new byte[width * height * 4];
        for (int y = 0; y < height; y++)
        {
            int srcY = height - 1 - y;
            for (int x = 0; x < width; x++)
            {
                Color32 c = pixels[srcY * width + x];
                int o = (y * width + x) * 4;
                rgba[o] = c.r;
                rgba[o + 1] = c.g;
                rgba[o + 2] = c.b;
                rgba[o + 3] = c.a;
            }
        }
    }

    static bool TryLoadPixels(string mapName, int index, string kind, string assetPath, out Color32[] pixels, out int width, out int height)
    {
        pixels = null;
        width = 0;
        height = 0;
        string full = AbsoluteAsset(assetPath);
        if (string.IsNullOrEmpty(full) || !File.Exists(full))
        {
            Debug.LogError("[Interlude deco] " + mapName + " layer " + index + ": " + kind + " file missing " + assetPath);
            return false;
        }

        byte[] png = File.ReadAllBytes(full);
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!tex.LoadImage(png))
        {
            Debug.LogError("[Interlude deco] " + mapName + " layer " + index + ": " + kind + " is not a readable image " + assetPath);
            UnityEngine.Object.DestroyImmediate(tex);
            return false;
        }

        width = tex.width;
        height = tex.height;
        pixels = tex.GetPixels32();
        UnityEngine.Object.DestroyImmediate(tex);
        return pixels != null && pixels.Length == width * height;
    }

    static string AbsoluteAsset(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath))
            return string.Empty;
        string rel = assetPath.Replace('\\', '/');
        if (rel.StartsWith("Assets/", StringComparison.Ordinal))
            rel = rel.Substring("Assets/".Length);
        return Path.Combine(Application.dataPath, rel);
    }
}
#endif
