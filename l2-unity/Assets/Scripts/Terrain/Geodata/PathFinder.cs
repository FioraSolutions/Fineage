using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PathFinder
{
    private PathFinderFactory.PathfindingJobComplete _completeCallback;
    private List<Node> _foundPath;
    private float _nodeSize;

    private Node _startPosition;
    private Node _endPosition;
    private int _maximumLoopCount;
    private volatile bool jobDone = false;

    public bool JobDone { get { return jobDone; } }

}
