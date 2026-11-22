using UnityEngine;

/// <summary>Spawns minimal door entities (no npcgrp): targeting proxy + <see cref="DoorNetVisual"/>.</summary>
public static class DoorSpawnShared
{
    public static bool TrySpawn(
        EntityIdentity identity,
        NpcStatusInterlude status,
        Stats stats,
        IWorldSpawnRegistration world)
    {
        if (identity == null || world == null)
            return false;

        var root = new GameObject("Door_" + identity.Name + "_" + identity.Id);
        root.transform.SetParent(world.NpcsContainer);

        root.transform.position = identity.Position;
        root.transform.rotation = identity.Heading;

        var controller = root.AddComponent<CharacterController>();
        controller.radius = 40f;
        controller.height = 80f;
        controller.center = new Vector3(0f, 40f, 0f);

        var monster = root.AddComponent<MonsterEntity>();
        var visual = root.AddComponent<DoorNetVisual>();
        visual.Init(identity.Id);

        monster.Identity = identity;
        monster.Status = status ?? new NpcStatusInterlude();
        monster.Stats = stats ?? new Stats();
        if (monster.Stats.MaxHp <= 0)
            monster.Stats.MaxHp = 100;
        monster.Status.SetHp(monster.Stats.MaxHp);
        monster.Initialize();

        if (NetDoorSession.TryGetStatus(identity.Id, out var doorStatus))
        {
            visual.BindByDoorId(doorStatus.DoorId);
            visual.ApplyStatus(doorStatus.Open, doorStatus.CurHp, doorStatus.MaxHp, doorStatus.Attackable);
        }

        root.SetActive(true);
        world.RegisterNpc(monster);
        return true;
    }
}