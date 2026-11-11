using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UnityEngine.Rendering;

public partial class NameplatesManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _nameplateViewDistance = 50f;
    [SerializeField] private LayerMask _entityMask;
    [SerializeField] private Color _defaultNameColor = Color.white;
    [SerializeField] private float _pixelScale = 1f;
    [Tooltip("L2 canvas: lock plate to whole screen pixels (hysteresis).")]
    [SerializeField] private bool _snapAnchorToPixels = true;
    [SerializeField] private float _snapHysteresisPx = 0.75f;
    [SerializeField] private bool _snapDiscardSubpixelFrac = true;
    [SerializeField] private bool _drawTitles = true;
    [SerializeField] private float _titleGapPixels = 2f;
    [SerializeField] private bool _drawBubbles = true;
    [SerializeField] private string _atlasResourcePath = "Data/UI/Font/L2Lobby/l2_lobby_font_atlas";
    [SerializeField] private string _csvResourcePath = "Data/UI/Font/L2Lobby/ul2font_ascii";

    [SerializeField] public Collider[] _entitiesInRange;

    private readonly List<NameplatePaintItem> _paintList = new List<NameplatePaintItem>(64);

    private NameplateBubbleResolver _bubbleResolver;
    private NameplateEntryStore _entryStore;
    private NameplatePixelSnap _pixelSnap;
    private NameplateTextDrawer _textDrawer;
    private NameplateBubbleDrawer _bubbleDrawer;
    private NameplateCastBarDrawer _castBarDrawer;

    private Transform _playerTransform;
    private bool _subscribed;

    private static NameplatesManager _instance;

    [System.Obsolete("Use NameplatePacketServices.Nameplates or [Inject] NameplatesManager instead")]
    public static NameplatesManager Instance => _instance;

    public Camera Camera
    {
        get => _camera;
        set => _camera = value;
    }

}
