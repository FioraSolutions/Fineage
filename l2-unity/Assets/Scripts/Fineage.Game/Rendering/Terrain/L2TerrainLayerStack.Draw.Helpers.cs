using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public sealed partial class L2TerrainLayerStack
{
    static void LogOnce(string msg)
    {
        if (_loggedOnce)
            return;
        _loggedOnce = true;
        // Debug.Log("[L2TerrainLayerStack] " + msg);
    }

    void ReleaseOverlays()
    {
        for (int i = 0; i < _overlayMats.Length; i++)
        {
            DestroyOverlay(_overlayMats[i]);
            _overlayMats[i] = null;
            if (i < _active.Length)
                _active[i] = false;
        }
    }

    static void DestroyOverlay(Material mat)
    {
        if (mat == null)
            return;
        if (Application.isPlaying)
            Destroy(mat);
        else
            DestroyImmediate(mat);
    }
}
