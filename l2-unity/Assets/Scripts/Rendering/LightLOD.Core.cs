#if UNITY_EDITOR
public partial class LightLOD : MonoBehaviour
{
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
}
