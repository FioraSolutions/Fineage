using System;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

public partial class Geodata : MonoBehaviour
{
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

    public Node GetNodeAt(Vector3 nodePos, string mapId, bool exactElevation) {
        if (TryGetNodeAt(nodePos, mapId, out Node node, exactElevation))
            return node;
        throw new Exception("Node not found");
    }

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
