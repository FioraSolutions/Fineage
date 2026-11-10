using System;
using UnityEngine;

/// <summary>
/// L2 SkyMeshActor16: L2sky_Cylinder + WhiteRing (RenderDoc EID 1661).
/// One instance. Opacity is only this slider. Tint follows GetHazeColor LUT day and night.
/// </summary>
[ExecuteInEditMode]
[DisallowMultipleComponent]
public partial class L2HazeRing : MonoBehaviour
{
    const string TexPath = "Data/Clock/WhiteRing";
    const string ShaderName = "L2/Sky/HazeRing";
    const string MeshName = "L2sky_Cylinder2_top80";
    const string MeshObjectName = "L2HazeRingMesh";

    /// Native cylinder: horizon ring Y=0, soft top Y=10 (~+21°), xz radius ~26.05.
    /// Soft band is 20% shorter than native (40% cut, then +20% back).
    public const float MeshHorizonY = 0f;
    public const float MeshTopY = 8f;
    public const float MeshRadius = 26.0515f;

    /// <summary>World Y above camera of the WhiteRing horizon after sky projection.</summary>
    public static float HorizonHeight(float skyZ)
    {
        return 0f;
    }

    /// <summary>World Y above camera of the WhiteRing soft top (~+17°).</summary>
    public static float SoftTopHeight(float skyZ)
    {
        float z = skyZ > 1f ? skyZ : 88f;
        float len = Mathf.Sqrt(MeshRadius * MeshRadius + MeshTopY * MeshTopY);
        return z * (MeshTopY / len);
    }

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static L2HazeRing Instance { get; private set; }
    public static Color CurrentTint { get; private set; } = L2HazeLut.ColorModifierDefault;

}
