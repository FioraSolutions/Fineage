using UnityEngine;

public static partial class EffectLightPlacement
{
    private static CharacterController ResolveCharacterController(Entity target)
    {
        if (target == null)
        {
            return null;
        }

        if (target is MonsterEntity monster)
        {
            CharacterController fromMonster = monster.GetCharacterController();
            if (fromMonster != null)
            {
                return fromMonster;
            }
        }
        else if (target is NpcEntity npc)
        {
            CharacterController fromNpc = npc.GetCharacterController();
            if (fromNpc != null)
            {
                return fromNpc;
            }
        }

        CharacterController controller = target.GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = target.GetComponentInChildren<CharacterController>(true);
        }

        return controller;
    }

    private static Entity TryResolvePlayerTarget()
    {
        World world = WorldApplyPacketServices.GameWorld;
        if (DomainPacketServices.LocalPlayer == null || DomainPacketServices.LocalPlayer.TargetId <= 0 || world == null)
        {
            return null;
        }

        return world.GetEntityNoLockSync(DomainPacketServices.LocalPlayer.TargetId);
    }

    private static Vector3 FlatNormalized(Vector3 v)
    {
        v.y = 0f;
        return v.sqrMagnitude > 0.0001f ? v.normalized : Vector3.zero;
    }
}
