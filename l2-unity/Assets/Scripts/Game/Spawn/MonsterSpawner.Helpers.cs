using UnityEngine;

public sealed partial class MonsterSpawner
{
    public void UpdateInfo(Entity entity, NpcInfoDto npcInfo)
    {
        if (entity == null || !(entity is MonsterEntity))
            return;

        MonsterEntity monster = (MonsterEntity)entity;
        monster.UpdateNpcPAtkSpd((int)npcInfo.Stats.PAtkRealSpeed);
        monster.UpdateNpcRunningSpd(npcInfo.Stats.RunRealSpeed);
        monster.UpdateNpcWalkSpd(npcInfo.Stats.WalkRealSpeed);
        monster.Running = npcInfo.Identity.IsRunning;
        // CharInfoSpeedLog.LogNpcPacket(monster, "OnNpcInfo update");
    }

    private static void InitMonster(Entity npc, GameObject npcGo, IWorldSpawnContext world, IGravity gravity)
    {
        NetworkAnimationController animationController =
            EntitySpawnShared.FindOnPrefab<NetworkAnimationController>(npcGo);
        if (animationController == null)
        {
            // Debug.LogError("MonsterSpawner: NetworkAnimationController missing on " + npcGo.name);
            return;
        }
        EntitySpawnShared.InitializeNpcAnimation(animationController);
        EntitySpawnShared.LogStriderAppear("spawn", npc);
        EntitySpawnShared.BindGearAndAnimation(npc, npcGo, animationController);
        EntitySpawnShared.RegisterAnimation(world, npc.Identity.Id, animationController, npc);
        EntitySpawnShared.ApplyNpcMoveSpeeds(npc, true);

        if (gravity != null)
            gravity.AddGravity(npc.Identity.Id, new GravityData(npc));
    }
}
