using System;

//This partial class controls the threads
{
    void Start() {
        _currentJobs = new List<PathFinder>();
        _todoJobs = new List<PathFinder>();
        Geodata geodata = GeodataPacketServices.Geodata;
        if (geodata != null)
            _nodeSize = geodata.NodeSize;
    }

    void Update() {
        if (_currentJobs == null || _todoJobs == null)
            return;
        /*
         * Another way to keep track of the threads we have open would have been to create 
         * a new thread for it but we can also just use Unity's main thread too since this class
         * derives from monoBehaviour
         */

        int i = 0;

        while(i < _currentJobs.Count) {
            if(_currentJobs[i].JobDone) {
                _currentJobs[i].NotifyComplete();
                _currentJobs.RemoveAt(i);
            } else {
                i++;
            }
        }

        if(_todoJobs.Count > 0 && _currentJobs.Count < _maxJobs) {
            PathFinder job = _todoJobs[0];
            _todoJobs.RemoveAt(0);
            _currentJobs.Add(job);

            //Start a new thread

            Thread jobThread = new Thread(job.FindPath);
            jobThread.Start();

            //As per the doc
            //https://msdn.microsoft.com/en-us/library/system.threading.thread(v=vs.110).aspx
            //It is not necessary to retain a reference to a Thread object once you have started the thread. 
            //The thread continues to execute until the thread procedure is complete.				
        }
    }

    public void RequestPathfind(Node start, Node target, PathfindingJobComplete completeCallback) {
        PathFinder newJob = new PathFinder(start, target, completeCallback, _nodeSize, _maximumLoopCount);
        _todoJobs.Add(newJob);
    }
}
