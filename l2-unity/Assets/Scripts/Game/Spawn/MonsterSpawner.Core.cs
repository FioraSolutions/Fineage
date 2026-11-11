using UnityEngine;

public sealed partial class MonsterSpawner
{
    public MonsterSpawner(ObjectPoolManager pool, NpcDecoService deco, GravityNpc gravity = null)
    {
        _pool = pool;
        _deco = deco;
        _gravity = gravity;
    }

    public Entity Spawn(NpcSpawnRequest request, IWorldSpawnContext world)
    {
        if (request == null || world == null)
            return null;

        EntityIdentity identity = request.Identity;
        // Height SoT lives in AcquireNpcGameObject → WorldPlacement (#930).
        GameObject npcGo = EntitySpawnShared.AcquireNpcGameObject(_pool, request.Prefab, identity);
        if (npcGo == null)
            return null;

        npcGo.transform.SetParent(world.MonstersContainer);
        MonsterEntity npc = EntitySpawnShared.FindOnPrefab<MonsterEntity>(npcGo);
        if (npc == null)
        {
            // Debug.LogError(
                // "MonsterSpawner: MonsterEntity missing on " + npcGo.name +
                // " prefab=" + (request.Prefab != null ? request.Prefab.name : "null") +
                // " npcId=" + identity.NpcId);
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

        InitMonster(npc, npcGo, world, _gravity);
        PetSummonRise.TryBegin(npc);
        _deco.Start(npc);
        // CharInfoSpeedLog.LogNpcPacket(npc, "OnNpcInfo spawn");
        world.RegisterNpc(npc);
        return npc;
    }
}
