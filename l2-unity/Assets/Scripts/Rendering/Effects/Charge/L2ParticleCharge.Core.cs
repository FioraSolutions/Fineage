using UnityEngine;

public partial class L2ParticleCharge : BaseEffect
{
    public override void Setup(EffectSettings settings, MagicCastData castData, Transform owner)
    {
        base.Setup(settings, castData, owner);
        _settings = settings;
        _castData = castData;


        if (_particleGroups == null || _particleGroups.Length == 0)
            _particleGroups = GetComponentsInChildren<EffectPart>(true);


        foreach (var group in _particleGroups)
            group.Setup(settings, castData);
    }

    public override void Play()
    {
        ResetTimer();
        DestoryEffect(_settings, _castData);
    }
}
