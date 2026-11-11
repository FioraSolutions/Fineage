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
}
