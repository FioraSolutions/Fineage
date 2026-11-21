using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Applies <see cref="LobbyAccountEnvironmentProfile"/> while on account lobby cameras.
/// </summary>
public static class LobbyAccountRenderSettings
{
    static bool _applied;
    static bool _fogWas;
    static Color _fogColorWas;
    static FogMode _fogModeWas;
    static float _fogStartWas;
    static float _fogEndWas;
    static float _fogDensityWas;
    static Color _ambientSkyWas;
    static Color _ambientEquatorWas;
    static Color _ambientGroundWas;
    static float _ambientIntensityWas;
    static AmbientMode _ambientModeWas;
    static Material _skyWas;
    static float _reflectionIntensityWas;

    public static bool IsApplied => _applied;

    public static LobbyAccountEnvironmentProfile LoadProfile()
    {
        var profile = Resources.Load<LobbyAccountEnvironmentProfile>(
            LobbyAccountEnvironmentProfile.DefaultResourcePath);
        if (profile == null)
        {
            Debug.LogError(
                "[LobbyAccountRenderSettings] missing Resources/"
                + LobbyAccountEnvironmentProfile.DefaultResourcePath
                + " (LobbyAccountEnvironmentProfile)");
        }
        return profile;
    }

    public static void Apply()
    {
        LobbyAccountEnvironmentProfile profile = LoadProfile();
        if (profile == null)
            return;

        if (!_applied)
            Remember();

        RenderSettings.fog = profile.fogEnabled;
        RenderSettings.fogColor = profile.fogColor;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = profile.fogStartDistance;
        RenderSettings.fogEndDistance = profile.fogEndDistance;

        Material sky = null;
        if (!string.IsNullOrEmpty(profile.skyboxResourcePath))
            sky = Resources.Load<Material>(profile.skyboxResourcePath);
        if (sky == null && !string.IsNullOrEmpty(profile.skyboxResourcePath))
        {
            Debug.LogError(
                "[LobbyAccountRenderSettings] missing skybox material Resources/"
                + profile.skyboxResourcePath);
        }

        RenderSettings.skybox = sky;
        RenderSettings.reflectionIntensity = profile.reflectionIntensity;
        RenderSettings.ambientMode = profile.ambientMode;
        RenderSettings.ambientIntensity = profile.ambientIntensity;
        RenderSettings.ambientSkyColor = profile.ambientSky;
        RenderSettings.ambientEquatorColor = profile.ambientEquator;
        RenderSettings.ambientGroundColor = profile.ambientGround;
        ApplySkyPresentation(profile, sky);
        PushLobbyActorLighting(profile);
        _applied = true;
    }

    static void ApplySkyPresentation(LobbyAccountEnvironmentProfile profile, Material sky)
    {
        if (sky == null)
            return;

        if (sky.HasProperty("_Exposure"))
            sky.SetFloat("_Exposure", profile.skyExposure);

        L2DayNightLut.EnsureLoaded();
        if (!L2DayNightLut.IsReady || !sky.HasProperty("_GradientColor1"))
            return;

        L2DayNightLut.Sample day = L2DayNightLut.SampleAt(profile.actorWorldHours);
        Color displaySky = L2DayNightLut.ApplyPostSky(day.sky, profile.actorWorldHours);
        sky.SetColor("_GradientColor1", displaySky);
    }

    static void PushLobbyActorLighting(LobbyAccountEnvironmentProfile profile)
    {
        L2EnvWorldLut.EnsureLoaded();
        L2ActorLightLut.EnsureLoaded();
        L2DayNightLut.EnsureLoaded();

        L2DayNightLut.Sample day = L2DayNightLut.SampleAt(profile.actorWorldHours);
        L2EnvWorldLut.PushGlobals(profile.actorWorldHours, day.actorAmbient, day.sun);
        L2ActorLightLut.PushGlobals(profile.actorWorldHours);
        L2ActorPointLights.Push();
    }

    public static void Release()
    {
        if (!_applied)
            return;
        RenderSettings.fog = _fogWas;
        RenderSettings.fogColor = _fogColorWas;
        RenderSettings.fogMode = _fogModeWas;
        RenderSettings.fogStartDistance = _fogStartWas;
        RenderSettings.fogEndDistance = _fogEndWas;
        RenderSettings.fogDensity = _fogDensityWas;
        RenderSettings.ambientMode = _ambientModeWas;
        RenderSettings.ambientIntensity = _ambientIntensityWas;
        RenderSettings.ambientSkyColor = _ambientSkyWas;
        RenderSettings.ambientEquatorColor = _ambientEquatorWas;
        RenderSettings.ambientGroundColor = _ambientGroundWas;
        RenderSettings.reflectionIntensity = _reflectionIntensityWas;
        RenderSettings.skybox = _skyWas;
        _applied = false;
    }

    static void Remember()
    {
        _fogWas = RenderSettings.fog;
        _fogColorWas = RenderSettings.fogColor;
        _fogModeWas = RenderSettings.fogMode;
        _fogStartWas = RenderSettings.fogStartDistance;
        _fogEndWas = RenderSettings.fogEndDistance;
        _fogDensityWas = RenderSettings.fogDensity;
        _ambientSkyWas = RenderSettings.ambientSkyColor;
        _ambientEquatorWas = RenderSettings.ambientEquatorColor;
        _ambientGroundWas = RenderSettings.ambientGroundColor;
        _ambientIntensityWas = RenderSettings.ambientIntensity;
        _ambientModeWas = RenderSettings.ambientMode;
        _skyWas = RenderSettings.skybox;
        _reflectionIntensityWas = RenderSettings.reflectionIntensity;
    }
}