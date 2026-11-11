using UnityEngine;

public sealed partial class ParticleGroupSpawnSpin : MonoBehaviour
{
    [SerializeField] private Vector2 _startSpinRevolutions = new Vector2(0f, 1f);
    [SerializeField] private bool _lockSpinAtSpawn = true;
    [SerializeField] private bool _usePresetAngles;
    [Tooltip("Preset spin steps (revolutions) similar to varied atlas orientation in original.")]
    [SerializeField] private float[] _presetSpinRevolutions = { 0f, 0.25f, 0.5f, 0.75f };

    public void Apply(Renderer renderer, float seed)
    {
        if (renderer == null)
        {
            return;
        }

        float spinRev = ResolveSpinRevolutions(seed);
        Material[] materials = renderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            if (mat == null)
            {
                continue;
            }

            if (mat.HasProperty(SpinParticlesId))
            {
                mat.SetFloat(SpinParticlesId, 1f);
            }

            if (mat.HasProperty(SpinsPerSecondRangeId))
            {
                mat.SetColor(SpinsPerSecondRangeId, new Color(0f, 0f, 0f, 0f));
            }

            if (!mat.HasProperty(StartSpinRangeId))
            {
                continue;
            }

            if (_lockSpinAtSpawn)
            {
                mat.SetColor(StartSpinRangeId, new Color(spinRev, spinRev, 0f, 0f));
            }
            else
            {
                mat.SetColor(StartSpinRangeId, new Color(_startSpinRevolutions.x, _startSpinRevolutions.y, 0f, 0f));
            }
        }
    }
}
