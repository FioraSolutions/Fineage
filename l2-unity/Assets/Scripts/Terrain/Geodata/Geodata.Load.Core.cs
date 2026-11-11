using System;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;

public partial class Geodata : MonoBehaviour
{
    private void Awake() {
        if(_instance == null) {
            _instance = this;
            GeodataPacketServices.Bind(geodata: this);
        } else {
            Destroy(this);
        }
    }

    void OnDestroy() {
        if (_instance != this)
            return; // duplicate destroyed in Awake
        _geodata?.Clear();
        _instance = null;
    }

    void Start() {
        if (_geodata == null) {
            _geodata = new Dictionary<string, Node[,,]>();
        }
    }
}
