using System;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class TimedCompositeEffectBase : BaseEffect
{
    protected EffectSettings _settings;
    protected MagicCastData _castData;
    protected EffectSettings _rootRuntimeSettings;

    private readonly List<EffectSettings> _runtimeSettings = new List<EffectSettings>();

    protected abstract string DebugPrefix { get; }
    protected virtual float RuntimeLifeTimeTailSeconds => 0f;









}
