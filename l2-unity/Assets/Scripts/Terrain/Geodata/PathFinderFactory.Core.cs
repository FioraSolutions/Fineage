using System;

//This partial class controls the threads
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
