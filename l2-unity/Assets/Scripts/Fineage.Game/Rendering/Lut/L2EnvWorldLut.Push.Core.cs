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
}
