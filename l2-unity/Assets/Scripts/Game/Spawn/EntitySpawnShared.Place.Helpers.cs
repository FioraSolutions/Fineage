using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public static partial class EntitySpawnShared
{
    static bool TryApplyGroundSnap(GameObject go, EntityIdentity identity)
    {
        if (PetSummonRise.IsRising(go))
            return false;

        identity.RememberServerPosition();
        Vector3 from = go.transform.position;
        // Current XY + original server Y.
        Vector3 reference = new Vector3(from.x, identity.ServerPosition.y, from.z);
        WorldPlacement.PlacementKind kind = identity.EntityType == EntityType.Monster
            ? WorldPlacement.PlacementKind.MonsterSpawn
            : WorldPlacement.PlacementKind.NpcSpawn;
        Vector3 snapped = WorldPlacement.Resolve(kind, reference);
        bool moved = (snapped - from).sqrMagnitude >= 0.0001f;
        if (moved)
        {
            identity.Position = snapped;
            WorldPlacement.ApplyPose(go, snapped, identity.Heading);
        }

        Vector3 afterFeet = WorldPlacement.SettleFeet(go, moved ? snapped : from);
        if ((afterFeet - from).sqrMagnitude >= 0.0001f)
        {
            identity.Position = afterFeet;
            L2OverheadAnchor.Invalidate(identity.Id);
            return true;
        }

        if (moved)
            L2OverheadAnchor.Invalidate(identity.Id);
        return moved;
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

    public static void ReapplyGroundForItem(ItemEntity item)
    {
        if (item == null || item.Identity == null)
            return;

        TryResnapGroundItem(item);

        MonoBehaviour host = WorldApplyPacketServices.GameWorld;
        if (host != null)
            host.StartCoroutine(RetryGroundItemSnap(item));
    }

    static bool TryResnapGroundItem(ItemEntity item)
    {
        if (item == null || item.Identity == null)
            return false;

        Vector3 from = item.transform.position;
        Vector3 reference = new Vector3(from.x, item.Identity.ServerPosition.y, from.z);
        var ctx = new WorldPlacement.PlacementFailContext(item.Identity.Id, item.ItemId);
        Vector3 snapped = WorldPlacement.Resolve(
            WorldPlacement.PlacementKind.ItemDrop,
            reference,
            null,
            ctx);
        if ((snapped - from).sqrMagnitude < 0.0001f)
            return false;

        item.Identity.Position = snapped;
        item.transform.position = snapped;
        return true;
    }

    static IEnumerator RetryGroundItemSnap(ItemEntity item)
    {
        float[] at = { 0.25f, 1f, 2f, 5f };
        float elapsed = 0f;
        for (int i = 0; i < at.Length; i++)
        {
            yield return new WaitForSeconds(at[i] - elapsed);
            elapsed = at[i];
            if (item == null)
                yield break;
            TryResnapGroundItem(item);
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
