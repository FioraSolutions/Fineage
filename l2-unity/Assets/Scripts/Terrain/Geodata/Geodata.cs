using System;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;
using VContainer;

public partial class Geodata : MonoBehaviour
{
    [SerializeField] private float _nodeSize = 0.5f;
    [SerializeField] private float _mapSize = 624.1524f;
    [SerializeField] private int _maximumLayerCount = 4;
    [SerializeField] private float _maximumElevationError = 5f;
    [SerializeField] private List<string> _mapsToLoad;
    [SerializeField] private Dictionary<string, Vector3> _mapsOrigin = new Dictionary<string, Vector3>();
    [SerializeField] private Dictionary<string, Node[,,]> _geodata;
    [SerializeField] private bool _loaded = false;

    [Header("Debug")]
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField] private bool _drawMapGizmos = true;
    [SerializeField] private int _drawMapGizmosLimit = 25000;
    [SerializeField] private bool _drawGizmosAroundPlayer = true;
    [SerializeField] private int _drawGizmosAroundPlayerRadius = 10;

    private LayerMask _obstacleMask;
    public LayerMask ObstacleMask { get { return _obstacleMask; } set { _obstacleMask = value; } }

    public float NodeSize { get { return _nodeSize; } }
    public float MapSize { get { return _mapSize; } }
    public bool Loaded { get { return _loaded; } }

    private static Geodata _instance;
    [System.Obsolete("Use [Inject] or GeodataPacketServices.Geodata instead")]
    public static Geodata Instance { get { return _instance; } }


}
