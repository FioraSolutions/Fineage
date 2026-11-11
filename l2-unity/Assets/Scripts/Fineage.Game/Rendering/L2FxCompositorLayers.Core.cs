using UnityEngine;

public static partial class L2FxCompositorLayers
{
    public static int ResolveL2SkyLayer()
    {
        int layer = LayerMask.NameToLayer(L2SkyName);
        return layer >= 0 ? layer : L2Sky;
    }

    public static int ResolveL2HazeLayer()
    {
        int layer = LayerMask.NameToLayer(L2HazeName);
        return layer >= 0 ? layer : L2Haze;
    }

    public static void ApplyL2SkyLayer(GameObject go)
    {
        if (go == null)
            return;

        SetLayerRecursive(go, ResolveL2SkyLayer());
    }

    public static void ApplyL2HazeLayer(GameObject go)
    {
        if (go == null)
            return;

        SetLayerRecursive(go, ResolveL2HazeLayer());
    }
}
