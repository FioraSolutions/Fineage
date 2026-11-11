using System;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

public partial class Geodata : MonoBehaviour
{
    public Node GetClosestNodeAt(Vector3 nodePos) {
        if (TryGetClosestNodeAt(nodePos, out Node node))
            return node;
        throw new Exception("Node not found");
    }

    public Node GetClosestNodeAt(Vector3 nodePos, string mapId) {
        if (TryGetClosestNodeAt(nodePos, mapId, out Node node))
            return node;
        throw new Exception("Node not found");
    }

    /// <summary>Non-throwing closest layer at XZ (#930 WorldPlacement).</summary>
    public bool TryGetClosestNodeAt(Vector3 nodePos, out Node node)
    {
        node = null;
        if (!TryGetCurrentMap(nodePos, out string mapId))
            return false;
        return TryGetClosestNodeAt(nodePos, mapId, out node);
    }

    public bool TryGetClosestNodeAt(Vector3 nodePos, string mapId, out Node node)
    {
        node = null;
        if (!TryGetAllNodesAt(nodePos, mapId, out List<Node> layers) || layers.Count == 0)
            return false;

        Node closest = null;
        float lowestDiff = float.PositiveInfinity;
        for (int i = 0; i < layers.Count; i++)
        {
            Node n = layers[i];
            if (n == null)
                continue;
            float nodeHeightDiff = Math.Abs(n.center.y - nodePos.y);
            if (closest == null ||
                nodeHeightDiff < lowestDiff - 0.0001f ||
                (Math.Abs(nodeHeightDiff - lowestDiff) <= 0.0001f && n.center.y > closest.center.y))
            {
                closest = n;
                lowestDiff = nodeHeightDiff;
            }
        }

        if (closest == null)
            return false;

        node = closest;
        return true;
    }

    public List<Node> GetAllNodesAt(Vector3 nodePos) {
        if (TryGetAllNodesAt(nodePos, out List<Node> layers))
            return layers;
        throw new Exception("Nodes not found");
    }

    public List<Node> GetAllNodesAt(Vector3 nodePos, string mapId) {
        if (TryGetAllNodesAt(nodePos, mapId, out List<Node> layers))
            return layers;
        throw new Exception("Nodes not found");
    }

    public bool TryGetAllNodesAt(Vector3 nodePos, out List<Node> layers)
    {
        layers = null;
        if (!TryGetCurrentMap(nodePos, out string mapId))
            return false;
        return TryGetAllNodesAt(nodePos, mapId, out layers);
    }

    public bool TryGetAllNodesAt(Vector3 nodePos, string mapId, out List<Node> layers)
    {
        layers = null;
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

        layers = new List<Node>();
        for (int y = 0; y < map.GetLength(1); y++)
        {
            Node layer = map[x, y, z];
            if (layer != null)
                layers.Add(layer);
        }

        return layers.Count > 0;
    }

    public Vector3 FromNodeToWorldPos(Vector3 nodePos, Vector3 origin) {
        Vector3 worldPos = nodePos * _nodeSize + origin;
        worldPos = new Vector3(
            NumberUtils.FloorToNearest(worldPos.x, _nodeSize),
            NumberUtils.FloorToNearest(worldPos.y, _nodeSize),
            NumberUtils.FloorToNearest(worldPos.z, _nodeSize));
        return worldPos;
    }

    private Vector3 FromWorldToNodePos(Vector3 worldPos, String mapId) {
        Vector3 terrainPos;
        if(!_mapsOrigin.TryGetValue(mapId, out terrainPos)) {
            throw new Exception("Terrain not found.");
        }

        Vector3 offsetPos = worldPos - terrainPos;
        Vector3 nodePos = new Vector3(
            Mathf.Floor(offsetPos.x / _nodeSize),
            Mathf.Floor(offsetPos.y / _nodeSize),
            Mathf.Floor(offsetPos.z / _nodeSize));

        return nodePos;
    }
}
