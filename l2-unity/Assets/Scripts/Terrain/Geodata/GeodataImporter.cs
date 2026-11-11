using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

public partial class GeodataImporter
{
    private string _geodataEntryName = "geodata";
    private string _mapId;
    private float _nodeSize;
    private Vector3 _origin;
    private long _startTime;
    private byte _maximumLayerCount = 8;
    private int _mapSize = 625;
    private long _timeout = 10000;
    private Node[,,] _mapNodes;
    private GeodataImporterFactory.ImportJobComplete _completeCallback;


    private volatile bool _jobDone = false;
    public bool JobDone { get { return _jobDone; } }






}
