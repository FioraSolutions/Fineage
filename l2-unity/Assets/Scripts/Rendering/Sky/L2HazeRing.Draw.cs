using System;
using UnityEngine;

public partial class L2HazeRing : MonoBehaviour
{
    static readonly int HazeColorId = Shader.PropertyToID("_HazeColor");

    static bool s_SessionOpacitySet;
    static float s_SessionOpacity;

    [SerializeField] Texture2D _whiteRing;
    [SerializeField] bool _followCamera = true;
    [Header("Haze")]
    [Tooltip("1 = L2. 0 = off. Does not change the clock.")]
    [SerializeField] [Range(0f, 1f)] float _opacity = 1f;

    Transform _tf;
    MeshFilter _mf;
    MeshRenderer _mr;
    Material _mat;
    Mesh _mesh;
    Camera _cam;

    public float Opacity
    {
        get { return _opacity; }
        set { _opacity = Mathf.Clamp01(value); }
    }

}
