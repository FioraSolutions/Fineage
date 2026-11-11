using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using System.Collections;
using System.Threading;

//This partial class controls the threads
public partial class PathFinderFactory : MonoBehaviour
{
    //Delegates are a variable that points to a function
    public delegate void PathfindingJobComplete(List<Node> path);

    //The maximum simultaneous threads we allow to open
    [SerializeField] private int _maxJobs = 3;
    [SerializeField] private int _maximumLoopCount = 250;

    private List<PathFinder> _currentJobs;
    private List<PathFinder> _todoJobs;
    private float _nodeSize = 0.5f;

    private static PathFinderFactory _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static PathFinderFactory Instance { get { return _instance; } }
}
