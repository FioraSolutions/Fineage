using UnityEngine;

public partial class L2ShaderHoldController
{
    /// <summary>
    /// SizeScale tail starts this fraction of hideWindow before alpha fade (shrink while still visible).
    /// </summary>
    private const float DefaultCastEndHoldReleaseLead = 0.1f;

    public struct Settings
    {
        public float ShaderHold;
        public bool ReleaseByCastProgress;
        public float ReleaseStartNormalized;
        public bool SmoothRelease;
        /// <summary>Fraction of composite hideTime: size release starts this much before alpha fade (cast-end defer only).</summary>
        public float CastEndHoldReleaseLead;
    }

    public struct CastTimeline
    {
        public float Now;
        public float CastStartTime;
        public float SlotDuration;
        public EffectSettings Settings;
    }

    private bool _hasRuntimeLoopOverride;
    private bool _runtimeLoopOverrideValue;
    private bool _castEndFadeRequested;
    private float _castEndHoldSnapshot = -1f;
    private float _baseEmitterAlpha = 1f;
    private bool _loggedReleaseStart;
    private bool _loggedReleaseResume;

    public bool CastEndFadeRequested => _castEndFadeRequested;
    public bool HasRuntimeLoopOverride => _hasRuntimeLoopOverride;
    public bool RuntimeLoopOverrideValue => _runtimeLoopOverrideValue;
    public float BaseEmitterAlpha => _baseEmitterAlpha;











}
