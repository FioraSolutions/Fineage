using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class EntitySpawnShared
{
    static readonly HashSet<string> ReportedSubstitutedMeshes = new HashSet<string>();

    public static EntityType ResolveEntityType(GameObject prefab, string className)
    {
        bool hasMonster = FindOnPrefab<MonsterEntity>(prefab) != null;
        bool hasNpc = FindOnPrefab<NpcEntity>(prefab) != null;
        if (hasMonster && !hasNpc)
            return EntityType.Monster;
        if (hasNpc && !hasMonster)
            return EntityType.NPC;
        return EntityTypeParser.ParseEntityType(className);
    }

    /// <summary>
    /// Peaceful Folk that reuse a monster mesh (Nerupa = aracne_m00, spirit animals, …) ship with
    /// <see cref="MonsterEntity"/> only. NpcSpawner needs <see cref="NpcEntity"/> so
    /// click → talk, not auto-attack + retaliate.
    /// Destroy dependent audio first — <see cref="MonsterAnimationAudioHandler"/> RequireComponent-blocks
    /// removing <see cref="MonsterEntity"/> and aborts the whole spawn (empty village).
    /// </summary>
    public static NpcEntity EnsureNpcEntity(GameObject go)
    {
        if (go == null)
            return null;

        NpcEntity npc = FindOnPrefab<NpcEntity>(go);
        if (npc != null)
            return npc;

        MonsterEntity monster = FindOnPrefab<MonsterEntity>(go);
        if (monster == null)
            return null;

        MonsterAnimationAudioHandler monsterAudio = FindOnPrefab<MonsterAnimationAudioHandler>(go);
        if (monsterAudio != null)
            Object.DestroyImmediate(monsterAudio);

        Object.DestroyImmediate(monster);

        if (FindOnPrefab<CharacterAnimationAudioHandler>(go) == null)
            go.AddComponent<CharacterAnimationAudioHandler>();
        return go.AddComponent<NpcEntity>();
    }

    public static T FindOnPrefab<T>(GameObject go) where T : Component
    {
        if (go == null)
            return null;
        T component = go.GetComponent<T>();
        return component != null ? component : go.GetComponentInChildren<T>(true);
    }

}
