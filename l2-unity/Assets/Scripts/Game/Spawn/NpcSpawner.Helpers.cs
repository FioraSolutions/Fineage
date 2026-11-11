using UnityEngine;

public sealed partial class NpcSpawner
{
    public Entity Spawn(NpcSpawnRequest request, IWorldSpawnContext world)
    {
        if (request == null || world == null)
            return null;

        EntityIdentity identity = request.Identity;
        // Prefer-highest spawn snap (AcquireNpcGameObject) + delayed resnap — outdoor Folks were
        // floating on raw geodata Y; DE caves stay on the upper deck via highest-hit, not "keep Z".

        GameObject npcGo = EntitySpawnShared.AcquireNpcGameObject(_pool, request.Prefab, identity);
        if (npcGo == null)
            return null;

        npcGo.transform.SetParent(world.NpcsContainer);
        NpcEntity npc = EntitySpawnShared.EnsureNpcEntity(npcGo);
        if (npc == null)
        {
            UnityEngine.Object.Destroy(npcGo);
            return null;
        }
        npc.NpcData = new NpcData(request.NpcName, request.Npcgrp);
        EntitySpawnShared.ApplyNpcIdentity(npc, request);
        EntitySpawnShared.ResetNpcRuntimeForSpawn(npc);

        npcGo.transform.name = request.Prefab != null
            ? request.Prefab.name + "_" + identity.Name
            : identity.Name;
        EntitySpawnShared.SanitizeCharacterControllerStepOffset(npcGo);
        EntitySpawnShared.DisableLegacyPositionSync(npcGo);
        npcGo.SetActive(true);
        EntitySpawnShared.ReapplyGroundAfterActivate(npcGo, identity);

        InitNpc(npc, npcGo, world);
        PetSummonRise.TryBegin(npc);
        _deco.Start(npc);
        world.RegisterNpc(npc);
        if (_appearFade != null && !npc.IsDead())
            _appearFade.Begin(npc);
        return npc;
    }

    private static void InitNpc(Entity npc, GameObject npcGo, IWorldSpawnContext world)
    {
        NetworkAnimationController animationController =
            EntitySpawnShared.FindOnPrefab<NetworkAnimationController>(npcGo);
        if (animationController == null)
        {
            // Debug.LogError("NpcSpawner: NetworkAnimationController missing on " + npcGo.name);
            return;
        }
        EntitySpawnShared.InitializeNpcAnimation(animationController);
        EntitySpawnShared.LogStriderAppear("spawn", npc);
        EntitySpawnShared.BindGearAndAnimation(npc, npcGo, animationController);
        EntitySpawnShared.RegisterAnimation(world, npc.Identity.Id, animationController, npc);
        EntitySpawnShared.ApplyNpcMoveSpeeds(npc, false);

        if (npc.name.Equals("Leandro") || npc.name.Equals("Remy"))
        {
            IGravity gravity = GravityNpcPacketServices.Gravity;
            if (gravity != null)
                gravity.AddGravity(npc.Identity.Id, new GravityData(npc));
        }
    }
}
