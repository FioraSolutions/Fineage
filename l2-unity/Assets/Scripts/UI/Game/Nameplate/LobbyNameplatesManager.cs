using System;

﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

/// <summary>
/// Lobby names — same L2 canvas path as <see cref="NameplatesManager"/>:
/// Project → screen-pixel glyphs → <see cref="L2NameplateScreenBatch"/> (one draw).
/// </summary>
public partial class LobbyNameplatesManager : MonoBehaviour
{
    private const int MaxSlots = 8;

    [SerializeField] private Camera _camera;
    [SerializeField] private float _nameplateViewDistance = 80f;
    [SerializeField] private Color _defaultNameColor = Color.white;
    [Tooltip("Glyph pixel scale. Tune later to match L2; 1 = native atlas pixels.")]
    [SerializeField] private float _pixelScale = 1f;
    [Tooltip("L2 canvas: lock plate to whole screen pixels (hysteresis).")]
    [SerializeField] private bool _snapAnchorToPixels = true;
    [SerializeField] private float _snapHysteresisPx = 0.75f;
    [SerializeField] private bool _snapDiscardSubpixelFrac = true;
    [SerializeField] private string _atlasResourcePath = "Data/UI/Font/L2Lobby/l2_lobby_font_atlas";
    [SerializeField] private string _csvResourcePath = "Data/UI/Font/L2Lobby/ul2font_ascii";

    private readonly List<PaintItem> _paintList = new List<PaintItem>(MaxSlots);
    private readonly NameplatePixelSnap _pixelSnap = new NameplatePixelSnap(MaxSlots);
    private readonly L2NameplateScreenBatch _batch = new L2NameplateScreenBatch();

    private L2BitmapFont _font;
    private bool _loggedReady;
    private bool _subscribed;

    private static LobbyNameplatesManager _instance;

    [System.Obsolete("Use NameplatePacketServices.LobbyNameplates or [Inject] LobbyNameplatesManager instead")]
    public static LobbyNameplatesManager Instance => _instance;

    public Camera Camera
    {
        get => _camera;
        set => _camera = value;
    }

    private struct PaintItem
    {
        public int Slot;
        public Vector3 World;
        public string Name;
        public Color Color;
    }

}
