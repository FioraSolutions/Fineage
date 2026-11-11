using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class L2EnvWorldLut
{
    public static void PushDistanceFog(float worldHours)
    {
        L2HazeLut.EnsureLoaded();
        Shader.SetGlobalColor(FogColorId, L2HazeLut.SampleAt(worldHours));
        float uuToUnity = VectorUtils.L2UuToUnity;
        Shader.SetGlobalFloat(FogEndId, FogEndUu * uuToUnity);
        Shader.SetGlobalFloat(FogScaleId, uuToUnity > 1e-8f ? (1f / FogRangeUu) / uuToUnity : 0f);
        Shader.SetGlobalFloat(FogEnableId, 1f);
    }

    public static void PushGlobals(float worldHours, Color actorAmbient, Color sunColor)
    {
        PushDistanceFog(worldHours);
        EnsureLoaded();
        if (!IsReady)
            return;

        Sample sample = SampleAt(worldHours);
        Shader.SetGlobalColor(TerrainAmbientId, sample.terrainAmbient);
        Shader.SetGlobalColor(StaticMeshAmbientId, sample.staticMeshAmbient);
        Shader.SetGlobalColor(BspAmbientId, sample.bspAmbient);
        Shader.SetGlobalColor(ActorAmbientId, actorAmbient);
        Shader.SetGlobalColor(HsvTerrainLightId, sample.hsvTerrainLight);
        Shader.SetGlobalColor(HsvStaticMeshLightId, sample.hsvStaticMeshLight);

        // ActorStaticLight slots are centred at 01:30, 04:30, ... 22:30.
        float maskPosition = Repeat24(worldHours) / 3f - 0.5f;
        int slot0Unwrapped = Mathf.FloorToInt(maskPosition);
        int slot0 = ((slot0Unwrapped % L2StaticMeshSunVisibility.SlotCount) +
            L2StaticMeshSunVisibility.SlotCount) % L2StaticMeshSunVisibility.SlotCount;
        int slot1 = (slot0 + 1) % L2StaticMeshSunVisibility.SlotCount;
        Shader.SetGlobalFloat(StaticMeshMaskSlot0Id, slot0);
        Shader.SetGlobalFloat(StaticMeshMaskSlot1Id, slot1);
        Shader.SetGlobalFloat(StaticMeshMaskBlendId, maskPosition - Mathf.Floor(maskPosition));

        L2CelestialLut.EnsureLoaded();
        Vector3 sunDirection = L2CelestialLut.SampleAt(worldHours).sunDirUnity;
        Shader.SetGlobalVector(
            StaticMeshLightDirectionId,
            new Vector4(-sunDirection.x, -sunDirection.y, -sunDirection.z, 0f));

        Shader.SetGlobalFloat(TerrainColor0ScaleId, 1f);
        float intensity01 = TerrainIntensity.Count >= 2
            ? SampleFloat(TerrainIntensity, Repeat24(worldHours))
            : DefaultTerrainIntensity01;
        Shader.SetGlobalFloat(TerrainIntensityId, intensity01);
        PushIntensityGrid(Repeat24(worldHours), intensity01);

        Color gi = sample.terrainAmbient;
        _ = sunColor;

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientSkyColor = gi;
        RenderSettings.ambientEquatorColor = gi;
        RenderSettings.ambientGroundColor = gi;
        RenderSettings.ambientIntensity = 1f;
    }

    public static void ShowUnityTreePreview(byte[] intensity)
    {
        int plane = IntensityGridSize * IntensityGridSize;
        if (intensity == null || intensity.Length != plane)
            return;
        _unityTreePreview = intensity;
        _unityTreePreviewUploaded = false;
        _openIntensityPushed = false;
        UploadUnityTreePreview();
    }

    static string UnityTreePreviewPath()
    {
        return Path.GetFullPath(Path.Combine(Application.dataPath, "..", "BakeOutput", "L2TerrainIntensityGrid_unitytree_preview.bin"));
    }

    static void EnsureUnityTreePreview()
    {
        if (_unityTreePreview != null)
            return;
        string path = UnityTreePreviewPath();
        if (!File.Exists(path))
            return;
        byte[] file = File.ReadAllBytes(path);
        if (file.Length != IntensityGridSize * IntensityGridSize)
            return;
        _unityTreePreview = file;
    }

    static void UploadUnityTreePreview()
    {
        EnsureUnityTreePreview();
        if (_unityTreePreview == null)
            return;

        int n = IntensityGridSize;
        var pixels = new byte[n * n * 4];
        for (int i = 0; i < n * n; i++)
        {
            pixels[i * 4] = _unityTreePreview[i];
            pixels[i * 4 + 1] = 255;
            pixels[i * 4 + 3] = 255;
        }

        if (_intensityTex == null || _intensityTex.width != n || _intensityTex.height != n)
        {
            if (_intensityTex != null)
                UnityEngine.Object.DestroyImmediate(_intensityTex);
            _intensityTex = new Texture2D(n, n, TextureFormat.RGBA32, false, true)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
                name = "L2TerrainIntensityUnityTree",
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        _intensityTex.SetPixelData(pixels, 0);
        _intensityTex.Apply(false, false);
        Shader.SetGlobalTexture(TerrainIntensityMapId, _intensityTex);
        Shader.SetGlobalVector(TerrainIntensityOriginId, IntensityOrigin);
        Shader.SetGlobalVector(TerrainIntensityOrigin1Id, Vector4.zero);
        Shader.SetGlobalVector(TerrainIntensityOrigin2Id, Vector4.zero);
        Shader.SetGlobalFloat(TerrainLightDebugId, 0f);
        _unityTreePreviewUploaded = true;
        _openIntensityPushed = true;
    }

    static void PushOpenIntensity()
    {
        Shader.SetGlobalFloat(TerrainLightDebugId, 0f);
        if (_openIntensityPushed && _intensityTex != null)
            return;
        _openIntensityPushed = true;
        if (_intensityTex == null)
        {
            _intensityTex = new Texture2D(1, 1, TextureFormat.RGBA32, false, true)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Point,
                name = "L2TerrainIntensityOpen"
            };
            _intensityTex.SetPixel(0, 0, Color.white);
            _intensityTex.Apply(false, false);
        }

        Shader.SetGlobalTexture(TerrainIntensityMapId, _intensityTex);
        Shader.SetGlobalVector(TerrainIntensityOriginId, IntensityOrigin);
        Shader.SetGlobalVector(TerrainIntensityOrigin1Id, Vector4.zero);
        Shader.SetGlobalVector(TerrainIntensityOrigin2Id, Vector4.zero);
    }
}
