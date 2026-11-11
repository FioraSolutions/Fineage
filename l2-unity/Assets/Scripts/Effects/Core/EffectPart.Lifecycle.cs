using UnityEngine;

public abstract partial class EffectPart : MonoBehaviour
{
    public virtual void Initialize(EffectSettings settings, float baseSize)
    {
        _settings = settings;
        _baseSize = baseSize;

        targetRenderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();



        UpdateShaderFloat(SHADER_PARAMETR_ALPHA, 0);
    }

    protected void UpdateShaderFloat(string name, float value)
    {
        if (targetRenderer == null)
        {
            return;
        }

        // L2Particle calls Setup/PlayPart without Initialize — create on demand.
        if (propBlock == null)
        {
            propBlock = new MaterialPropertyBlock();
        }

        // Сначала получаем текущий блок из рендерера
        targetRenderer.GetPropertyBlock(propBlock);

        // Устанавливаем значение
        propBlock.SetFloat(name, value);

        // ПРИНУДИТЕЛЬНО отдаем его обратно
        targetRenderer.SetPropertyBlock(propBlock);
    }

    public void SetOwnerWorldPosOverride(bool active, Vector3 worldPosition)
    {
        _ownerWorldPosOverrideActive = active;
        if (active)
        {
            _ownerWorldPosOverride = worldPosition;
        }
    }

    protected Vector3 ResolveOwnerWorldPos()
    {
        if (_ownerWorldPosOverrideActive)
        {
            return _ownerWorldPosOverride;
        }

        return ResolveOwnerWorldPosDefault();
    }
}
