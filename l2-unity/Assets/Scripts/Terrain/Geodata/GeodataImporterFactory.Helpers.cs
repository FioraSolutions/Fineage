using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public partial class GeodataImporterFactory : MonoBehaviour
{
    void Update() {
        if (_currentJobs == null || _todoJobs == null)
            return;
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
            GeodataImporter job = _todoJobs[0];
            _todoJobs.RemoveAt(0);
            _currentJobs.Add(job);

            Thread jobThread = new Thread(job.ImportGeodata);
            jobThread.Start();
        }
    }

    public void RequestImportGeodata(string mapId, float nodeSize, byte maximumLayerCount, Vector3 origin, ImportJobComplete completeCallback) {
        GeodataImporter newJob = new GeodataImporter(mapId, nodeSize, maximumLayerCount, origin, completeCallback);
        if(newJob != null) {
            _todoJobs.Add(newJob);
        } else {
            // Debug.LogWarning("New job is null");
        }
    }
}
