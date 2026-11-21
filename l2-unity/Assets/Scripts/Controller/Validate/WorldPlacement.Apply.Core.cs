using UnityEngine;

public static partial class WorldPlacement
{
    public static void ApplyPose(GameObject go, Vector3 position, Quaternion rotation)
    {
        if (go == null)
            return;

        CharacterController[] controllers = go.GetComponentsInChildren<CharacterController>(true);
        bool[] wasEnabled = new bool[controllers.Length];
        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] == null)
                continue;
            wasEnabled[i] = controllers[i].enabled;
            controllers[i].enabled = false;
        }

        go.transform.SetPositionAndRotation(position, rotation);
        Physics.SyncTransforms();

        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] == null)
                continue;
            controllers[i].enabled = wasEnabled[i];
        }
    }

    /// <summary>Mask used by gravity batch rays and KeepOnBrush.</summary>
    public static LayerMask CurrentWalkableMask => WalkableMask(null);

    /// <summary>
    /// Walkable height layers. Water, foliage (Deco), and actors are not ground.
    /// </summary>
    static LayerMask WalkableMask(LayerMask? groundMask)
    {
        LayerMask mask = groundMask.HasValue
            ? groundMask.Value
            : ResolveDefaultGroundMask();
        mask |= LayerMask.GetMask("Default", "Terrain", "StaticMesh", "Brush", "AllowWalk", "Obstacle");
        mask &= ~LayerMask.GetMask(
            "Entity", "EntityClick", "DropItem", "Player", "UI", "Ignore Raycast",
            "SkillEffect", "Deco", "Water", "TransparentFX");
        return mask;
    }
}
