using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class L2EnvWorldLut
{
    static string _distanceFogLog;

    public static void PushDistanceFog(float worldHours)
    {
        L2HazeLut.EnsureLoaded();
        Color haze = L2HazeLut.SampleAt(worldHours);
        float uuToUnity = VectorUtils.L2UuToUnity;
        if (string.IsNullOrEmpty(L2ZoneRender.ActiveMap))
        {
            float startUu = FogEndUu - FogRangeUu;
            Shader.SetGlobalColor(FogColorId, haze);
            Shader.SetGlobalFloat(FogStartId, startUu * uuToUnity);
            Shader.SetGlobalFloat(FogEndId, FogEndUu * uuToUnity);
            Shader.SetGlobalFloat(FogScaleId, uuToUnity > 1e-8f ? (1f / FogRangeUu) / uuToUnity : 0f);
            Shader.SetGlobalFloat(FogEnableId, 1f);
            LogDistanceFog("map=(none) actor=(haze) rgb=" + Rgb(haze) +
                " startUu=" + startUu.ToString("0.###") + " endUu=" + FogEndUu.ToString("0.###"));
            return;
        }

        if (!L2ZoneRender.TryActiveFog(out L2ZoneRenderTable.DistanceFog fog) || !fog.HasEnd)
        {
            Shader.SetGlobalFloat(FogEnableId, 0f);
            LogDistanceFog("map=" + L2ZoneRender.ActiveMap + " actor=" + L2ZoneRender.ActiveZone + " fog=off");
            return;
        }

        Color color = haze;
        if (fog.HasColor)
            color = new Color(fog.R / 255f, fog.G / 255f, fog.B / 255f, 1f);
        L2ZoneRenderTable.DistanceFogGlobals(
            fog, uuToUnity, out float startMeters, out float endMeters, out float scale, out bool enable);
        Shader.SetGlobalColor(FogColorId, color);
        Shader.SetGlobalFloat(FogEnableId, enable ? 1f : 0f);
        if (!enable)
        {
            LogDistanceFog("map=" + L2ZoneRender.ActiveMap + " actor=" + fog.Actor + " fog=disabled");
            return;
        }
        Shader.SetGlobalFloat(FogStartId, startMeters);
        Shader.SetGlobalFloat(FogEndId, endMeters);
        Shader.SetGlobalFloat(FogScaleId, scale);
        LogDistanceFog("map=" + L2ZoneRender.ActiveMap + " actor=" + fog.Actor +
            " rgb=" + fog.R + "," + fog.G + "," + fog.B +
            " startUu=" + fog.StartUu.ToString("0.###") + " endUu=" + fog.EndUu.ToString("0.###") +
            " startM=" + startMeters.ToString("0.###") + " endM=" + endMeters.ToString("0.###"));
    }

    static void LogDistanceFog(string message)
    {
        if (message == _distanceFogLog)
            return;
        _distanceFogLog = message;
        FineageLog.Info(FineageLogCategory.World, "[ZoneFog] " + message);
    }

    static string Rgb(Color color)
    {
        return Mathf.RoundToInt(color.r * 255f) + "," +
            Mathf.RoundToInt(color.g * 255f) + "," +
            Mathf.RoundToInt(color.b * 255f);
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
