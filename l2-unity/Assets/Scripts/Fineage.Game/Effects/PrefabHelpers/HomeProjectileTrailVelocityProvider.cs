using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed partial class HomeProjectileTrailVelocityProvider : MonoBehaviour
{

    [SerializeField] private float _minimumAxisRange = 0.002f;
    [SerializeField] private float _smoothing = 18f;
    [SerializeField] private bool _debugLogs;
    [SerializeField] private List<TrailBinding> _bindings = new List<TrailBinding>();

    private static readonly int StartVelocityRangeXID = Shader.PropertyToID("_StartVelocityRangeX");
    private static readonly int StartVelocityRangeYID = Shader.PropertyToID("_StartVelocityRangeY");
    private static readonly int StartVelocityRangeZID = Shader.PropertyToID("_StartVelocityRangeZ");
    private static readonly int TrailPathFadeTID = Shader.PropertyToID("_TrailPathFadeT");
    private static readonly int TrailPathFadeHeadID = Shader.PropertyToID("_TrailPathFadeHead");
    private static readonly int TrailPathFadeTailID = Shader.PropertyToID("_TrailPathFadeTail");
    private static readonly int StartTimeID = Shader.PropertyToID("_StartTime");
    private static readonly int LifetimeRangeID = Shader.PropertyToID("_LifetimeRange");

    private const float GoldenAngleRad = 2.39996323f;

    private MaterialPropertyBlock _propertyBlock;

    public void ResetRuntimeState()
    {
        OnEnable();
    }
}
