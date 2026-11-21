using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// On-screen CPU/GPU frame ms from <see cref="FrameTimingManager"/> (requires enableFrameTimingStats).
/// Enable with env <see cref="FineagePerfOverlaySettings.EnvVarName"/> or <see cref="FineagePerfOverlaySettings.CommandLineFlag"/>.
/// </summary>
public sealed class FineageFrameTimingOverlay : MonoBehaviour
{
    const int TimingBufferSize = 4;

    readonly FrameTiming[] _timings = new FrameTiming[TimingBufferSize];
    readonly GUIStyle _style = new GUIStyle();

    float _smoothedFps;
    double _smoothedCpuMs;
    double _smoothedGpuMs;
    string _lastSampleLine = "FrameTimingManager: waiting for samples";

    void Awake()
    {
        _style.fontSize = 14;
        _style.normal.textColor = Color.white;
        _style.padding = new RectOffset(8, 8, 8, 8);
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        FrameTimingManager.CaptureFrameTimings();

        uint count = FrameTimingManager.GetLatestTimings((uint)_timings.Length, _timings);
        if (count == 0)
            return;

        FrameTiming sample = _timings[0];
        double cpuMs = sample.cpuFrameTime;
        double gpuMs = sample.gpuFrameTime;
        bool gpuTimingAvailable = gpuMs > 0d;
        if (cpuMs <= 0d)
        {
            _lastSampleLine = "FrameTimingManager: waiting for valid CPU sample (enableFrameTimingStats?)";
            return;
        }
        const double smooth = 0.12d;
        _smoothedCpuMs = _smoothedCpuMs <= 0d ? cpuMs : _smoothedCpuMs * (1d - smooth) + cpuMs * smooth;

        if (gpuTimingAvailable)
        {
            _smoothedGpuMs = _smoothedGpuMs <= 0d ? gpuMs : _smoothedGpuMs * (1d - smooth) + gpuMs * smooth;
        }

        float frameMs = Time.unscaledDeltaTime * 1000f;
        float instFps = frameMs > 0.001f ? 1000f / frameMs : 0f;
        _smoothedFps = _smoothedFps <= 0f ? instFps : Mathf.Lerp(_smoothedFps, instFps, 0.12f);

        _lastSampleLine = gpuTimingAvailable
            ? $"fps ~{_smoothedFps:0} | cpu {_smoothedCpuMs:0.00} ms | gpu {_smoothedGpuMs:0.00} ms"
            : $"fps ~{_smoothedFps:0} | cpu {_smoothedCpuMs:0.00} ms | GPU timing unavailable";
    }

    void OnGUI()
    {
        if (!_style.normal.background)
        {
            Texture2D bg = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            bg.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.55f));
            bg.Apply();
            _style.normal.background = bg;
        }

        const float width = 380f;
        const float height = 56f;
        GUILayout.BeginArea(new Rect(8f, 8f, width, height), GUIContent.none, _style);
        GUILayout.Label(_lastSampleLine);
        GUILayout.EndArea();
    }
}