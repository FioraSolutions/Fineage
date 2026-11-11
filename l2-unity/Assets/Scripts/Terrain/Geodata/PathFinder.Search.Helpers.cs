using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PathFinder
{
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
