using System;
using UnityEngine;

public partial class L2HazeRing
{
    void DestroyRing()
    {
        if (_mat != null)
        {
            if (Application.isPlaying)
                Destroy(_mat);
            else
                DestroyImmediate(_mat);
            _mat = null;
        }

        if (_mesh != null)
        {
            if (Application.isPlaying)
                Destroy(_mesh);
            else
                DestroyImmediate(_mesh);
            _mesh = null;
        }

        if (_tf != null)
        {
            if (Application.isPlaying)
                Destroy(_tf.gameObject);
            else
                DestroyImmediate(_tf.gameObject);
            _tf = null;
        }

        _mf = null;
        _mr = null;
    }
}
