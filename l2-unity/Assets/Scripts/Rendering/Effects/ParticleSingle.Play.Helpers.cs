using UnityEngine;
using UnityEngine.Rendering;

public partial class ParticleSingle
{
    public override void Setup(EffectSettings s, MagicCastData c)
    {
        _settings = s;
        _castData = c;
        _lifetime.ResolveDurationFromSetup(
            ref _duration,
            _hasFixedDuration,
            _settings,
            _castData,
            out _);
        _lifetime.Duration = _duration;
    }

    public void FitToOwnerWidth(Transform target)
    {
        if (target == null)
        {
            return;
        }

        CharacterController controller = target.GetComponent<CharacterController>();
        if (controller == null)
        {
            return;
        }

        float targetWidth = controller.radius * 2f;
        transform.localScale = new Vector3(targetWidth * 4f, 1f, targetWidth * 4f);
    }

    public void SetRuntimeContinuousLoopOverride(bool hasOverride, bool value)
    {
        _holdController.SetRuntimeLoopOverride(hasOverride, value);

        if ((_lifetime.Active && !_lifetime.Stopped) || _holdController.CastEndFadeRequested)
        {
            ApplyCompositeShaderHoldToAllRuntimeMaterials(Now());
        }
    }
}
