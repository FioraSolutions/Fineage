#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class LightLOD : MonoBehaviour
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

    void Awake()
    {
        CacheLight();
    }

    void Start()
    {
        _ready = false;
        _lastUpdate = 0;
    }

    void CacheLight()
    {
        _light = GetComponent<Light>();
        // Light.shadowResolution is Built-in only; URP logs if assigned.
        _builtInRp = GraphicsSettings.currentRenderPipeline == null;
    }

    void Update()
    {
        if (_light == null)
        {
            CacheLight();
            if (_light == null)
                return;
        }

#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
            _ready = true;
#endif

        if (!_ready)
        {
            if (CameraController.Instance == null || CameraController.Instance.Target == null)
                return;
            if (CameraController.Instance.CurrentDistance > CameraController.Instance.MaxDistance)
                return;
            _ready = true;
        }

        if (Time.time - _lastUpdate < _updateDelay)
            return;

        _lastUpdate = Time.time;
#if UNITY_EDITOR
        AdjustLODQuality(EditorApplication.isPlaying ? Camera.main : Camera.current);
#else
        AdjustLODQuality(Camera.main);
#endif
    }

    void AdjustLODQuality(Camera camera)
    {
        if (camera == null || _light == null)
            return;

        _squareDistanceFromCamera = Vector3.SqrMagnitude(
            camera.transform.position - transform.position);

        for (int i = 0; i < LODLevels.Count; i++)
        {
            if (_squareDistanceFromCamera <= LODLevels[i].MinSquareDistance
                || _squareDistanceFromCamera > LODLevels[i].MaxSquareDistance)
                continue;

            _light.enabled = true;
            _light.shadows = LODLevels[i].LightShadows;
            ApplyShadowResolution(LODLevels[i].ShadowResolution);
            return;
        }

        _light.enabled = false;
    }

    void ApplyShadowResolution(UnityEngine.ShadowResolution lodResolution)
    {
        if (!_builtInRp)
            return;

        UnityEngine.ShadowResolution quality = QualitySettings.shadowResolution;
        UnityEngine.ShadowResolution capped = quality <= lodResolution ? quality : lodResolution;
        _light.shadowResolution = (LightShadowResolution)capped;
    }
}
