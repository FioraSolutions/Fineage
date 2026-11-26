#ifndef L2_TERRAIN_COMMON_INCLUDED
#define L2_TERRAIN_COMMON_INCLUDED

// Color UV: projected world UU / (layer Scale * UScale). _Layer_UV.xy is that repeat.
// .zw is UPan/VPan. Height (UE Z) is a separate interpolator so XZ/YZ cliffs project.
// Splat UV: mesh UV0 rotated 90° around 0.5, then (i + 0.5) / 256.
// GenerateTriangles writes mask UV as (globalVertex + 0.5) / mapSize. Map size is 256.
// Stored UV is i/254, including the extra edge vertex i=255.
// Overlay alphamap: Texture2DArray slice i = official layer i+1, mask in .r.
// Overlay count is the array depth, not a fixed slice cap.
//
// UnTerrain.cpp:
//   TriangulateLayer(pass) — one triangle list per layer, not the full tile.
//   PassShouldRenderTriangle + IsTriangleAll(this, 0) — skip if this layer is 0.
//   IsTriangleAll(later, 255) — skip if a later layer is fully opaque.
//   GetLayerAlpha / GetAlpha_DXT — raw 0..255, no extra scale.
// Pixel version of that cull (full mesh still submitted).

// Sun grid from L2unpacker Output/ExtractStoredIntensity.java (sector windows).
// The ray rebake (terrain-normals) is the emergency fallback only.
float4 _L2TerrainIntensityOrigin;
float4 _L2TerrainIntensityOrigin1;
float4 _L2TerrainIntensityOrigin2;
TEXTURE2D(_L2TerrainIntensityMap);
TEXTURE2D(_L2TerrainIntensityMap1);
TEXTURE2D(_L2TerrainIntensityMap2);

void L2_TerrainIntensityCell(float2 worldUu, float4 origin, out float gx, out float gy, out float grid)
{
    float cell = max(origin.z, 1.0);
    grid = max(origin.w, 1.0);
    gx = (worldUu.y - origin.x) / cell;
    gy = (worldUu.x - origin.y) / cell;
}

bool L2_TerrainIntensityInside(float gx, float gy, float grid)
{
    return gx >= 0.0 && gy >= 0.0 && gx <= grid - 1.0 && gy <= grid - 1.0;
}

float4 L2_TerrainIntensitySample(float2 worldUu)
{
    float gx;
    float gy;
    float grid;
    L2_TerrainIntensityCell(worldUu, _L2TerrainIntensityOrigin, gx, gy, grid);
    if (L2_TerrainIntensityInside(gx, gy, grid))
    {
        float2 uv = (float2(gx, gy) + 0.5) / grid;
        return SAMPLE_TEXTURE2D_LOD(_L2TerrainIntensityMap, sampler_PointClamp, uv, 0);
    }

    if (_L2TerrainIntensityOrigin1.w > 1.0)
    {
        L2_TerrainIntensityCell(worldUu, _L2TerrainIntensityOrigin1, gx, gy, grid);
        if (L2_TerrainIntensityInside(gx, gy, grid))
        {
            float2 uv = (float2(gx, gy) + 0.5) / grid;
            return SAMPLE_TEXTURE2D_LOD(_L2TerrainIntensityMap1, sampler_PointClamp, uv, 0);
        }
    }

    if (_L2TerrainIntensityOrigin2.w > 1.0)
    {
        L2_TerrainIntensityCell(worldUu, _L2TerrainIntensityOrigin2, gx, gy, grid);
        if (L2_TerrainIntensityInside(gx, gy, grid))
        {
            float2 uv = (float2(gx, gy) + 0.5) / grid;
            return SAMPLE_TEXTURE2D_LOD(_L2TerrainIntensityMap2, sampler_PointClamp, uv, 0);
        }
    }

    L2_TerrainIntensityCell(worldUu, _L2TerrainIntensityOrigin, gx, gy, grid);
    float2 fallbackUv = (float2(gx, gy) + 0.5) / grid;
    return SAMPLE_TEXTURE2D_LOD(_L2TerrainIntensityMap, sampler_PointClamp, fallbackUv, 0);
}

float L2_TerrainIntensityAt(float2 worldUu)
{
    return L2_TerrainIntensitySample(worldUu).r;
}

float2 L2_TerrainIntensityUv(float2 worldUu)
{
    float gx;
    float gy;
    float grid;
    L2_TerrainIntensityCell(worldUu, _L2TerrainIntensityOrigin, gx, gy, grid);
    return (float2(gx, gy) + 0.5) / grid;
}

// Blue: cell has no capture, brightness is the LUT fill.
// Dark red: cell was captured and its intensity byte is 0.
// Green: cell was captured with a nonzero byte. Brighter means a larger byte.
float3 L2_TerrainLightDebugColor(float2 worldUu)
{
    float4 sampleUv = L2_TerrainIntensitySample(worldUu);
    if (sampleUv.g < 0.5)
        return float3(0.12, 0.35, 0.95);
    if (sampleUv.r < (0.5 / 255.0))
        return float3(0.55, 0.04, 0.04);
    return float3(0.05, saturate(sampleUv.r * 2.0), 0.06);
}

// worldUu is official UU on Unity XZ (already * 52.5): .x = Unity X = UE Y, .y = Unity Z = UE X.
// heightUu is Unity Y * 52.5 = UE Z. repeatUv.xy is layer Scale * UScale/VScale (UU per repeat).
// A zero repeat is an authoring error logged when the material is built. The 1e-4 floor
// only keeps the division finite; it is not a 128 UU stand-in.
// texRotUu is TextureRotation (65536 = 360°). axis is TEXMAPAXIS_XY/XZ/YZ (0/1/2).
// Keep this in lockstep with L2TerrainLayerUv.Sample.
float2 L2_TerrainLayerUv(float2 worldUu, float heightUu, float4 repeatUv, float texRotUu, float axis)
{
    float2 s = max(repeatUv.xy, float2(1e-4, 1e-4));
    float2 plane;
    if (axis >= 1.5)
        plane = float2(worldUu.x, heightUu);
    else if (axis >= 0.5)
        plane = float2(worldUu.y, heightUu);
    else
        plane = worldUu.yx;
    float2 uv = plane / s;
    uv += repeatUv.zw;
    if (abs(texRotUu) > 0.5)
    {
        float a = texRotUu * (6.28318530718 / 65536.0);
        float c = cos(a);
        float srot = sin(a);
        uv = float2(uv.x * c - uv.y * srot, uv.x * srot + uv.y * c);
    }
    return uv;
}

float2 L2_TerrainSplatUv(float2 meshUv)
{
    float2 uv = meshUv - 0.5;
    uv = float2(uv.y, -uv.x);
    uv += 0.5;
    return (uv * 254.0 + 0.5) / 256.0;
}

#endif
