public partial class L2WeaponChargeGroup : BaseEffectGroup, IWeaponEffect
{
    protected override void ApplyShaderParams(Component component, float now, float seed)
    {
        if (component == null) return;

        if (component is Renderer renderer)
        {
            ApplyMaterialsParams(renderer, now, seed);
        }
        else if (component is VisualEffect vfx)
        {
            // Debug.Log($"VFX USE COMPONENT!: {_duration}");
            
            // Защита от деления на ноль
            float safeDuration = Mathf.Max(0.001f, _duration);
            
            if (useFixedLifetime)
            {
                // Debug.Log($"_swordTip размер меча fx1: {_duration}");
                TrySetVfxFloat(vfx, "LifetimeRange", _duration);
                TrySetVfxFloat(vfx, "_LifetimeRange", _duration);
                vfx.Play();
            }
            else
            {
                // Компенсируем закрутку (ForwardOffset): чтобы шаг спирали выглядел одинаково,
                // на длинном оружии нужно больше витков закрутки, а на коротком - меньше.
                // Допустим, 3.5f - это базовая закрутка для эталонного оружия.
                float baseForwardOffset = 3.5f;
                // Для короткого оружия ограничиваем снизу, чтобы витков не было слишком мало
                float forwardOffset = Mathf.Max(3.5f, baseForwardOffset * _timeMultiplier);
                
                // --- СПЕЦИАЛЬНОЕ УСЛОВИЕ ДЛЯ КОРОТКОГО ОРУЖИЯ ---
                // Если оружие короче эталонного пока отключаем смотриться странно Glow пролетает быстро, а этот эффект задерживается
                //if (_timeMultiplier < 1.0f)
                //{
                 //   adjustedLife = 1.5f;       
                //    forwardOffset = 1.3f;      
                 //   float turnsPerSecond = 0.5f;
                 //   vfx.SetFloat("TurnsPerSecond", turnsPerSecond);
                //}
                
                TrySetVfxFloat(vfx, "ForwardOffset", forwardOffset);
                TrySetVfxFloat(vfx, "LifetimeRange", adjustedLife);
                TrySetVfxFloat(vfx, "_LifetimeRange", adjustedLife);
                // Debug.Log($"_swordTip размер меча fx2: {adjustedLife} , {forwardOffset}");
                vfx.Play();
            }
 
        }
    }
}
