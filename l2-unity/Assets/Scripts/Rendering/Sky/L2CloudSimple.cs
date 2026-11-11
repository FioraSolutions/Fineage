using System;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;

public partial class L2CloudSimple : MonoBehaviour
{
    const string ShaderName = "L2/Sky/Cloud";
    const string TexDomeA0 = "Data/Clock/Cloud_Final01";
    const string TexDomeA1 = "Data/Clock/Cloud_Final02";
    const string TexDomeB0 = "Data/Clock/Cloud_Final03";
    const string TexDomeB1 = "Data/Clock/Cloud_Silhouette";

    const string MeshCapPath = "Data/StaticMeshes/L2Sky_S/L2sky_sp02";
    const string MeshBeltPath = "Data/StaticMeshes/L2Sky_S/L2sky_sp01";

    static readonly int TextureFactorId = Shader.PropertyToID("_TextureFactor");
    static readonly int SkyParamsId = Shader.PropertyToID("_SkyParams");
    static readonly int SrcBlendId = Shader.PropertyToID("_SrcBlend");
    static readonly int DstBlendId = Shader.PropertyToID("_DstBlend");

    // Cap xz ~1050, belt xz ~258 (package / CSV). Same skyZ, each uses its own radius
    // so the belt sits on the horizon instead of cutting through the village.
    const float SkyCapRadius = 1050f;
    const float SkyBeltRadius = 258f;
    const float BeltNativeBottomY = 0.085f;
    const float BeltNestIntoHaze = 0.75f;

    static readonly Color DaySheetTint = L2CloudLut.SheetFallback;
    static readonly Color DaySilhouetteTint = L2CloudLut.SilhouetteFallback;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static L2CloudSimple Instance { get; private set; }

    [SerializeField] bool _followCamera = true;
    [SerializeField] [Range(0f, 1f)] float _opacity = 1f;

    Texture2D _domeA0, _domeA1, _domeB0, _domeB1;
    Mesh _capMesh, _beltMesh;
    CloudChunk _domeA, _domeB;
    Camera _cam;

    struct CloudChunk
    {
        public Transform tf;
        public MeshFilter mf;
        public MeshRenderer mr;
        public Mesh mesh;
        public Material[] mats;
    }


    /// <summary>
    /// FBX under StaticMeshes. Sky export keeps UE units (no *0.01). Scale Factor = 1.
    /// </summary>
}
