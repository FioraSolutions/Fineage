using UnityEngine;

public static partial class L2FxAdjustParticleLife
{
    public static void ApplyToMaterial(Material material, float authoredMaxLife, float targetLife)
    {
        if (material == null)
        {
            return;
        }

        float authored = Mathf.Max(0.01f, authoredMaxLife);
        float target = Mathf.Max(0.01f, targetLife);
        float delta = target - authored;
        if (Mathf.Abs(delta) < 1e-4f)
        {
            return;
        }

        if (material.HasProperty(LifetimeRangeId))
        {
            Vector4 range = material.GetVector(LifetimeRangeId);
            // L2 FRange::operator+=(delta) on Min and Max.
            float min = Mathf.Max(0.01f, range.x + delta);
            float max = Mathf.Max(min, range.y + delta);
            material.SetVector(LifetimeRangeId, new Vector4(min, max, 0f, 0f));
        }

        AddFloatIfPresent(material, FadeOutStartTimeId, delta);
        AddFloatIfPresent(material, FadeoutStartTimeId, delta);
    }

    static void AddFloatIfPresent(Material material, int propertyId, float delta)
    {
        if (!material.HasProperty(propertyId))
        {
            return;
        }

        material.SetFloat(propertyId, Mathf.Max(0f, material.GetFloat(propertyId) + delta));
    }
}
