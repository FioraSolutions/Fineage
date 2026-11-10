using System;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

public partial class Geodata : MonoBehaviour {
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

    private void Awake() {
        if(_instance == null) {
            _instance = this;
            GeodataPacketServices.Bind(geodata: this);
        } else {
            Destroy(this);
        }
    }

    void OnDestroy() {
        if (_instance != this)
            return; // duplicate destroyed in Awake
        _geodata?.Clear();
        _instance = null;
    }

    void Start() {
        if (_geodata == null) {
            _geodata = new Dictionary<string, Node[,,]>();
        }
    }

    public void LoadMaps(IList<string> mapIds) {
        if (_geodata == null) {
            _geodata = new Dictionary<string, Node[,,]>();
        }

        _mapsToLoad = mapIds != null ? new List<string>(mapIds) : new List<string>();
        _loaded = _mapsToLoad.Count == 0;

        for (int i = 0; i < _mapsToLoad.Count; i++) {
            string mapId = _mapsToLoad[i];
            if (_mapsOrigin.ContainsKey(mapId)) {
                continue;
            }

            Vector3 origin;
            try {
                origin = GeodataImporter.LoadMapOrigin(mapId, _nodeSize);
                _mapsOrigin.Add(mapId, origin);
            } catch (Exception) {
                // Debug.LogError(e.Message);
                continue;
            }

            GeodataPacketServices.Factory.RequestImportGeodata(mapId, _nodeSize, (byte) _maximumLayerCount, origin, (callback) => {
                if(callback != null) {
                    _geodata[mapId] = callback;
                }

                if(_geodata.Keys.Count >= _mapsToLoad.Count) {
                    _loaded = true;
                }
            });
        }
    }


    public string GetCurrentMap(Vector3 location) {
        if (TryGetCurrentMap(location, out string mapId))
            return mapId;
        throw new Exception("Outside bounds.");
    }

    /// <summary>Non-throwing map resolve for hot paths (#930 WorldPlacement).</summary>
    public bool TryGetCurrentMap(Vector3 location, out string mapId)
    {
        mapId = null;
        if (_mapsOrigin == null)
            return false;

        foreach (var mapName in _mapsOrigin.Keys)
        {
            if (IsInMapBounds(location, _mapsOrigin[mapName]))
            {
                mapId = mapName;
                return true;
            }
        }

        return false;
    }

    private bool IsInMapBounds(Vector3 location, Vector3 mapOrigin) {
        if(location.x < mapOrigin.x) {
            return false;
        }
        if(location.x > mapOrigin.x + _mapSize) {
            return false;
        }
        if(location.z < mapOrigin.z) {
            return false;
        }
        if(location.z > mapOrigin.z + _mapSize) {
            return false;
        }

        return true;
    }

    public Node GetNodeAt(Vector3 nodePos, string mapId) {
        return GetNodeAt(nodePos, mapId, false);
    }

    public Node GetNodeAt(Vector3 nodePos) {
        return GetNodeAt(nodePos, GetCurrentMap(nodePos), false);
    }

    public Node GetNodeAt(Vector3 nodePos, bool exactElevation) {
        return GetNodeAt(nodePos, GetCurrentMap(nodePos), exactElevation);
    }

    // Get node at a given world position
    public Node GetNodeAt(Vector3 nodePos, string mapId, bool exactElevation) {
        if (TryGetNodeAt(nodePos, mapId, out Node node, exactElevation))
            return node;
        throw new Exception("Node not found");
    }

    /// <summary>Non-throwing node lookup (#930). Prefer closest layer in the elevation window.</summary>
    public bool TryGetNodeAt(Vector3 nodePos, out Node node, bool exactElevation = false)
    {
        node = null;
        if (!TryGetCurrentMap(nodePos, out string mapId))
            return false;
        return TryGetNodeAt(nodePos, mapId, out node, exactElevation);
    }

    public bool TryGetNodeAt(Vector3 nodePos, string mapId, out Node node, bool exactElevation = false)
    {
        node = null;
        if (_geodata == null || string.IsNullOrEmpty(mapId) || !_geodata.ContainsKey(mapId))
            return false;

        Vector3 nodeIndex;
        try
        {
            nodeIndex = FromWorldToNodePos(nodePos, mapId);
        }
        catch
        {
            return false;
        }

        int x = (int)nodeIndex.x;
        int z = (int)nodeIndex.z;
        Node[,,] map = _geodata[mapId];
        if (x < 0 || z < 0 || x >= map.GetLength(0) || z >= map.GetLength(2))
            return false;

        float maxErr = exactElevation ? 0.01f : _maximumElevationError;
        Node best = null;
        float bestOffset = float.PositiveInfinity;
        for (int y = 0; y < map.GetLength(1); y++)
        {
            Node layer = map[x, y, z];
            if (layer == null)
                continue;

            float layerOffset = Math.Abs(layer.nodeIndex.y - nodeIndex.y);
            if (layerOffset > maxErr)
                continue;

            // Closest layer in window; on ties prefer higher Y (multi-deck villages).
            if (best == null ||
                layerOffset < bestOffset - 0.0001f ||
                (Math.Abs(layerOffset - bestOffset) <= 0.0001f && layer.center.y > best.center.y))
            {
                best = layer;
                bestOffset = layerOffset;
            }
        }

        if (best == null)
            return false;

        node = new Node(best);
        return true;
    }

}
