using UnityEngine;

public partial class L2ParticleCharge : BaseEffect
{
    [SerializeField] private EffectPart[] _particleGroups;

    [Header("Масштабирование")]
    [SerializeField] private float _baseScale = 0.012f; // Твой идеальный скейл

    // Твое идеальное число ширины (Ось Y из твоих логов)
    private const float ReferenceWeaponWidth = 0.1007553f; 
    private EffectSettings _settings;
    private MagicCastData _castData;









}
