using UnityEngine;

public static partial class CrowdAnimTest
{
    public static void Apply(Entity entity)
    {
        if (entity is not UserEntity)
            return;
        ApplyGo(entity.gameObject);
    }
}
