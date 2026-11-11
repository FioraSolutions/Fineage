using System;
using Object = UnityEngine.Object;

﻿using System.Globalization;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// L2 ANSun / ANMoon billboards: textures from High Elf RenderDoc, motion/size/color from celestial LUT.
/// Follows the camera using sweep-relative world offsets (same as EnvManager actor Location).
/// </summary>
[ExecuteInEditMode]
public partial class L2CelestialDiscs : MonoBehaviour
{
    const string SunTexPath = "Data/Clock/sun_texture_20335";
    const string MoonTexPath = "Data/Clock/moon_texture_20931";
    const string ShaderName = "L2/Sky/CelestialDisc";

    // Captured EID 1329: side 101.32 UU at moonDrawScale 4.5, clip-W ~244.
    const float MoonRefSideUu = 101.32f;
    const float MoonRefDrawScale = 4.5f;
    const float MoonRefDistanceUu = 244.33f;

    // Captured EID 1332: side 82.70 UU, clip-W ~338. DrawScale ~2 at daytime disc.
    const float SunRefSideUu = 82.70f;
    const float SunRefDrawScale = 2.0f;
    const float SunRefDistanceUu = 338.11f;

    const float VisibleScaleEpsilon = 0.02f;


    [SerializeField] Texture2D _sunTexture;
    [SerializeField] Texture2D _moonTexture;
    [SerializeField] bool _followCamera = true;
    [Tooltip("0 = just inside camera far clip so terrain/hills can occlude the disc. >80 overrides.")]
    [SerializeField] float _forceDistanceMeters = 0f;
    [SerializeField] bool _logTrajectory = true;
    [SerializeField] float _logInterval = 0.25f;
    [SerializeField] float _jumpAngleDeg = 3f;

    struct BodyTrack
    {
        public bool hasLast;
        public bool lastVisible;
        public bool lastFallback;
        public Vector3 lastPlaceDir;
        public Vector3 lastPos;
        public float lastHours;
    }
}
