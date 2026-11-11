using UnityEngine;
using System.Collections.Generic;

public static partial class CharTemplateRegistry
{
    public static float GetRunSpeed(int classId, int sex, float serverValue, bool isTwoHanded)
    {
        if (TryResolveModel(classId, out var model))
        {
            var stats = model.GetData(sex);

            float speed = serverValue * stats.RunK;

            if (isTwoHanded)
                speed *= stats.TwoHandedBonus;

            return Mathf.Clamp(speed, 0.25f, 0.85f);
        }

        return 0.5f;
    }

    public static float GetWalkSpeed(int classId, int sex, float serverValue, bool isTwoHanded)
    {
        if (TryResolveModel(classId, out var model))
        {
            var stats = model.GetData(sex);
            return Mathf.Clamp(serverValue * stats.WalkK, 0.2f, 0.7f);
        }

        return 0.3f;
    }
}
