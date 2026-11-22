#if (UNITY_EDITOR)
using System;
using UnityEngine;

/// <summary>
/// UE2 <c>Class=Camera</c> actor from a map t3d (Lobby.unr viewports).
/// <see cref="fovAngleDeg"/> is the UE2 HORIZONTAL FovAngle; NaN means the actor
/// stores none and inherits the class default (PlayerController.uc FovAngle=50).
/// </summary>
[Serializable]
public class L2T3DCameraInfo
{
    public string name;
    public string tag;
    public Vector3 position;
    public Vector3 eulerAngles;
    public float fovAngleDeg = float.NaN;
}
#endif
