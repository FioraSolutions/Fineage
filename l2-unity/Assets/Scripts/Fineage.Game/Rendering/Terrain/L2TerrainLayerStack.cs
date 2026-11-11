using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// Extra terrain draws after the opaque base: one DrawMesh per enabled
/// _Layer_1..9. Submitted from L2TerrainOverlayRenderPass (AfterRenderingOpaques).
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
[DefaultExecutionOrder(50)]
public sealed partial class L2TerrainLayerStack : MonoBehaviour
{
    public const string BaseShaderName = "L2/World/TerrainBaseModulate2X";
    public const string LayerShaderName = "L2/World/TerrainLayerModulate2X";
    public const int FirstOverlay = 1;
    public const int LastOverlay = 9;

    static readonly int LayerTexId = Shader.PropertyToID("_LayerTex");
    static readonly int LayerMaskId = Shader.PropertyToID("_LayerMask");
    static readonly int LayerUvId = Shader.PropertyToID("_LayerUV");
    static readonly int SplatSliceId = Shader.PropertyToID("_SplatSlice");
    static readonly int LayerIndexId = Shader.PropertyToID("_LayerIndex");
    static readonly int WorldUvId = Shader.PropertyToID("_L2WorldUvPerTileUu");
    static readonly int CullId = Shader.PropertyToID("_Cull");
    static readonly List<L2TerrainLayerStack> Active = new List<L2TerrainLayerStack>(8);

    MeshRenderer _renderer;
    MeshFilter _filter;
    Shader _layerShader;
    readonly Material[] _overlayMats = new Material[LastOverlay + 1];
    readonly bool[] _active = new bool[LastOverlay + 1];
    static bool _loggedOnce;
    static bool _loggedAttach;

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
