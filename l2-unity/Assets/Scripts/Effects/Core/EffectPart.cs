using UnityEngine;

public abstract partial class EffectPart : MonoBehaviour
{

    protected Renderer targetRenderer;
    protected MaterialPropertyBlock propBlock;
    protected EffectSettings _settings;
    protected MagicCastData _castData;
    protected float _baseSize;
    protected const string SHADER_PARAMETR_ALPHA = "_Alpha";
    protected const string SHADER_PARAMETR_MAX_ALPHA = "_MaxAlpha";
    protected const string SHADER_PARAMETR_SPEED = "_Speed";

    protected const string SHADER_PARAMETR_RANGE_MAX_ALPHA = "_MaxAlpha";

    protected const string SHADER_PARAMETR_SCALE_START_SIZE = "_ScaleSizeStart";
    protected const string SHADER_PARAMETR_SCALE_END_SIZE = "_ScaleSizeEnd";
    protected const string SHADER_PARAMETR_SCALE_START_TIME = "_StartTimeScale";
    protected const string SHADER_PARAMETR_SCALE_END_TIME = "_EndTimeScale";

    protected const string SHADER_PARAMETR_START_ALPHA_TIMEV1 = "_StartAlphaTimeV1";
    protected const string SHADER_PARAMETR_END_ALPHA_TIMEV1 = "_EndAlphaTimeV1";
    protected const string SHADER_PARAMETR_START_ALPHA_TIMEV0 = "_StartAlphaTimeV0";
    protected const string SHADER_PARAMETR_END_ALPHA_TIMEV0 = "_EndAlphaTimeV0";





    public virtual Vector3 SurfaceNormal { get; set; }
    public virtual Transform FollowTarget { get; set; }
    public virtual Transform OwnerTarget { get; set; }

    private bool _ownerWorldPosOverrideActive;
    private Vector3 _ownerWorldPosOverride;
    private bool _shaderTargetWorldPosOverrideActive;
    private Vector3 _shaderTargetWorldPosOverride;
    private Transform _shaderTargetWorldPosFollowTransform;
    private Vector3 _shaderTargetWorldPosLocal;







    public abstract void Setup(EffectSettings settings , MagicCastData castData);
    public abstract void PlayPart();
    public abstract void StopPart();




}
