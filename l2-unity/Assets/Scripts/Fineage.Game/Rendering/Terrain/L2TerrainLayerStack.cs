using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// Extra terrain draws after the opaque base: one DrawMesh per enabled official
/// overlay. Count comes from the _Layers / _Splatmaps Texture2DArray depth (or
/// from 2D _Layer_N slots when no arrays are bound). Submitted from
/// L2TerrainOverlayRenderPass (AfterRenderingOpaques).
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
[DefaultExecutionOrder(50)]
public sealed partial class L2TerrainLayerStack : MonoBehaviour
{
    public const string BaseShaderName = "L2/World/TerrainBaseModulate2X";
    public const string LayerShaderName = "L2/World/TerrainLayerModulate2X";
    public const int FirstOverlay = 1;

    static readonly int LayerTexId = Shader.PropertyToID("_LayerTex");
    static readonly int LayerMaskId = Shader.PropertyToID("_LayerMask");
    static readonly int LayersId = Shader.PropertyToID("_Layers");
    static readonly int SplatmapsId = Shader.PropertyToID("_Splatmaps");
    static readonly int UseSplatArraysId = Shader.PropertyToID("_UseSplatArrays");
    static readonly int LayerUvId = Shader.PropertyToID("_LayerUV");
    static readonly int SplatSliceId = Shader.PropertyToID("_SplatSlice");
    static readonly int LayerIndexId = Shader.PropertyToID("_LayerIndex");
    static readonly int WorldUvId = Shader.PropertyToID("_L2WorldUvPerTileUu");
    static readonly int CullId = Shader.PropertyToID("_Cull");
    static readonly List<L2TerrainLayerStack> Active = new List<L2TerrainLayerStack>(8);

    MeshRenderer _renderer;
    MeshFilter _filter;
    Shader _layerShader;
    Material[] _overlayMats = new Material[0];
    bool[] _active = new bool[0];
    int _lastOverlay;
    static bool _loggedOnce;
    static bool _loggedAttach;

    void EnsureOverlayCapacity(int lastOverlay)
    {
        int need = lastOverlay + 1;
        if (_overlayMats != null && _overlayMats.Length >= need)
            return;
        System.Array.Resize(ref _overlayMats, need);
        System.Array.Resize(ref _active, need);
    }

    public static bool HasOverlays
    {
        get
        {
            for (int i = 0; i < Active.Count; i++)
            {
                L2TerrainLayerStack stack = Active[i];
                if (stack != null && stack.isActiveAndEnabled)
                    return true;
            }
            return false;
        }
    }

}
