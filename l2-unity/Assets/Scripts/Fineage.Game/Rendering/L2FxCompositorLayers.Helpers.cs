using UnityEngine;

public static partial class L2FxCompositorLayers
{
    public static int ResolveL2CloudsLayer()
    {
        int layer = LayerMask.NameToLayer(L2CloudsName);
        return layer >= 0 ? layer : L2Clouds;
    }

    public static void ApplyL2CloudsLayer(GameObject go)
    {
        if (go == null)
            return;

        SetLayerRecursive(go, ResolveL2CloudsLayer());
    }

    public static void ApplySkillEffectLayer(GameObject go)
    {
        if (go == null)
            return;

        int layer = LayerMask.NameToLayer(SkillEffectName);
        if (layer < 0)
            layer = SkillEffect;

        SetLayerRecursive(go, layer);
    }

    public static void SetLayerRecursive(GameObject go, int layer)
    {
        if (go == null)
            return;

        go.layer = layer;
        Transform t = go.transform;
        for (int i = 0; i < t.childCount; i++)
            SetLayerRecursive(t.GetChild(i).gameObject, layer);
    }
}
