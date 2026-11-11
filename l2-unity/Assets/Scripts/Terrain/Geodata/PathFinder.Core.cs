using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PathFinder
{
    public PathFinder(Node start, Node target, PathFinderFactory.PathfindingJobComplete callback, float nodeSize, int maximumLoopCount) {
        _startPosition = start;
        _endPosition = target;
        _completeCallback = callback;
        _nodeSize = nodeSize;
        _maximumLoopCount = maximumLoopCount;
    }

    public void FindPath() {
        try {
            _foundPath = SearchByClosest(_startPosition, _endPosition);
        } catch (Exception) {
            // Debug.LogWarning("Error while finding path: " + e.Message);
        }

        jobDone = true;
    }

    public void NotifyComplete() {
        if(_completeCallback != null) {
            _completeCallback(_foundPath);
        }
    }

    public List<Node> SearchByClosest(Node start, Node end)
    {
        // List of Visited Nodes
        List<Node> visitedNodes = new List<Node>();

        // List of Nodes to Visit
        List<Node> nodesToVisit = new List<Node>();
        nodesToVisit.Add(start);

        bool added;

        int i = 0;
        while (i++ < _maximumLoopCount) {
            Node node;

            // Get and remove node from the nodesToVisit list
            try {
                node = nodesToVisit[0];
                nodesToVisit.RemoveAt(0);
            } catch (Exception) {
                // No Path found
                // Debug.Log($"No path found - {start.center} to {end.center}.");
                return null;
            }

            // Current node is the destination node
            // Path was found
            if (node.nodeIndex.Equals(end.nodeIndex)) {
               // Debug.Log($"Found path - {start.center} to {end.center} after {i} iteration(s).");
                return ConstructPathFull(node);
            }

            // Add node to visited nodes
            visitedNodes.Add(node);

            Node[] neighbors = GetNeighbours(node.center);
            if (neighbors == null) {
                continue;
            }

            //node.setNeighbors(neighbors);

            // Iterate through node's neighbors
            foreach (Node neighbor in neighbors) {
                if (neighbor == null) {
                    continue;
                }

                // check if neighbor was visited and needs to be visited
                if (!visitedNodes.Contains(neighbor) && !nodesToVisit.Contains(neighbor)) {

                    // Calculate neighbor node cost
                    neighbor.parentNode = new Node(node);
                    neighbor.cost = Vector3.Distance(neighbor.center, end.center);


                    // insert neighbor into the nodes to visit based on its cost
                    added = false;
                    for (int index = 0; index < nodesToVisit.Count; index++) {
                        if (neighbor.cost < nodesToVisit[index].cost) {
                            nodesToVisit.Insert(index, neighbor);
                            added = true;
                            break;
                        }
                    }

                    // add neighbor at the end of the nodes to visit list
                    if (!added) {
                        nodesToVisit.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }
}
