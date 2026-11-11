using UnityEngine;

public sealed partial class ParticleGroupSpawnSpin : MonoBehaviour
{
    private float ResolveSpinRevolutions(float seed)
    {
        if (_usePresetAngles && _presetSpinRevolutions != null && _presetSpinRevolutions.Length > 0)
        {
            int idx = Mathf.Abs(Mathf.FloorToInt(seed * 12.9898f)) % _presetSpinRevolutions.Length;
            return Mathf.Repeat(_presetSpinRevolutions[idx], 1f);
        }

        float t = Mathf.Repeat(Mathf.Abs(seed) * 0.6180339887f, 1f);
        return Mathf.Lerp(_startSpinRevolutions.x, _startSpinRevolutions.y, t);
    }
}
