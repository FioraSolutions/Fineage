using UnityEngine;

public partial class L2ParticleCharge : BaseEffect
{
    public override void SetProgress(float normalizedTime)
    {
        // Debug.Log("L2ParticleCharge>SetProgress: не реализовано");
        //foreach (var group in _particleGroups)
           // group.SetProgress(normalizedTime);
    }

    public void ResetTimer()
    {

        float duration = (_settings != null) ? _settings.defaultLifeTime : 0.5f;
        AutoScale();
        foreach (var group in _particleGroups)
        {
            // Используем интерфейс IWeaponEffect, о котором говорили ранее
            if (group is IWeaponEffect weaponEffect)
            {
                weaponEffect.SetWeapon(_owner);
            }

  
            group.OwnerTarget = _owner;
            group.FollowTarget = _owner;
            group.PlayPart();
        }
    }

    private void AutoScale()
    {
       if (_owner != null)
        {
            // Используем твой новый метод получения локальных размеров
        MeshFilter meshFilter = _owner.GetComponentInChildren<MeshFilter>();
        float widthMultiplier = 1f;

        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
 
            Vector3 localSize = meshFilter.sharedMesh.bounds.size;
            Vector3 realSize = Vector3.Scale(localSize, meshFilter.transform.lossyScale);

            // Для этого меча ширина - это Y (0.100). 
            // Мы берем Y, так как X - это длина, а Z - слишком тонкая (толщина).
            float currentWidth = realSize.y;

            // Считаем коэффициент: ширина к ширине.
            // Для твоего эталона это будет 0.1007 / 0.1007 = 1.0
            widthMultiplier = currentWidth / ReferenceWeaponWidth;
            
            // Ограничиваем, чтобы эффект не "схлопнулся" и не стал "гигантским"
            widthMultiplier = Mathf.Clamp(widthMultiplier, 0.7f, 2.5f);
        }

        transform.localScale = new Vector3(
            _baseScale, 
            _baseScale, 
            _baseScale * widthMultiplier
        );
        }
    }
}
