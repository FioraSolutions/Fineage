using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PathFinder
{
    public List<Node> ConstructPathFull(Node node) {
        List<Node> path = new List<Node>();
        while (node.parentNode != null) {
            path.Insert(0, node);
            node = node.parentNode;
        }
        return path;
    }

    private Node[] GetNeighbours(Vector3 nodePos) {
        Node[] returnList = new Node[8];
        short i = 0;

        for(int x = -1; x <= 1; x++) {
            for(int z = -1; z <= 1; z++) {
                if(x == 0 && z == 0)
                    continue;

                try {
                    Vector3 neighborPos = new Vector3(nodePos.x + x * _nodeSize,
                            nodePos.y,
                            nodePos.z + z * _nodeSize);
                    Geodata geodata = GeodataPacketServices.Geodata;
                    string mapId = geodata.GetCurrentMap(neighborPos);

                    Node node = null;
                    try {
                        node = geodata.GetNodeAt(neighborPos, mapId);
                    } catch(Exception) { }

                    if(node == null) {
                        neighborPos.y = nodePos.y + _nodeSize;
                        try {
                            node = geodata.GetNodeAt(neighborPos, mapId);
                        } catch(Exception) { }
                    }

                    if(node == null) {
                        neighborPos.y = nodePos.y - _nodeSize;
                        try {
                            node = geodata.GetNodeAt(neighborPos, mapId);
                        } catch(Exception) { }
                    }

                    if(node != null) {
                        returnList[i++] = node;
                    }
                } catch(Exception) {
                    // Debug.Log("Node neighbor could not be found.");
                }
            }
        }

        return returnList;
    }
}
