using System;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

/// <summary>
/// EID 1566 myst strip (noise over simple clouds). Disabled from DayNightCycle:
/// the port currently draws a solid colored card and the original effect is
/// not visible. Do not enable until combiner + placement match L2.
/// </summary>
[ExecuteInEditMode]
[DisallowMultipleComponent]
public partial class L2CloudMyst : MonoBehaviour
{
    // Port draws a solid colored card; original 1566 noise is not visible yet.
    public static bool PortEnabled = false;

    const string ShaderName = "L2/Sky/CloudMyst";
    const string TexColor = "Data/Clock/Cloud_MystColor";
    const string TexAlpha = "Data/Clock/Cloud_MystAlpha";
    const string TexNoise = "Data/Clock/Cloud_MystNoise";
    const string MeshObjectName = "L2CloudMystMesh";

    static readonly int CloudColorId = Shader.PropertyToID("_CloudColor");
    static readonly int SkyParamsId = Shader.PropertyToID("_SkyParams");

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static L2CloudMyst Instance { get; private set; }

    [SerializeField] bool _followCamera = true;
    [SerializeField] [Range(0f, 1f)] float _opacity = 1f;
    [Tooltip("How far in front of the camera the myst strip sits before sky-projection.")]
    [SerializeField] float _viewDistance = 55f;
    [Tooltip("Lift into the sky so the strip stays in the clouds, not on the player.")]
    [SerializeField] [Range(0f, 1f)] float _viewLift = 0.35f;

    Texture2D _color, _alpha, _noise;
    Transform _tf;
    MeshFilter _mf;
    MeshRenderer _mr;
    Mesh _mesh;
    Material _mat;
    Camera _cam;

}
