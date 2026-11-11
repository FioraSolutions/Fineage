using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Threading;

public partial class GeodataImporterFactory : MonoBehaviour
{
    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
            return;
        }

        _currentJobs = new List<GeodataImporter>();
        _todoJobs = new List<GeodataImporter>();
    }

    void OnDestroy() {
        _instance = null;
    }
}
