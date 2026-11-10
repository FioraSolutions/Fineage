using AtmosphericHeightFog;
using System;
using UnityEngine;

public partial class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Material _skyboxMaterial;

    private Light _mainLight;
    private WorldClock _clock;

    [Header("Sun/Moon appearances")]
    [SerializeField] private float _horizonOffsetDegree = 5f;
    [SerializeField] private float _mainLightRotY = 45f;
    [SerializeField] private Texture2D _sunTexture;
    [SerializeField] private Vector2 _sunTiling = new Vector2(1.5f, 1.5f);
    [SerializeField] private Texture2D _moonTexture;
    [SerializeField] private Vector2 _moonTiling = new Vector2(2.5f, 2.5f);

    [Header("Sky colors")]
    [SerializeField] private Color _dayColor = new Color(0f / 255f, 90f / 255f, 140f / 255f); // peak at sunriseEndTime
    [SerializeField] private Color _dawnColor = new Color(19f / 255f, 35f / 255f, 55f / 255f); // peak at sunriseStartTime  
    [SerializeField] private Color _duskColor = new Color(70f / 255f, 53f / 255f, 43f / 255f); // peak at sunsetStartTime
    [SerializeField] private Color _nightColor = new Color(1f / 255f, 1f / 255f, 2f / 255f) * -1f; // peak at sunsetEndTime

    [Header("Fog colors")]
    [SerializeField] private Color _dayFogColorStart = new Color(126f / 255f, 190f / 255f, 255f / 255f);
    [SerializeField] private Color _dayFogColorEnd = new Color(115f / 255f, 153f / 255f, 191f / 255f) * 0.7388527f;
    [SerializeField] private Color _nightFogColorStart = new Color(112f / 255f, 117f / 255f, 123f / 255f);
    [SerializeField] private Color _nightFogColorEnd = new Color(112f / 255f, 117f / 255f, 123f / 255f) * 0.7388527f;
    [SerializeField] private float _dayDirectionalIntensity = 0.250f;
    [SerializeField] private float _nightDirectionalIntensity = 0;

    [Header("Main light colors")]
    [SerializeField] private Color _mainLightDayColor = new Color(255f / 255f, 249f / 255f, 225f / 255f);
    [SerializeField] private Color _mainLightNightColor = new Color(101f / 255f, 110f / 255f, 152f / 255f);
    [SerializeField] private Color _mainLightduskColor = new Color(255f / 255f, 206f / 255f, 158f / 255f);
    [SerializeField] private Color _mainLightDawnColor = new Color(255f / 255f, 206f / 255f, 158f / 255f);
    [Header("Ambient light colors")]
    [SerializeField] private Color _ambientLightDayColor = new Color(166f / 255f, 156f / 255f, 135f / 255f);
    [SerializeField] private Color _ambientLightNightColor = new Color(42f / 255f, 42f / 255f, 40f / 255f);
    [SerializeField] private Color _ambientLightduskColor = new Color(82f / 255f, 65f / 255f, 41f / 255f);
    [SerializeField] private Color _ambientLightDawnColor = new Color(96f / 255f, 96f / 255f, 79f / 255f);

    [Header("Skybox cloud leftover (always off — L2 clouds are separate components)")]
    [SerializeField] private float _dayCloudsOpcacity = 0f;
    [SerializeField] private float _nightCloudsOpacity = 0f;
    [SerializeField] private float _dayHorizonCloudsOpcacity = 0f;
    [SerializeField] private float _nightHorizonCloudsOpacity = 0f;

    [Header("Ambient light intensity")]
    [SerializeField] private float _ambientMinIntensity = 0.2f;
    [SerializeField] private float _ambientMaxIntensity = 0.5f;

    [Header("Main light intensity")]
    [SerializeField] private float _mainLightMinIntensity = 0.4f;
    [SerializeField] private float _mainLightMaxIntensity = 1f;

    [Header("L2 High Elf color LUT")]
    [SerializeField] private bool _useL2ColorLut = true;

    [Header("L2 High Elf sun/moon discs")]
    [SerializeField] private bool _useL2Celestial = true;

    [Header("L2 High Elf haze ring")]
    [SerializeField] private bool _useL2HazeRing = true;

    [Header("L2 High Elf star field")]
    [SerializeField] private bool _useL2StarDome = true;

    [Header("L2 High Elf simple clouds")]
    [SerializeField] private bool _useL2CloudSimple = true;

    // EID 1566 myst strip: in the original capture it sits on top of simple
    // clouds as noise. Current port draws a solid colored card and the
    // intended effect is not visible — keep off until the combiner/placement
    // matches L2.
    [Header("L2 High Elf myst clouds (off — result not visible yet)")]
    [SerializeField] private bool _useL2CloudMyst = false;

    L2CelestialDiscs _celestialDiscs;
    L2HazeRing _hazeRing;
    L2StarDome _starDome;
    L2CloudSimple _cloudSimple;
    L2CloudMyst _cloudMyst;
    bool _skyReady;
    float _appliedHours;

    void OnEnable()
    {
        if (_useL2ColorLut)
        {
            L2DayNightLut.EnsureLoaded();
            L2EnvWorldLut.EnsureLoaded();
            L2ActorLightLut.EnsureLoaded();
        }

        if (_useL2Celestial)
        {
            L2CelestialLut.EnsureLoaded();
            EnsureCelestialDiscs();
        }

        if (_useL2HazeRing)
        {
            L2HazeLut.EnsureLoaded();
            EnsureHazeRing();
        }

        if (_useL2StarDome)
        {
            EnsureStarDome();
        }

        StripLegacyCombinedClouds();

        if (_useL2CloudSimple)
        {
            EnsureCloudSimple();
        }

        if (_useL2CloudMyst && L2CloudMyst.PortEnabled)
        {
            EnsureCloudMyst();
        }
        else
        {
            DisableCloudMyst();
        }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (_clock == null)
            {
                _clock = WorldClock.EnsurePersistent();
            }
            if (_clock == null)
            {
                _clock = GetComponent<WorldClock>();
            }
            if (_clock == null)
            {
                _clock = WorldClockPacketServices.Clock;
            }
            if (_mainLight == null)
            {
                _mainLight = GetComponent<Light>();
            }
            if (_clock == null || _mainLight == null)
            {
                return;
            }

            float hours = _clock.WorldHours;
            if (_skyReady && Mathf.Abs(hours - _appliedHours) < 0.0001f)
            {
                L2ActorPointLights.Push();
                return;
            }

            if (_useL2Celestial)
            {
                L2CelestialLut.EnsureLoaded();
                EnsureCelestialDiscs();
            }

            if (_useL2HazeRing)
            {
                L2HazeLut.EnsureLoaded();
                EnsureHazeRing();
            }

            if (_useL2StarDome)
            {
                EnsureStarDome();
            }

            if (_useL2CloudSimple)
            {
                EnsureCloudSimple();
            }

            if (_useL2CloudMyst && L2CloudMyst.PortEnabled)
            {
                EnsureCloudMyst();
            }
            else
            {
                DisableCloudMyst();
            }

            bool usedL2LightDir = false;
            if (_useL2Celestial && L2CelestialLut.IsReady)
            {
                usedL2LightDir = ApplyL2CelestialLight();
            }

            if (!usedL2LightDir)
            {
                float mainLightLerpValue = _clock.Clock.dayRatio > 0 ? _clock.Clock.dayRatio : _clock.Clock.nightRatio;
                float sunRotation = Mathf.Lerp(0 - _horizonOffsetDegree, 180 + _horizonOffsetDegree, mainLightLerpValue);
                transform.eulerAngles = new Vector3(sunRotation, _mainLightRotY, 0);

                if (transform.eulerAngles.x < -1 || transform.eulerAngles.x > 181)
                {
                    _mainLight.intensity = 0;
                }
                else
                {
                    UpdateMainLightIntensity();
                }
            }

            ShareMainLightRotation();

            UpdateAmbientLightIntensity();

            if (_useL2ColorLut)
            {
                L2DayNightLut.EnsureLoaded();
                L2EnvWorldLut.EnsureLoaded();
                L2ActorLightLut.EnsureLoaded();
            }

            if (_useL2ColorLut && L2DayNightLut.IsReady)
            {
                ApplyL2ColorLut();
            }
            else
            {
                UpdateSkyColor();
                UpdateFogColor();
                UpdateAmbientLightColor();
                UpdateLightColor();
            }

            UpdateCloudsOpacity();
            if (_useL2Celestial)
            {
                HideSkyboxSunMoon();
            }
            else if (_skyboxMaterial != null)
            {
                UpdateMainLightTexture();
            }

            _appliedHours = _clock.WorldHours;
            _skyReady = true;
        }
        catch (Exception)
        {
            // Debug.LogException(e);
        }

    }

}
