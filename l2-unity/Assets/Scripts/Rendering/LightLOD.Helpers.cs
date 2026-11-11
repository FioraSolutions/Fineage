#if UNITY_EDITOR
public partial class LightLOD : MonoBehaviour
{
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
            CameraController camera = CameraPacketServices.Controller;
            if (camera == null || camera.Target == null)
                return;
            if (camera.CurrentDistance > camera.MaxDistance)
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
