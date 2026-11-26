using System;
using System.Collections.Generic;

/// <summary>
/// Rebuilds DecoLayer instances the way UE2 does: per quad, from the density map,
/// <c>DensityMultiplier</c>, <c>MaxPerQuad</c>, <c>ScaleMultiplier</c> / ScaleMap, and <c>Seed</c>.
/// <c>UpdateDecorations</c> is native and is not in the Interlude script dump. Each quad is
/// reseeded from the official layer <c>Seed</c> plus its coordinates so a sector can be
/// rebuilt on its own, which is how UE2 fills <c>DecoSectorInfo</c> around the camera.
/// The Windows <c>appRand</c> LCG matches the rest of this client (214013 / 2531011).
/// </summary>
public struct L2DecoInstance
{
    public int Layer;
    public float UnityX;
    public float UnityZ;
    public float YawDegrees;
    public float UnityScaleX;
    public float UnityScaleY;
    public float UnityScaleZ;
    public byte R;
    public byte G;
    public byte B;
    public byte A;
}

public static class L2DecoGenerator
{
    public const float UnrealUnitsPerMeter = 52.5f;
    public const uint AppRandMultiplier = 214013u;
    public const uint AppRandIncrement = 2531011u;

    public static uint AdvanceAppRand(uint state)
    {
        return unchecked(state * AppRandMultiplier + AppRandIncrement);
    }

    public static float AppFrand(ref uint state)
    {
        state = AdvanceAppRand(state);
        return ((state >> 16) & 0x7FFFu) / 32767f;
    }

    /// <summary>Official appRound for a non-negative count: floor(x + 0.5).</summary>
    public static int CountForDensity(float densityMin, float densityMax, int maxPerQuad, int alpha)
    {
        if (alpha <= 0)
            return 0;
        float t = alpha / 255f;
        float value = densityMin + (densityMax - densityMin) * t;
        int count = (int)Math.Floor(value + 0.5f);
        if (count < 0)
            count = 0;
        if (maxPerQuad >= 0 && count > maxPerQuad)
            count = maxPerQuad;
        return count;
    }

    public static bool IsQuadVisible(int[] words, int quadsX, int qx, int qy)
    {
        if (words == null || quadsX <= 0 || qx < 0 || qy < 0)
            return false;
        int i = qy * quadsX + qx;
        int word = i >> 5;
        int bit = i & 31;
        if (word < 0 || word >= words.Length)
            return false;
        uint bits = unchecked((uint)words[word]);
        return ((bits >> bit) & 1u) != 0u;
    }

    /// <summary>
    /// Same corner as <c>L2TerrainGenerator</c>: Unity X = UE Y, Unity Z = UE X,
    /// origin shifted by half the tile plus one sector for the seam.
    /// </summary>
    public static void QuadToUnityMeters(
        L2DecoTileFile tile, int qx, int qy, float fx, float fy, out float unityX, out float unityZ)
    {
        float halfX = tile.QuadsX * tile.ScaleX * 0.5f;
        float halfY = tile.QuadsY * tile.ScaleY * 0.5f;
        float minUeX = tile.OriginX - halfX - tile.ScaleX;
        float minUeY = tile.OriginY - halfY - tile.ScaleY;
        float ueX = minUeX + (qx + fx) * tile.ScaleX;
        float ueY = minUeY + (qy + fy) * tile.ScaleY;
        unityX = ueY / UnrealUnitsPerMeter;
        unityZ = ueX / UnrealUnitsPerMeter;
    }

    public static void AppendSector(L2DecoTileFile tile, int layerIndex, int sectorX, int sectorY, List<L2DecoInstance> into)
    {
        if (tile == null || tile.Layers == null || layerIndex < 0 || layerIndex >= tile.Layers.Length || into == null)
            return;
        L2DecoLayerInput layer = tile.Layers[layerIndex];
        if (layer == null || (!layer.ShowOnTerrain && !layer.ForceRender))
            return;
        if (layer.DensityAlpha == null || tile.SectorQuads <= 0)
            return;

        int x0 = sectorX * tile.SectorQuads;
        int y0 = sectorY * tile.SectorQuads;
        int x1 = x0 + tile.SectorQuads;
        int y1 = y0 + tile.SectorQuads;
        if (x1 > tile.QuadsX) x1 = tile.QuadsX;
        if (y1 > tile.QuadsY) y1 = tile.QuadsY;
        for (int qy = y0; qy < y1; qy++)
        {
            for (int qx = x0; qx < x1; qx++)
            {
                if (!layer.ShowOnInvisibleTerrain && !IsQuadVisible(tile.VisibilityWords, tile.QuadsX, qx, qy))
                    continue;
                int alpha = AlphaAt(layer.DensityAlpha, layer.DensityWidth, layer.DensityHeight, qx, qy, tile.QuadsX, tile.QuadsY);
                int count = CountForDensity(layer.DensityMin, layer.DensityMax, layer.MaxPerQuad, alpha);
                if (count <= 0)
                    continue;
                uint state = QuadState(layer.Seed, qx, qy);
                for (int n = 0; n < count; n++)
                    into.Add(MakeInstance(tile, layer, layerIndex, qx, qy, ref state));
            }
        }
    }

    static uint QuadState(int seed, int qx, int qy)
    {
        return unchecked((uint)seed + (uint)qx + ((uint)qy << 16));
    }

    static L2DecoInstance MakeInstance(
        L2DecoTileFile tile, L2DecoLayerInput layer, int layerIndex, int qx, int qy, ref uint state)
    {
        float fx = AppFrand(ref state);
        float fy = AppFrand(ref state);
        float scaleT;
        if (layer.ScaleAlpha != null && layer.ScaleWidth > 0)
        {
            int sample = AlphaAt(layer.ScaleAlpha, layer.ScaleWidth, layer.ScaleHeight, qx, qy, tile.QuadsX, tile.QuadsY);
            scaleT = sample / 255f;
        }
        else
        {
            scaleT = AppFrand(ref state);
        }

        float yaw = 0f;
        if (layer.RandomYaw)
            yaw = AppFrand(ref state) * 360f;

        var spawn = new L2DecoInstance();
        spawn.Layer = layerIndex;
        QuadToUnityMeters(tile, qx, qy, fx, fy, out spawn.UnityX, out spawn.UnityZ);
        spawn.YawDegrees = yaw;
        // UE X/Y/Z land on Unity Z/X/Y, the same swap as static-mesh placement.
        float ueX = Lerp(layer.ScaleXMin, layer.ScaleXMax, scaleT);
        float ueY = Lerp(layer.ScaleYMin, layer.ScaleYMax, scaleT);
        float ueZ = Lerp(layer.ScaleZMin, layer.ScaleZMax, scaleT);
        spawn.UnityScaleX = ueY;
        spawn.UnityScaleY = ueZ;
        spawn.UnityScaleZ = ueX;
        spawn.R = 255;
        spawn.G = 255;
        spawn.B = 255;
        spawn.A = 255;
        if (layer.ColorRgba != null && layer.ColorWidth > 0 && layer.ColorHeight > 0)
        {
            int tx = Texel(qx, tile.QuadsX, layer.ColorWidth);
            int ty = Texel(qy, tile.QuadsY, layer.ColorHeight);
            int o = (ty * layer.ColorWidth + tx) * 4;
            if (o + 3 < layer.ColorRgba.Length)
            {
                spawn.R = layer.ColorRgba[o];
                spawn.G = layer.ColorRgba[o + 1];
                spawn.B = layer.ColorRgba[o + 2];
                spawn.A = layer.ColorRgba[o + 3];
            }
        }

        return spawn;
    }

    static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    static int AlphaAt(byte[] data, int width, int height, int qx, int qy, int quadsX, int quadsY)
    {
        if (data == null || width <= 0 || height <= 0)
            return 0;
        int tx = Texel(qx, quadsX, width);
        int ty = Texel(qy, quadsY, height);
        int i = ty * width + tx;
        if (i < 0 || i >= data.Length)
            return 0;
        return data[i];
    }

    static int Texel(int quad, int quads, int texels)
    {
        if (quads <= 0 || texels <= 0)
            return 0;
        int t = (int)(((quad + 0.5f) * texels) / quads);
        if (t < 0)
            return 0;
        if (t >= texels)
            return texels - 1;
        return t;
    }
}
