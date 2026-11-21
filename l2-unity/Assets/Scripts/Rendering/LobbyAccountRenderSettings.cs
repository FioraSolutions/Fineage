using UnityEngine;

/// <summary>
/// Shared render settings for account lobby (character select / create). Matches Interlude l2_lobby ambient.
/// </summary>
public static class LobbyAccountRenderSettings
{
    static readonly Color Ambient = new Color(0.38f, 0.46f, 0.48f);
    static bool _applied;
    static bool _fogWas;
    static Color _fogColorWas;
    static FogMode _fogModeWas;
    static float _fogStartWas;
    static float _fogEndWas;
    static float _fogDensityWas;
    static Color _ambientWas;
    static AmbientMode _ambientModeWas;
    static Material _skyWas;

    public static bool IsApplied => _applied;

    public static void Apply()
    {
        if (!_applied)
            Remember();

        RenderSettings.fog = false;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = Ambient;
        _applied = true;
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
        RenderSettings.ambientLight = _ambientWas;
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
        _ambientWas = RenderSettings.ambientLight;
        _ambientModeWas = RenderSettings.ambientMode;
        _skyWas = RenderSettings.skybox;
    }
}