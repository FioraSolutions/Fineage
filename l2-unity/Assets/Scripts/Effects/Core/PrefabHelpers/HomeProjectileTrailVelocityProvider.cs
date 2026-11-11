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

    public void ResetRuntimeState()
    {
        OnEnable();
    }
}
