using System;
using UnityEngine;
using VContainer;

public partial class L2StarDome : MonoBehaviour
{
    const string SmallTexPath = "Data/Clock/StarField_smallStar01";
    const string LargeTexPath = "Data/Clock/StarField_largeStar02";
    const string ShaderName = "L2/Sky/StarField";
    const string MeshObjectName = "L2StarDomeMesh";

    static readonly int StarColorId = Shader.PropertyToID("_StarColor");
    static readonly int SkyDistanceId = Shader.PropertyToID("_SkyDistance");

    // high_elf_moon.rdc D3D9 textureFactor (consts.f[0]) at night.
    static readonly Color SmallNightTint = new Color(90f / 255f, 90f / 255f, 90f / 255f, 1f);
    static readonly Color LargeNightTint = new Color(200f / 255f, 200f / 255f, 200f / 255f, 1f);

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static L2StarDome Instance { get; private set; }

    [SerializeField] Texture2D _smallStars;
    [SerializeField] Texture2D _largeStars;
    [SerializeField] bool _followCamera = true;

    Transform _tf;
    MeshFilter _mf;
    MeshRenderer _mr;
    Mesh _mesh;
    Material _smallMat;
    Material _largeMat;
    Camera _cam;


}
