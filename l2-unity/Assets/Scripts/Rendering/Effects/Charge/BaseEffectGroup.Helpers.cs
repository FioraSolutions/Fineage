using System;
using UnityEngine;

public abstract partial class BaseEffectGroup : EffectPart
{
    protected void StopEffect()
    {
        _stopped = true;
        if (_effectInstances == null) return;
        foreach (var inst in _effectInstances) inst.Deactivate();
    }

    public override void StopPart() => StopEffect();

    public override void Setup(EffectSettings settings, MagicCastData castData)
    {
        _settings = settings;
        _castData = castData;
        //if (settings != null) _duration = settings.defaultLifeTime;
    }
}
