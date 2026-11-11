using System;

﻿using System.Collections.Generic;
using UnityEngine;
using VContainer;

/// <summary>
/// Runtime pool for effect hit flashes (L2 FNPawnLight-style).
/// Visual params come from LightEffectSetting when provided.
/// </summary>
public partial class FNManagerLight : MonoBehaviour
{
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static FNManagerLight Instance { get; private set; }
    private const string LOG_TAG = "[FN_LIGHT]";

    [Header("Fallback defaults (used when settings == null)")]
    [SerializeField] private Color _color = Color.white;
    [SerializeField] private float _intensity = 0.6f;
    [SerializeField] private float _durationSeconds = 0.4f;
    [SerializeField] private float _rangeMeters = 2f;
    [SerializeField] private float _spotAngle = 71.5f;
    [SerializeField] private float _innerSpotAngle = 32.5f;
    [SerializeField] private int _poolSize = 8;
    [SerializeField] private bool _debugLog = true;

    private readonly List<FlashSlot> _slots = new List<FlashSlot>();
    private Transform _poolRoot;
    private int _spawnCounter;

    private sealed class FlashSlot
    {
        public GameObject Go;
        public Light Light;
        public float Age;
        public float Duration;
        public float PeakIntensity;
        public bool Active;
        public int SpawnId;
    }

}
