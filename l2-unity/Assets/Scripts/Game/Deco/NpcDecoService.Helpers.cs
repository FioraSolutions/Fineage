using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class NpcDecoService
{
    public void Stop(Entity entity)
    {
        if (entity != null && entity.Identity != null)
            Stop(entity.Identity.Id);
    }

    BaseEffect SpawnPiece(Entity entity, NpcDecoPiece piece)
    {
        if (piece == null || piece.Prefab == null)
            return null;

        if (!NpcDecoAttachment.TryResolve(entity, piece.Attach, out Transform parent, out Vector3 worldPos))
            return null;

        BaseEffect instance = UnityEngine.Object.Instantiate(
            piece.Prefab,
            worldPos,
            NpcDecoAttachment.UprightYaw(parent),
            parent);
        instance.transform.localScale = Vector3.one;
        if (instance.GetComponent<NpcDecoKeepWorldUp>() == null)
            instance.gameObject.AddComponent<NpcDecoKeepWorldUp>();
        instance.gameObject.name = "NpcDeco_" + piece.Label + "_" + piece.Attach;
        instance.gameObject.SetActive(true);
        instance.BindLifetimeToHost();
        BindHostOwnedEmitters(instance);
        instance.Setup(_hostSettings, null, parent);
        instance.Play();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // NpcDeco2911Trace.DumpSpawnedPrefab(instance);
#endif
        return instance;
    }

    static NpcDecoEffect ResolveDeco(Entity entity)
    {
        Npcgrp npcgrp = null;
        if (entity is NpcEntity npc && npc.NpcData != null)
            npcgrp = npc.NpcData.Npcgrp;
        else if (entity is MonsterEntity monster && monster.NpcData != null)
            npcgrp = monster.NpcData.Npcgrp;

        return npcgrp != null ? npcgrp.Deco : null;
    }

    static void BindHostOwnedEmitters(BaseEffect instance)
    {
        ParticleGroupV2[] groups = instance.GetComponentsInChildren<ParticleGroupV2>(true);
        for (int i = 0; i < groups.Length; i++)
        {
            if (groups[i] != null)
                groups[i].BindHostOwnedEmission();
        }
    }
}
