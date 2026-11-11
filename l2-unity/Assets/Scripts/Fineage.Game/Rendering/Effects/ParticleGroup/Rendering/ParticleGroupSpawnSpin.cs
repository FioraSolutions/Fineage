using UnityEngine;

public sealed partial class ParticleGroupSpawnSpin : MonoBehaviour
{
    private static readonly int SpinParticlesId = Shader.PropertyToID("_SpinParticles");
    private static readonly int SpinsPerSecondRangeId = Shader.PropertyToID("_SpinsPerSecondRange");
    private static readonly int StartSpinRangeId = Shader.PropertyToID("_StartSpinRange");


}
