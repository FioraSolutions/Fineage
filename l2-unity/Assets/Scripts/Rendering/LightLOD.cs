#if UNITY_EDITOR
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public partial class LightLOD : MonoBehaviour
{
    Light _light;
    bool _builtInRp;

    [SerializeField]
    [Range(0, 15)]
    float _updateDelay = 1f;
    [SerializeField] float _squareDistanceFromCamera;
    [SerializeField] float _lastUpdate;

    [SerializeField]
    List<LODAdjustment> LODLevels = new();

    bool _ready;
}
