using System;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

public partial class Geodata : MonoBehaviour
{
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
}
