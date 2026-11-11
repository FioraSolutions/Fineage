using UnityEngine;

public static partial class CrowdAnimTest
{
    public static void EnsureTimeCap()
    {
        if (_timeApplied
            && Time.fixedDeltaTime == FixedDt
            && Time.maximumDeltaTime == MaxDt)
            return;
        Time.fixedDeltaTime = FixedDt;
        Time.maximumDeltaTime = MaxDt;
        _timeApplied = true;
    }

    public static void Apply(Entity entity)
    {
        EnsureTimeCap();
        if (entity is not UserEntity)
            return;
        ApplyGo(entity.gameObject);
    }
}
