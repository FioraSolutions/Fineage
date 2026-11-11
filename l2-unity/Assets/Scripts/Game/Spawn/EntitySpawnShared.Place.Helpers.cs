using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public static partial class EntitySpawnShared
{
    static bool TryApplyGroundSnap(GameObject go, EntityIdentity identity)
    {
        if (PetSummonRise.IsRising(go))
            return false;

        Vector3 from = go.transform.position;
        WorldPlacement.PlacementKind kind = identity.EntityType == EntityType.Monster
            ? WorldPlacement.PlacementKind.MonsterSpawn
            : WorldPlacement.PlacementKind.NpcSpawn;
        Vector3 snapped = WorldPlacement.Resolve(kind, from);
        if ((snapped - from).sqrMagnitude < 0.0001f)
            return false;

        identity.Position = snapped;
        WorldPlacement.ApplyPose(go, snapped, identity.Heading);
        WorldPlacement.SettleFeet(go, snapped);
        return true;
    }

    public static void ApplyGroundedTransform(GameObject go, Vector3 position, Quaternion rotation)
    {
        WorldPlacement.ApplyPose(go, position, rotation);
    }

    public static void SanitizeCharacterControllerStepOffset(GameObject root)
    {
        if (root == null)
            return;

        CharacterController[] controllers = root.GetComponentsInChildren<CharacterController>(true);
        for (int i = 0; i < controllers.Length; i++)
        {
            CharacterController cc = controllers[i];
            if (cc == null)
                continue;
            if (cc.radius <= 0f)
                cc.radius = 0.1f;
            if (cc.height < cc.radius * 2f)
                cc.height = cc.radius * 2f;
            // Unity: stepOffset <= height + 2*radius; keep well inside to avoid the warning on scaled rigs.
            float maxStep = Mathf.Max(0f, Mathf.Min(cc.height * 0.5f, cc.height + cc.radius * 2f - 0.01f));
            if (cc.stepOffset > maxStep)
                cc.stepOffset = maxStep;
        }
    }

    public static void DisableLegacyPositionSync(GameObject root)
    {
        if (root == null)
            return;

        NetworkTransformReceive receive = root.GetComponent<NetworkTransformReceive>();
        if (receive != null)
            receive.enabled = false;
    }
}
