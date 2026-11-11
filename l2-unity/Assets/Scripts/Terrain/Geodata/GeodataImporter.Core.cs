using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

public partial class GeodataImporter
{
    public GeodataImporter(string mapId, float nodeSize, byte maximumLayerCount, Vector3 origin, GeodataImporterFactory.ImportJobComplete callback) {
        _startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        _mapId = mapId;
        _nodeSize = nodeSize;
        _completeCallback = callback;
        _origin = origin;
        _maximumLayerCount = maximumLayerCount;
    }

    public void ImportGeodata() {
        try {
            byte[] data = GetGeodataBinaryForMap(_mapId);
            if (data != null)
                _mapNodes = LoadMapGeodata(data, _origin, _nodeSize);
        } catch (Exception) {
            // Missing zip / corrupt map — leave _mapNodes null; WorldPlacement falls back to mesh.
        }

        _jobDone = true;
    }

    public void NotifyComplete() {
        if(_completeCallback != null) {
            _completeCallback(_mapNodes);
        }
    }
}
