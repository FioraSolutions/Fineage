using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public sealed partial class L2TerrainLayerStack
{
    void OnEnable()
    {
        _renderer = GetComponent<MeshRenderer>();
        _filter = GetComponent<MeshFilter>();
        _layerShader = Shader.Find(LayerShaderName);
        EnsureNativeVertexGrid();
        if (!Active.Contains(this))
            Active.Add(this);
    }

    void OnDisable()
    {
        Active.Remove(this);
        ReleaseOverlays();
    }

    void OnDestroy()
    {
        Active.Remove(this);
        ReleaseOverlays();
    }
}
