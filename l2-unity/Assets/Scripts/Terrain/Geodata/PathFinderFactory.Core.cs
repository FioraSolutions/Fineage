using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Threading;

public partial class PathFinderFactory : MonoBehaviour
{
    private void Awake() {
        if (_instance == null) {
            _instance = this;
            _currentJobs = new List<PathFinder>();
            _todoJobs = new List<PathFinder>();
        } else {
            Destroy(this);
        }
    }

    void OnDestroy() {
        _instance = null;
    }
}
