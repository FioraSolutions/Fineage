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

    private float GetDistance(Node posA, Node posB) {
        //We find the distance between each node
        //not much to explain here

        float distX = Mathf.Abs(posA.worldPosition.x - posB.worldPosition.x);
        float distZ = Mathf.Abs(posA.worldPosition.z - posB.worldPosition.z);
        float distY = Mathf.Abs(posA.worldPosition.y - posB.worldPosition.y);

        if(distX > distZ) {
            return 14 * distZ + 10 * (distX - distZ) + 10 * distY;
        }

        return 14 * distX + 10 * (distZ - distX) + 10 * distY;
    }

    public static List<Node> SmoothPath(List<Node> path) {
        List<Node> waypoints = new List<Node>();

        int currentNode = 0;
        //waypoints.Add(path[0]);

        for(int i = 0; i < path.Count; i++) {
            Vector3 origin = path[currentNode].center;
            Vector3 destination = path[i].center;
            Geodata geodata = GeodataPacketServices.Geodata;
            Vector3 yOffset = Vector3.up * geodata.NodeSize * 1.5f;
            bool cantSeeTarget = Physics.Linecast(destination + yOffset, origin + yOffset, geodata.ObstacleMask);

            if(cantSeeTarget) {
                waypoints.Add(path[i - 1]);
                currentNode = i - 1;
            }
        }

        waypoints.Add(path[path.Count - 1]);
        return waypoints;
    }
}
